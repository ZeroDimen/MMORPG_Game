using System;
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
    public PlayerStatus Status;
    
    [Header("이동")] 
    [SerializeField] [Range(1, 5)] private float breakForce = 1f;
    
    [SerializeField] private float jumpHeight = 2f;
    
    public float BreakForce => breakForce;

    [SerializeField] private AudioClip[] _audioClips;
    public AudioSource Audio { get; private set; }
    
    // 컴포넌트 캐싱
    private Animator _animator;
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private PlayerHPBarController _playerHpBarController;
    
    // 상태 정보
    public EPlayerState State; 
    private Dictionary<EPlayerState,ICharacterState> _states;
    
    // 캐릭터 이동 정보
    private float _velocityY;
    
    private void Awake()
    {
        // 컴포넌트 초기화
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _characterController = GetComponent<CharacterController>();
        Audio = GetComponent<AudioSource>();
        
        
        // 상태 객체 초기화
        var playerStateIdle = new PlayerStateIdle(this,  _animator, _playerInput);
        var playerStateMove = new PlayerStateMove(this,  _animator, _playerInput);
        var playerStateJump = new PlayerStateJump(this,  _animator, _playerInput);
        var playerStateSpawn = new PlayerStateSpawn(this,  _animator, _playerInput);
        var playerStateAttack = new PlayerStateAttack(this, _animator, _playerInput);
        var playerStateHit = new PlayerStateHit(this, _animator, _playerInput);
        var playerStateDead = new PlayerStateDead(this, _animator, _playerInput);
        var playerStateEmotion1 = new PlayerStateEmotion1(this, _animator, _playerInput);
        var playerStateEmotion2 = new PlayerStateEmotion2(this, _animator, _playerInput);

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
            { EPlayerState.Emotion2, playerStateEmotion2 },
        };
        
        _playerHpBarController = GetComponent<PlayerHPBarController>();
        GameManager.Instance.SetGameState(EGameState.Play);
    }

    protected virtual void Start()
    {
        if (photonView.IsMine)
        {
            SetSpawn();
            // SetState(EPlayerState.Spawn);
            // chatting 상호작용
            _playerInput.actions["Chat"].performed += _ => GameManager.Instance.SetChattingInputField();
            
            // GameManager에서 LocalPlayer → PlayerController 접근할 수 있도록 설정
            PhotonNetwork.LocalPlayer.TagObject = this;
            
        }
        SaveManager.Instance.LoadGameFromMaster(this);
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
        }
        else
        {
            _playerInput.actions.FindAction("Jump").Disable();
            _playerInput.actions.FindAction("Fire").Disable();
            _playerInput.actions.FindAction("Look").Disable();
            _playerInput.actions.FindAction("Move").Disable();
        }
    }
    
    
    public void SetHit(int damage)
    {
        if (!photonView.IsMine) return;
        
        int processDamage = damage - Status.DEF;
        Status.HP -= processDamage;

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
        if (!photonView.IsMine) return;
        GameEvents.OnSetExp?.Invoke(Status.EXP + amount);
        SetLevel();
        _playerHpBarController.SetExp($"LV : {Status.LV} | {Status.EXP} / {Status.MAXEXP}");
        PlayerStatusView.Instance.UpdateStatusUI(Status);
    }

    private void SetLevel()
    {
        GameEvents.OnSetMaxExp?.Invoke(Status.LV * 10);
        if (Status.EXP >= Status.MAXEXP)
        {
            GameEvents.OnSetExp?.Invoke(Status.EXP - Status.MAXEXP);
            GameEvents.OnSetLevel?.Invoke(Status.LV + 1);
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
        if(photonView.ViewID == viewId)
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
        if(photonView.ViewID == viewID)
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
    
    
}