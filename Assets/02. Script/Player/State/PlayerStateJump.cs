using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateJump: PlayerState, ICharacterState
{
    public PlayerStateJump(PlayerController playerController, Animator animator, PlayerInput playerInput) : base(playerController, animator, playerInput)
    {
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        if (_playerInput.actions["Jump"].IsPressed())
        {
            Debug.Log("_playerInput.actions[\"Move\"].IsPressed");
        }
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}