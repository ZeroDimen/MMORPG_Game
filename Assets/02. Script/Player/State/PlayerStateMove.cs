using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateMove: PlayerState, ICharacterState
{
    private float _moveSpeed;
    private bool currentIsRunning;
    
    public PlayerStateMove(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        _animator.SetBool(PlayerAniParamMove, true);
        
        // AudioManager._instance.SfxPlay("Walk", true);
        _playerController.GiveSfxPlay("Walk", true);
        currentIsRunning = false;
        
        
        // Player Input에 대한 액션 할당
        _playerInput.actions["Fire"].performed += Attack;
        _playerInput.actions["Jump"].performed += Jump;
        
        // moveSpeed 초기화
        _moveSpeed = 0f;
    }

    public void Update()
    {
        // 캐릭터 방향 설정
        var moveVector = _playerInput.actions["Move"].ReadValue<Vector2>();
        if (moveVector != Vector2.zero)
        {
            Rotate(moveVector.x, moveVector.y);
        }
        else
        {
            _playerController.SetState(EPlayerState.Idle);
        }
        
        // 이동 스피드 설정
        var isRun = _playerInput.actions["Run"].IsPressed();
        if (isRun && _moveSpeed < 1f)
        {
            _moveSpeed += Time.deltaTime;
            _moveSpeed = Mathf.Clamp01(_moveSpeed);
            wakingAudio(true);
        }
        else if (!isRun && _moveSpeed > 0f)
        {
            _moveSpeed -= Time.deltaTime * _playerController.BreakForce;
            _moveSpeed = Mathf.Clamp01(_moveSpeed);
            wakingAudio(false);
        }
        _animator.SetFloat(PlayerAniParamMoveSpeed, _moveSpeed);
    }

    private void wakingAudio(bool isRunning)
    {
        if (currentIsRunning != isRunning)
        {
            currentIsRunning = isRunning;
            if (isRunning)
            {
                // AudioManager._instance.SfxStop();
                // AudioManager._instance.SfxPlay("Run", true);
                _playerController.GiveSfxStop();
                _playerController.GiveSfxPlay("Run", true);
            }
            else
            {
                // AudioManager._instance.SfxStop();
                // AudioManager._instance.SfxPlay("Walk", true);
                _playerController.GiveSfxStop();
                _playerController.GiveSfxPlay("Walk", true);
            }
        }
    }

    public void Exit()
    {
        _animator.SetBool(PlayerAniParamMove, false);
        AudioManager._instance.SfxStop();
        _playerController.GiveSfxStop();
        // Player Input에 대한 액션 할당 해제
        _playerInput.actions["Fire"].performed -= Attack;
        _playerInput.actions["Jump"].performed -= Jump;
    }
}