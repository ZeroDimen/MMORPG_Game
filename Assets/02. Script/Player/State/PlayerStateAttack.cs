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

public void Update()
    {
        // 공격 애니메이션 진행률이 AttackCancelThreshold를 넘으면 이동 입력으로 캔슬 가능
        if (_playerInput.actions["Move"].IsPressed())
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= _playerController.AttackCancelThreshold)
                _playerController.SetState(EPlayerState.Move);
        }
    }

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