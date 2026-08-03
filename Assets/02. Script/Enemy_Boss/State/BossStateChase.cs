using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class BossStateChase: EnemyState, ICharacterState
{
    private float _waitTime;
    private int attacknum = 0;

    public BossStateChase(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent,
        EnemyStatus enemyStatus)
        : base(enemyController, animator, navMeshAgent)
    {
        _enemyStatus = enemyStatus;
    }

    public void Enter()
    {
        _navMeshAgent.isStopped = false;
        _enemyController.RpcSetBool(EnemyAniParamChase, true); // 로컬에서 직접 수정하지 않고, RPC로 모든 클라이언트의 Animator에 동기화

        _waitTime = 0f;
    }

    public void Update()
    {
        Phase1();
    }

    public void Exit()
    {
        _enemyController.RpcSetBool(EnemyAniParamChase, false);
    }
    
    private bool DetectionTargetInSight(Vector3 position)
    {
        var cosTheta = Vector3.Dot(_enemyController.transform.forward,
            (position - _enemyController.transform.position).normalized);
        var angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;

        if (angle < _enemyController.DetectionSightAngle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Phase1()
    {
        var detectionTargetTransform = _enemyController.DetectionTargetInCircle();
        if (detectionTargetTransform)
        {
            // 공격
            if (!_navMeshAgent.pathPending &&
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance * 1.75f &&
                _waitTime > _enemyController.AttackWaitTime &&
                DetectionTargetInSight(detectionTargetTransform.position))
            
            {
                _enemyController.SetState(EEnemyState.Attack);
            }
            // 공격
            else if (!_navMeshAgent.pathPending &&
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance * 20f &&
                _waitTime > _enemyController.AttackWaitTime &&
                // DetectionTargetInSight(detectionTargetTransform.position) &&
                _enemyStatus.hp <= _enemyStatus.maxHp / 2)
            {
                _enemyController.SetState(EEnemyState.Skill1);
            }
            else
            {
                _waitTime = 0f;
            }
            
            // 달리기 구현
            if (DetectionTargetInSight(detectionTargetTransform.position)
                && _navMeshAgent.remainingDistance > _enemyController.MinimumRunDistance)
            {
                _enemyController.RpcSetFloat(EnemyAniParamMoveSpeed, 1f);
            }
            else
            {
                _enemyController.RpcSetFloat(EnemyAniParamMoveSpeed, 0f);
            }
            
            _navMeshAgent.SetDestination(detectionTargetTransform.position);
        }
        else
        {
            _enemyController.SetState(EEnemyState.Idle);
        }
        
        _waitTime += Time.deltaTime;
    }
}