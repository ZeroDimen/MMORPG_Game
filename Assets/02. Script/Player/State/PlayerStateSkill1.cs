using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateSkill1: PlayerState, ICharacterState
{
    public PlayerStateSkill1(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        // Emotion1 애니메이션 실행
        _animator.SetTrigger(PlayerAniParamSkill1);
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}