using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerState
{
    protected PlayerController _playerController;
    protected Animator _animator;
    protected PlayerInput _playerInput;

    private bool isAttacking = false;
    
    public PlayerState(PlayerController playerController, Animator animator, PlayerInput playerInput)
    {
        _playerController = playerController;
        _animator = animator;
        _playerInput = playerInput;
        _playerInput.actions["Cursor"].performed += OnCursor;
        _playerInput.actions["Cursor"].canceled += OffCursor;
    }

    protected void Attack(InputAction.CallbackContext context)
    {
        if (isAttacking) return;
        _playerController.SetState(EPlayerState.Attack);
    }
    
    protected void Jump(InputAction.CallbackContext context)
    {
        _playerController.Jump();
        _playerController.SetState(EPlayerState.Jump);
    }

    protected void Emotion1(InputAction.CallbackContext context)
    {
        _playerController.SetState(EPlayerState.Emotion1);
    }
    
    protected void Emotion2(InputAction.CallbackContext context)
    {
        _playerController.SetState(EPlayerState.Emotion2);
    }
    
    private void OnCursor(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameState == EGameState.Interaction)
            return;
        GameManager.Instance.SetGameState(EGameState.Alt);
    }
    private void OffCursor(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameState != EGameState.Alt)
            return;
        GameManager.Instance.SetGameState(EGameState.Play);
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
