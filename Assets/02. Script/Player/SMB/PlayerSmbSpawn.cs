using UnityEngine;

// Spawn 에니메이션 후 Idle 상태로 전환하기 위한 함수
public class PlayerSmbSpawn : StateMachineBehaviour
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