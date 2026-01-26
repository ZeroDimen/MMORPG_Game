using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateEmotion2: PlayerState, ICharacterState
{
    public PlayerStateEmotion2(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        // Emotion2 애니메이션 실행
        _animator.SetTrigger(PlayerAniParamEmotion2);
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}