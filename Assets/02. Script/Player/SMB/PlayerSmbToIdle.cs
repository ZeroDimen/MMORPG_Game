using UnityEngine;

// 에니메이션 후 Idle 상태로 전환하기 위한 함수
public class PlayerSmbToIdle : StateMachineBehaviour
{
    private PlayerController _playerController;
    private Constants.EPlayerState _enteredState;
    
public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerController == null) _playerController = animator.GetComponent<PlayerController>();
        _enteredState = _playerController.State;
    }

public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerController.State == _enteredState)
            _playerController.SetState(Constants.EPlayerState.Idle);
    }
}