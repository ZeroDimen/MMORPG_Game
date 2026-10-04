using UnityEngine;

public class EnemySmbGroggy : StateMachineBehaviour
{
    private EnemyController _enemyController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enemyController) _enemyController = animator.GetComponent<EnemyController>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 그로기 애니메이션이 실제로 끝났을 때만, 아직 코드 State가 Groggy일 때만 Chase로 복귀
        if (_enemyController.State == Constants.EEnemyState.Groggy)
            _enemyController.SetState(Constants.EEnemyState.Chase);
    }
}
