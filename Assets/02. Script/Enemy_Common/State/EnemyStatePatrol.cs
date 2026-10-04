using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStatePatrol: EnemyState, ICharacterState
{
    private float _waitTime;
    
    public EnemyStatePatrol(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _waitTime = 0f;
        _navMeshAgent.isStopped = false;
        _animator.SetBool(EnemyAniParamPatrol, true);
    }

    public void Update()
    {
        // Patrol > Chase 상태로 전환 조건
        var detectionTargetTransform = _enemyController.DetectionTargetInCircle();
        if (detectionTargetTransform && _waitTime > _enemyController.ChaseWaitTime)
        {
            _navMeshAgent.SetDestination(detectionTargetTransform.position);
            _enemyController.SetState(EEnemyState.Chase);
        } 
        else if (!_navMeshAgent.pathPending &&
                 _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            _enemyController.SetState(EEnemyState.Idle);
        }
        // _waitTime 추가
        _waitTime += Time.deltaTime;
    }

    public void Exit()
    {
        _animator.SetBool(EnemyAniParamPatrol, false);
    }
}