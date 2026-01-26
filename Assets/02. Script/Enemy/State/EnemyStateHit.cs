using UnityEngine;
using UnityEngine.AI;
using static Constants;
public class EnemyStateHit: EnemyState, ICharacterState
{
    public EnemyStateHit(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent)
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _navMeshAgent.isStopped = true;
        _animator.SetTrigger(EnemyAniParamHit);
    }

    public void Update() { }
    public void Exit() { }
}