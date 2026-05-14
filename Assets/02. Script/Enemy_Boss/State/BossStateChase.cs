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
        _animator.SetBool(EnemyAniParamChase, true);

        _waitTime = 0f;
    }

    public void Update()
    {
        if (_enemyStatus.hp >= _enemyStatus.maxHp / 2) 
        {
            Phase1();
        }
        else // 최대 체력의 절반 이하일 경우
        {
            Phase2();
        }
    }

    public void Exit()
    {
        _animator.SetBool(EnemyAniParamChase, false);
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
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance &&
                _waitTime > _enemyController.AttackWaitTime &&
                DetectionTargetInSight(detectionTargetTransform.position))
            {
                _enemyController.SetState(EEnemyState.Attack);
            }
            else
            {
                _waitTime = 0f;
            }
            
            // 달리기 구현
            if (DetectionTargetInSight(detectionTargetTransform.position)
                && _navMeshAgent.remainingDistance > _enemyController.MinimumRunDistance)
            {
                _animator.SetFloat(EnemyAniParamMoveSpeed, 1);
            }
            else
            {
                _animator.SetFloat(EnemyAniParamMoveSpeed, 0);
            }
            
            _navMeshAgent.SetDestination(detectionTargetTransform.position);
        }
        else
        {
            _enemyController.SetState(EEnemyState.Idle);
        }
        
        _waitTime += Time.deltaTime;
    }
    private void Phase2()
    {
        var detectionTargetTransform = _enemyController.DetectionTargetInCircle();
        if (detectionTargetTransform)
        {
            // 공격
            if (!_navMeshAgent.pathPending &&
                _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance* 3.5f &&
                _waitTime > _enemyController.AttackWaitTime &&
                DetectionTargetInSight(detectionTargetTransform.position))
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
                _animator.SetFloat(EnemyAniParamMoveSpeed, 0.7f);
            }
            else
            {
                _animator.SetFloat(EnemyAniParamMoveSpeed, 0);
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