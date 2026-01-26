using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateIdle: EnemyState, ICharacterState
{
    private float _waitTime;
    
    public EnemyStateIdle(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _waitTime = 0f;
        _navMeshAgent.isStopped = true;
        _animator.SetBool(EnemyAniParamIdle, true);
    }

    public void Update()
    {
        var detectionTargetTransform = _enemyController.DetectionTargetInCircle();
        if (detectionTargetTransform && _waitTime > _enemyController.ChaseWaitTime)
        {
            _navMeshAgent.SetDestination(detectionTargetTransform.position);
            _enemyController.SetState(EEnemyState.Chase);
        } 
        else if (_waitTime > _enemyController.PatrolWaitTime)
        {
            var randomValue = Random.Range(0, 100);
            if (randomValue < _enemyController.PatrolChance)
            {
                // 정찰 시작
                var patrolPosition = FindRandomPatrolPosition();

                // 정찰 위치가 현 위치에서 2unit 이상 벗어날 경우 정찰 시작
                var realDistance = Vector3.Magnitude(patrolPosition - _enemyController.transform.position);
                var minimumDistance = _navMeshAgent.stoppingDistance + 2;
                if (realDistance > minimumDistance)
                {
                    _navMeshAgent.SetDestination(patrolPosition);
                    _enemyController.SetState(EEnemyState.Patrol);
                }
            }
            _waitTime = 0f;
        }
        _waitTime += Time.deltaTime;
    }

    public void Exit()
    {
        _animator.SetBool(EnemyAniParamIdle, false);
    }

    // 정찰 목적지를 반환하는 함수
    private Vector3 FindRandomPatrolPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _enemyController.PatrolDetectionDistance;
        randomDirection += _enemyController.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, _enemyController.PatrolDetectionDistance,
                NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return _enemyController.transform.position;  
        }
    }
}