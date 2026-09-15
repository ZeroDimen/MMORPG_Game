using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateAttack: PlayerState, ICharacterState
{
    public PlayerStateAttack(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }
    
public void Enter()
    {
        _playerController.IsAttacking = true;
        _animator.SetTrigger(PlayerAniParamAttack);
        _playerController.GiveSfxPlay("Attack");
        
        // 연속 공격 추가시 필요함
        // _playerInput.actions["Fire"].performed += AttackTrigger;
    }

    public void Update() { }

public void Exit()
    {
        _playerController.IsAttacking = false;
        // _playerInput.actions["Fire"].performed -= AttackTrigger;
    }

    private void AttackTrigger(InputAction.CallbackContext context)
    {
        _animator.SetTrigger(PlayerAniParamAttack);
    }
}