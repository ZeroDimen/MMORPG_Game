using UnityEngine;

public class PlayerSmbJump : StateMachineBehaviour
{
    private PlayerController _playerController;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerController == null) _playerController = animator.GetComponent<PlayerController>();    
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerController.SetState(Constants.EPlayerState.Idle);
    }
}