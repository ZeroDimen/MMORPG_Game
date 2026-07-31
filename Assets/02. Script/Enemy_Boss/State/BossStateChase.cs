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
            // remainingDistance/stoppingDistance 대신 실제 거리(직선 거리)로 판정 - NavMeshAgent가
            // updatePosition=false + 루트모션 구조라 remainingDistance가 실시간 거리와 어긋날 수 있음
            float distanceToTarget = Vector3.Distance(_enemyController.transform.position, detectionTargetTransform.position);
            bool inSight = DetectionTargetInSight(detectionTargetTransform.position);

            // 공격 (특정거리 = MinimumRunDistance 안)
            if (distanceToTarget <= _enemyController.MinimumRunDistance &&
                _waitTime > _enemyController.AttackWaitTime &&
                inSight)
            {
                _enemyController.SetState(EEnemyState.Attack);
            }
            // 스킬1 (특정거리 밖 + 체력 50% 이하)
            else if (distanceToTarget > _enemyController.MinimumRunDistance &&
                     _waitTime > _enemyController.AttackWaitTime &&
                     inSight &&
                     _enemyStatus.hp <= _enemyStatus.maxHp / 2)
            {
                _enemyController.SetState(EEnemyState.Skill1);
            }
            else
            {
                _waitTime = 0f;
            }

            // 달리기 구현
            if (inSight && distanceToTarget > _enemyController.MinimumRunDistance)
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