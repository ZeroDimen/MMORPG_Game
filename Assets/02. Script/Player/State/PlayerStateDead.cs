using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateDead: PlayerState, ICharacterState
{
    public PlayerStateDead(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        _animator.SetTrigger(PlayerAniParamDead);
    }

    public void Update() { }
    public void Exit() { }
}