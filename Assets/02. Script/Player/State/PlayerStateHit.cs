using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateHit: PlayerState, ICharacterState
{
    public PlayerStateHit(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        _animator.SetTrigger(PlayerAniParamHit);
    }

    public void Update() { }
    public void Exit() { }
}