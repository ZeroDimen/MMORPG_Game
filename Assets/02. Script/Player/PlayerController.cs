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
    
    [Header("이동")] 
    [SerializeField] [Range(1, 5)] private float breakForce = 1f;
    
    [Header("Status")]
    [SerializeField]
    protected internal PlayerStatus playerStatus;
    
    [SerializeField] private float jumpHeight = 2f;
    
    public float BreakForce => breakForce;
    
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
        
        
        // 상태 객체 초기화
        var playerStateIdle = new PlayerStateIdle(this,  _animator, _playerInput);
        var playerStateMove = new PlayerStateMove(this,  _animator, _playerInput);
        var playerStateJump = new PlayerStateJump(this,  _animator, _playerInput);
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
            { EPlayerState.Attack, playerStateAttack },
            { EPlayerState.Hit, playerStateHit },
            { EPlayerState.Dead, playerStateDead },
            { EPlayerState.Emotion1, playerStateEmotion1 },
            { EPlayerState.Emotion2, playerStateEmotion2 },
        };

        if (photonView.IsMine)
        {
            // chatting 상호작용
            _playerInput.actions["Chat"].performed += _ => GameManager.Instance.SetChattingInputField();
        }
        _playerHpBarController = GetComponent<PlayerHPBarController>();
        GameManager.Instance.SetGameState(EGameState.Play);
    }

    private void OnEnable()
    {
        // GameManager에서 LocalPlayer → PlayerController 접근할 수 있도록 설정
        if (photonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.TagObject = this;
        }
        
        // 카메라 초기화
        _playerInput.camera = Camera.main;
        if (_playerInput.camera != null && photonView.IsMine)
        {
            _playerInput.camera.GetComponent<CameraController>().SetTarget(headTransform, _playerInput);
        }
        
        // 상태 초기화
        State = EPlayerState.None;
        SetLevel();
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
    
    
    public void SetHit(int damage, Vector3 attackDirection)
    {
        if (!photonView.IsMine) return;
        
        int processDamage = damage - playerStatus.defense;
        playerStatus.hp -= processDamage;

        float result = (float)playerStatus.hp / playerStatus.maxHp;

        _playerHpBarController.SetHp(result);
        
        if (playerStatus.hp <= 0)
        {
            SetState(EPlayerState.Dead);
            _playerHpBarController.SetHp($"0 / {playerStatus.maxHp}");
        }
        else
        {
            SetState(EPlayerState.Hit);
            _playerHpBarController.SetHp($"{playerStatus.hp} / {playerStatus.maxHp}");
        }
    }

    public void SetExp(int amount)
    {
        playerStatus.exp += amount;
        SetLevel();
        _playerHpBarController.SetExp($"LV : {playerStatus.level} | {playerStatus.exp} / {playerStatus.maxExp}");
        
    }

    private void SetLevel()
    {
        playerStatus.maxExp = playerStatus.level * 10;
        if (playerStatus.exp >= playerStatus.maxExp)
        {
            playerStatus.exp -= playerStatus.maxExp;
            playerStatus.level++;
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
}