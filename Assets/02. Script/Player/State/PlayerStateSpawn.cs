using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateSpawn: PlayerState, ICharacterState
{
    public PlayerStateSpawn(PlayerController playerController, Animator animator, PlayerInput playerInput) 
        : base(playerController, animator, playerInput) { }

    public void Enter()
    {
        _animator.SetTrigger(PlayerAniParamSpawn);
    }

    public void Update() { }
    public void Exit() { }
}