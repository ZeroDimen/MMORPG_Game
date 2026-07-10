using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateSkill2: PlayerState, ICharacterState
{
    public PlayerStateSkill2(PlayerController playerController, Animator animator, PlayerInput playerInput,
        SkillManager skillManager)
        : base(playerController, animator, playerInput)
    {
        _skillManager = skillManager;
    }

    public void Enter()
    {
        if (!_skillManager.GetSkillUse(1))
        {
            Debug.Log("스킬 2을 사용할 수 없습니다.");
            _playerController.SetState(EPlayerState.Idle);
            return;
        }
        // Skill2 애니메이션 실행
        _animator.SetTrigger(PlayerAniParamSkill2);
        _playerController.GiveSfxPlay("Skill_Wind");
        _skillManager.StartCooltime(1);
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}