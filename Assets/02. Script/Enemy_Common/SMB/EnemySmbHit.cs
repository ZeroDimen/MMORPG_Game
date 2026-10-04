using Unity.VisualScripting;
using UnityEngine;
using static Constants;

public class EnemySmbHit : StateMachineBehaviour
{
    private EnemyController _enemyController;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enemyController) _enemyController = animator.GetComponent<EnemyController>();
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemyController.SetState(EEnemyState.Idle);
    }
}