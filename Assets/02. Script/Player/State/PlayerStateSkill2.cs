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
        // Skill2 애니메이션 실행
        _animator.SetTrigger(PlayerAniParamSkill2);
        _skillManager.StartCooltime(1);
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}