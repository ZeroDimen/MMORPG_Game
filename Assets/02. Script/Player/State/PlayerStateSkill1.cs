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
        // Emotion1 애니메이션 실행
        _animator.SetTrigger(PlayerAniParamSkill1);
        _skillManager.StartCooltime(0);
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}