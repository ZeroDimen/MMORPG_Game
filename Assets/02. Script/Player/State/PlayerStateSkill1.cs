using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateSkill1: PlayerState, ICharacterState
{
    public PlayerStateSkill1(PlayerController playerController, Animator animator, PlayerInput playerInput,
        SkillManager skillManager)
        : base(playerController, animator, playerInput)
    {
        _skillManager = skillManager;
    }

    public void Enter()
    {
        if (!_skillManager.IsSkillUnlocked(0) || !_skillManager.GetSkillUse(0))
        {
            Debug.Log("스킬 1을 사용할 수 없습니다.");
            _playerController.SetState(EPlayerState.Idle);
            return;
        }
        // Skill1 애니메이션 실행
        _animator.SetTrigger(PlayerAniParamSkill1);
        _playerController.GiveSfxPlay("Skill_Fire");
        _skillManager.StartCooltime(0);
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}