using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateAttack: PlayerState, ICharacterState
{
    public PlayerStateAttack(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }
    
    public void Enter()
    {
        _animator.SetTrigger(PlayerAniParamAttack);
        // 연속 공격 추가시 필요함
        // _playerInput.actions["Fire"].performed += AttackTrigger;
    }

    public void Update() { }

    public void Exit()
    {
        // _playerInput.actions["Fire"].performed -= AttackTrigger;
    }

    private void AttackTrigger(InputAction.CallbackContext context)
    {
        _animator.SetTrigger(PlayerAniParamAttack);
    }
}