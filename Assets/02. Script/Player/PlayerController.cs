using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private Transform headTransform;
    public SkillManager skillManager;
    public PlayerStatus Status;

    [Header("이동")]
    [SerializeField][Range(1, 5)] private float breakForce = 1f;

    [SerializeField] private float jumpHeight = 2f;

    public float BreakForce => breakForce;

    [SerializeField] private AudioClip[] _audioClips;
    public AudioSource Audio { get; private set; }

    // 컴포넌트 캐싱
    private Animator _animator;
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    public PlayerHPBarController _playerHpBarController { get; private set; }

    // 상태 정보
    public EPlayerState State;
    private Dictionary<EPlayerState, ICharacterState> _states;

    // 캐릭터 이동 정보
    private float _velocityY;
    
    private bool _cursorHeld = false;

    private void Awake()
    {
        // 컴포넌트 초기화
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        Audio = GetComponent<AudioSource>();
        skillManager = GameObject.FindWithTag("SkillManager").GetComponent<SkillManager>();


        // 상태 객체 초기화
        var playerStateIdle = new PlayerStateIdle(this, _animator, _playerInput);
        var playerStateMove = new PlayerStateMove(this, _animator, _playerInput);
        var playerStateJump = new PlayerStateJump(this, _animator, _playerInput);
        var playerStateSpawn = new PlayerStateSpawn(this, _animator, _playerInput);
        var playerStateAttack = new PlayerStateAttack(this, _animator, _playerInput);
        var playerStateHit = new PlayerStateHit(this, _animator, _playerInput);
        var playerStateDead = new PlayerStateDead(this, _animator, _playerInput);
        var playerStateEmotion1 = new PlayerStateEmotion1(this, _animator, _playerInput);
        var playerStateSkill1 = new PlayerStateSkill1(this, _animator, _playerInput, skillManager);
        var playerStateSkill2 = new PlayerStateSkill2(this, _animator, _playerInput, skillManager);

        _states = new Dictionary<EPlayerState, ICharacterState>
        {
            { EPlayerState.Idle, playerStateIdle },
            { EPlayerState.Move, playerStateMove },
            { EPlayerState.Jump, playerStateJump },
            { EPlayerState.Spawn , playerStateSpawn },
            { EPlayerState.Attack, playerStateAttack },
            { EPlayerState.Hit, playerStateHit },
            { EPlayerState.Dead, playerStateDead },
            { EPlayerState.Emotion1, playerStateEmotion1 },
            { EPlayerState.Skill1, playerStateSkill1 },
            { EPlayerState.Skill2, playerStateSkill2 },
        };

        _playerHpBarController = GetComponent<PlayerHPBarController>();
    }

    protected virtual void Start()
    {
        if (photonView.IsMine)
        {
            SetSpawn();

            // chatting 상호작용
            _playerInput.actions["Chat"].performed += OnChat;
            
            _playerInput.actions["Cursor"].performed += OnCursor;
            _playerInput.actions["Cursor"].canceled += OffCursor;

            // GameManager에서 LocalPlayer → PlayerController 접근할 수 있도록 설정
            PhotonNetwork.LocalPlayer.TagObject = this;

            GameManager.Instance.ResetToPlay();
            SaveManager.Instance.LoadGameFromMaster(this);
            StartCoroutine(GetPlayerStatus());
        }
        SaveManager.Instance.LoadGameFromMaster(this);

        if (photonView.IsMine)
        {
            AudioPanelView.instance.mySfxAudioSources.Add(Audio);
            PhotonNetwork.LocalPlayer.TagObject = this;
        }
        else
            AudioPanelView.instance.otherSfxAudioSources.Add(Audio);
        // Boss 등 모든 EnemyController와 내 CharacterController 간 물리 충돌만 무시 (무기 트리거 판정에는 영향 없음)
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            var enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider != null)
            {
                Physics.IgnoreCollision(_characterController, enemyCollider, true);
            }
        }
    }

    private void OnEnable()
    {
        // 상태 초기화
        State = EPlayerState.None;

        _playerInput.camera = Camera.main;
    }

    private void Update()
    {
        if (State != EPlayerState.None && photonView.IsMine)
        {
            _states[State].Update();
        }
    }

    // 새로운 상태를 할당하는 함수
    public void SetState(EPlayerState state)
    {
        if (State == state || !photonView.IsMine) return;
        if (State != EPlayerState.None) _states[State].Exit();
        State = state;
        if (State != EPlayerState.None) _states[State].Enter();
    }

    // EGameState.Interaction일때 조작 비활성화 
    public void SetPlayerInputEnabled(bool enabled)
    {
        if (!photonView.IsMine) return;

        if (enabled)
        {
            _playerInput.actions.FindAction("Jump").Enable();
            _playerInput.actions.FindAction("Fire").Enable();
            _playerInput.actions.FindAction("Look").Enable();
            _playerInput.actions.FindAction("Move").Enable();
            _playerInput.actions.FindAction("Skill1").Enable();
            _playerInput.actions.FindAction("Skill2").Enable();

        }
        else
        {
            if (State == EPlayerState.Move)
                SetState(EPlayerState.Idle);
            
            GiveSfxStop();
            
            _playerInput.actions.FindAction("Jump").Disable();
            _playerInput.actions.FindAction("Fire").Disable();
            _playerInput.actions.FindAction("Look").Disable();
            _playerInput.actions.FindAction("Move").Disable();
            _playerInput.actions.FindAction("Skill1").Disable();
            _playerInput.actions.FindAction("Skill2").Disable();
        }
    }


    public void SetHit(int damage)
    {
        if (!photonView.IsMine || Status == null) return;

        int processDamage = damage - Status.DEF;
        if (processDamage <= 0)
        {
            processDamage = 1;
        }
        Status.SetStatus("HP", Status.HP - processDamage);

        float result = (float)Status.HP / Status.MAXHP;

        _playerHpBarController.SetHp(result);

        if (Status.HP <= 0)
        {
            SetState(EPlayerState.Dead);
            _playerHpBarController.SetHp($"0 / {Status.MAXHP}");
        }
        else
        {
            SetState(EPlayerState.Hit);
            _playerHpBarController.SetHp($"{Status.HP} / {Status.MAXHP}");
        }
    }

    public void SetExp(int amount)
    {
        if (!photonView.IsMine || Status == null) return;

        if (amount != 0)
            Status.SetStatus("EXP", Status.EXP + amount);
        else
            Status.SetStatus("EXP", 0);

        SetLevel();
        _playerHpBarController.SetExp($"LV : {Status.LV} | {Status.EXP} / {Status.MAXEXP}");
        PlayerStatusView.Instance.UpdateStatusUI(Status);
        skillManager.SetSkillData(Status.LV);
    }

    private void SetLevel()
    {
        if (Status == null) return;
        Status.SetStatus("MAXEXP", Status.LV * 10);
        Status.SetStatus("ATK", (int)Math.Round((Status.LV * 2.5) + 50)); // 2.5는 래벨당 성장 공격력
        if (Status.EXP >= Status.MAXEXP)
        {
            Status.SetStatus("EXP", Status.EXP - Status.MAXEXP);
            Status.SetStatus("LV", Status.LV + 1);
            GameEvents.OnPlayerLevelUpEvent?.Invoke();
            SetExp(0);
        }


    }

    // 점프
    public void Jump()
    {
        if (!_characterController.isGrounded) return;
        _velocityY = Mathf.Sqrt(jumpHeight * -2f * Gravity);
    }

    private void OnAnimatorMove()
    {
        if (State == EPlayerState.None) return;

        Vector3 movePosition;
        if (_characterController.isGrounded)
        {
            movePosition = _animator.deltaPosition;

            if (_velocityY < 0f)
                _velocityY = -2f;
        }
        else
        {
            movePosition = _characterController.velocity * Time.deltaTime;
        }

        _velocityY += Gravity * Time.deltaTime;
        movePosition.y = _velocityY * Time.deltaTime;
        _characterController.Move(movePosition);
    }

    [PunRPC]
    public void GiveSfxPlay(string clipName, bool islong = false)
    {
        SfxPlay(clipName, islong);
        var id = photonView.ViewID;
        photonView.RPC(nameof(ReceiveSfxPlay), RpcTarget.Others, clipName, id, islong);
    }

    [PunRPC]
    public void ReceiveSfxPlay(string clipName, int viewId, bool islong)
    {
        if (photonView.ViewID == viewId)
            SfxPlay(clipName, islong);
    }

    public void SfxPlay(string clipName, bool islong) // 효과음을 출력하는 함수
    {
        foreach (var clip in _audioClips)
        {
            if (clip.name == clipName)
            {
                if (!islong)
                {
                    Audio.PlayOneShot(clip);
                    return;
                }
                else
                {
                    Audio.clip = clip;
                    Audio.Play();
                    return;
                }
            }
        }
        Debug.Log($"{clipName} not found");
    }

    [PunRPC]
    public void GiveSfxStop()
    {
        Audio.Stop();
        var id = photonView.ViewID;
        photonView.RPC(nameof(RecieveSfxStop), RpcTarget.Others, id);
    }

    [PunRPC]
    public void RecieveSfxStop(int viewID)
    {
        if (photonView.ViewID == viewID)
            Audio.Stop();
    }

    private void SetSpawn()
    {
        var id = photonView.ViewID;
        photonView.RPC(nameof(ReceiveSpawn), RpcTarget.Others, id);
    }

    [PunRPC]
    public void ReceiveSpawn(int viewID)
    {
        if (photonView.ViewID == viewID)
        {
            SetState(EPlayerState.Spawn);
        }
    }

    IEnumerator GetPlayerStatus()
    {
        yield return new WaitUntil((() => Status != null));
        skillManager.SetSkillData(Status.LV);
    }

    private void OnCursor(InputAction.CallbackContext _)
    {
        if (_cursorHeld) return;
        if (GameManager.Instance.GameState != EGameState.Play) return;

        GameManager.Instance.PushState(EGameState.Interaction);
        _cursorHeld = true;
    }
    
    private void OffCursor(InputAction.CallbackContext _)
    {
        if (!_cursorHeld) return;

        GameManager.Instance.PopState(EGameState.Interaction);
        _cursorHeld = false;
    }
    
    private void OnDestroy()
    {
        if (!photonView.IsMine) return;

        _playerInput.actions["Chat"].performed -= OnChat;
        _playerInput.actions["Cursor"].performed -= OnCursor;
        _playerInput.actions["Cursor"].canceled  -= OffCursor;
    }
    
    private void OnChat(InputAction.CallbackContext _)
        => GameManager.Instance.SetChattingInputField();
}