using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyState
{
    protected EnemyController _enemyController;
    protected Animator _animator;
    protected NavMeshAgent _navMeshAgent;

    protected EnemyStatus _enemyStatus;
    public EnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent)
    {
        _enemyController = enemyController;
        _animator = animator;
        _navMeshAgent = navMeshAgent;
    }
}