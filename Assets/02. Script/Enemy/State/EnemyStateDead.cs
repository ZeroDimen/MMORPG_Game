using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateDead : EnemyState, ICharacterState
{
    public EnemyStateDead(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.enabled = false;
        
        _animator.SetTrigger(EnemyAniParamDead);
    }

    public void Update() { }

    public void Exit() { }
}