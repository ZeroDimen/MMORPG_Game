using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class BossStateChase: EnemyState, ICharacterState
{
    private float _waitTime;
    private int attacknum = 0;
    private float _nextSkill1RollTime = 0f; // 체력 50% 초과 구간에서 확률 판정 재시도 간격 관리용

    public BossStateChase(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent,
        EnemyStatus enemyStatus)
        : base(enemyController, animator, navMeshAgent)
    {
        _enemyStatus = enemyStatus;
    }

    public void Enter()
    {
        // NavMesh에 정상 배치된 상태일 때만 Resume(isStopped=false) 호출 - 점프 착지 직후 등 아직 온메쉬에 안정적으로 올라오지 않았을 때 예외 방지
        if (_navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = false;
        }
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
            // remainingDistance/stoppingDistance 대신 실측 거리(직선 거리)로 판정
            float distanceToTarget = Vector3.Distance(_enemyController.transform.position, detectionTargetTransform.position);
            bool inSight = DetectionTargetInSight(detectionTargetTransform.position);

            // 공격 (특정거리 = MinimumRunDistance 안 + 그로기 상태 x)
            if (distanceToTarget <= _enemyController.MinimumRunDistance &&
                _waitTime > _enemyController.AttackWaitTime &&
                inSight && _enemyController.State != EEnemyState.Groggy)
            {
                _enemyController.SetState(EEnemyState.Attack);
                _waitTime += Time.deltaTime;
                return; // 전환 직후 아래 이동/속도 로직이 같은 프레임에 덮어쓰지 않도록 종료
            }
            // 스킬1 (특정거리 밖 + 그로기 상태 x + 그로기 종료 후 쿨타임 7초)
            // 체력 50% 이하면 확정 발동(풀 콤보), 초과면 3초 간격으로 25% 확률 판정(견제용 단발 점프)
            else if (distanceToTarget > _enemyController.MinimumRunDistance &&
                     _waitTime > _enemyController.AttackWaitTime &&
                     inSight && _enemyController.State != EEnemyState.Groggy &&
                     Time.time - _enemyController.LastGroggyEndTime >= 7f &&
                     TrySkill1Trigger())
            {
                _enemyController.SetState(EEnemyState.Skill1);
                _waitTime += Time.deltaTime;
                return; // 전환 직후 아래 이동/속도 로직이 같은 프레임에 덮어쓰지 않도록 종료
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

    // 체력 50% 이하: 항상 발동(풀 콤보). 초과: 3초 간격으로 25% 확률 판정(견제용 단발 점프)
    private bool TrySkill1Trigger()
    {
        if (_enemyStatus.hp <= _enemyStatus.maxHp / 2) return true;

        if (Time.time < _nextSkill1RollTime) return false;

        _nextSkill1RollTime = Time.time + 3f;
        return Random.value < 0.25f;
    }
}