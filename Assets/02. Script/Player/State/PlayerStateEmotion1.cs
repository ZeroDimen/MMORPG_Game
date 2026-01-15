using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateEmotion1: PlayerState, ICharacterState
{
    public PlayerStateEmotion1(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        // Emotion1 애니메이션 실행
        _animator.SetTrigger(PlayerAniParamEmotion1);
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}