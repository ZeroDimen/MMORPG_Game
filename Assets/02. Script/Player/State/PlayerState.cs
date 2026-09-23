using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerState
{
    protected PlayerController _playerController;
    protected Animator _animator;
    protected PlayerInput _playerInput;
    protected SkillManager _skillManager;

    public PlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput)
    {
        _playerController = playerController;
        _animator = animator;
        _playerInput = playerInput;
    }

protected void Attack(InputAction.CallbackContext context)
    {
        if (_playerController.IsAttacking) return;
        _playerController.SetState(EPlayerState.Attack);
    }
    
    protected void Jump(InputAction.CallbackContext context)
    {
        if (!_playerController.IsGrounded) return; // 접지 상태가 아니면 Jump 상태로 전환하지 않음
        _playerController.Jump();
        _playerController.SetState(EPlayerState.Jump);
    }

    protected void Emotion1(InputAction.CallbackContext context)
    {
        _playerController.SetState(EPlayerState.Emotion1);
    }
    
    protected void Skill1(InputAction.CallbackContext context)
    {
        _playerController.SetState(EPlayerState.Skill1);
    }
    
    protected void Skill2(InputAction.CallbackContext context)
    {
        _playerController.SetState(EPlayerState.Skill2);
    }
    
    protected void Rotate(float x, float z)
    {
        if (_playerInput.camera != null)
        {
            var cameraTransform = _playerInput.camera.transform;
            var cameraForward = cameraTransform.forward;
            var cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            var moveDirection = cameraForward * z + cameraRight * x;

            if (moveDirection != Vector3.zero)
            {
                moveDirection.Normalize();
                _playerController.transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
    }
}
