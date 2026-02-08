using UnityEngine;

public class EnemySmbAttack : StateMachineBehaviour
{
    private EnemyController _enemyController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enemyController) _enemyController = animator.GetComponent<EnemyController>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemyController.SetState(Constants.EEnemyState.Chase);
    }
}