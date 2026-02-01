using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviourPun
{
    [Header("AI")]
    [SerializeField] private float patrolWaitTime = 1f;
    [SerializeField] private float patrolChance = 30f;
    [SerializeField] private float patrolDetectionDistance = 10f;
    [SerializeField] private LayerMask detactionTargetLayerMask;
    [SerializeField] private float chaseWaitTime = 1f;
    [SerializeField] private float detectionSightAngle = 30f;
    [SerializeField] private float minimumRunDistance = 1f;
    [SerializeField] private float attackWaitTime = 0f;

    [Header("Status")]
    [SerializeField]
    protected internal EnemyStatus enemyStatus;
    
    
    // AI 관련
    public float PatrolWaitTime => patrolWaitTime;
    public float PatrolChance => patrolChance;
    public float PatrolDetectionDistance => patrolDetectionDistance;
    public float ChaseWaitTime => chaseWaitTime;
    public float DetectionSightAngle => detectionSightAngle;
    public float MinimumRunDistance => minimumRunDistance;
    public float AttackWaitTime => attackWaitTime;

    private Collider[] _detectionResults = new Collider[1];
    
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Transform _targetTransform;
    
    private EnemyHPBarController _enemyHpBarController;
    
    // 상태 관리
    public EEnemyState State;
    private Dictionary<EEnemyState, ICharacterState> _states;
    
    // Dead 연출
    private Rigidbody _rigidbody;
    private Collider _collider;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        
        
        // NavMeshAgent 설정
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;
        
        // 플레이어 정보 초기화
        _targetTransform = null;
        
        // 상태 초기화
        var enemyStateIdle = new EnemyStateIdle(this, _animator, _navMeshAgent);
        var enemyStatePatrol = new EnemyStatePatrol(this, _animator, _navMeshAgent);
        var enemyStateChase = new EnemyStateChase(this, _animator, _navMeshAgent);
        var enemyStateAttack = new EnemyStateAttack(this, _animator, _navMeshAgent);
        var enemyStateHit = new EnemyStateHit(this, _animator, _navMeshAgent);
        var enemyStateDead = new EnemyStateDead(this, _animator, _navMeshAgent);
        
        _states = new Dictionary<EEnemyState, ICharacterState>
        {
            { EEnemyState.Idle, enemyStateIdle },
            { EEnemyState.Patrol, enemyStatePatrol },
            { EEnemyState.Chase, enemyStateChase },
            { EEnemyState.Attack, enemyStateAttack },
            { EEnemyState.Hit, enemyStateHit },
            { EEnemyState.Dead, enemyStateDead}
        };
        SetState(EEnemyState.Idle);
        
        // HP Bar 할당
        _enemyHpBarController = GetComponent<EnemyHPBarController>();
    }

    private void Update()
    {
        if (State != EEnemyState.Dead && State != EEnemyState.None)
        {
            _states[State].Update();
        }
    }

    public void SetState(EEnemyState state)
    {
        if (State == state) return;
        if (State == EEnemyState.Dead) return;

        if (State != EEnemyState.None) _states[State].Exit();
        State = state;
        if (State != EEnemyState.None) _states[State].Enter();
    }
    
    public int SetHit(int damage, Vector3 attackDirection)
    {
        if (State == EEnemyState.Dead) return 0;
        if (_enemyHpBarController)
        {
            enemyStatus.hp -= damage;
            float result = (float)enemyStatus.hp / enemyStatus.maxHp;
            _enemyHpBarController.SetHp(result);

            if (enemyStatus.hp <= 0)
            {
                // 사망 처리
                SetState(EEnemyState.Dead);
                
                _rigidbody.isKinematic = false;
                _rigidbody.useGravity = true;
                
                var direction = attackDirection;
                direction.y = 1f;
                direction = direction.normalized;
                var force = direction * 3f;
                
                _rigidbody.AddForce(force, ForceMode.Impulse);
                _collider.isTrigger = false;
                
                // 2초 후 비활성화
                StartCoroutine(DisableAfterDelay(2f));
                
                return enemyStatus.exp;
            }
            else
            {
                // 피격 처리
                SetState(EEnemyState.Hit);
                StartCoroutine(Knockback(attackDirection));
            }
        }
        return 0;
    }
    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private IEnumerator Knockback(Vector3 direction)
    {
        Vector3 knockbackDirection = direction;
        float knockbackDistance = 1f;
        float knockbackDuration = 0.2f;
        float elapsed = 0f;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + knockbackDirection * knockbackDistance;
        targetPosition.y = transform.position.y;

        while (elapsed < knockbackDuration)
        {
            Vector3 lerpPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / knockbackDuration);
            lerpPosition.y = startPosition.y;
            transform.position = lerpPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    private void OnAnimatorMove()
    {
        var position = _animator.rootPosition;
        _navMeshAgent.nextPosition = position;
        transform.position = position;
    }

    // 일정 거리 안에 Player가 있는지 확인 후 있으면 반환
    // 있을 경우, 이미 찾은 상태면 기존 Player 반환
    // 없으면 null 반환
    public Transform DetectionTargetInCircle()
    {
        if (!_targetTransform)
        {
            Physics.OverlapSphereNonAlloc(transform.position, 
                PatrolDetectionDistance, _detectionResults, detactionTargetLayerMask);
            _targetTransform = _detectionResults[0]?.transform;
        }
        else
        {
            float playerDistance = Vector3.Distance(transform.position, _targetTransform.position);
            if (playerDistance > PatrolDetectionDistance)
            {
                _targetTransform = null;
                _detectionResults[0] = null;
            }
        }
        return _targetTransform;
    }
    

    private void OnDrawGizmos()
    {
        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, PatrolDetectionDistance);
        
        // 시야각
        Gizmos.color = Color.red;
        Vector3 rightDirection = Quaternion.Euler(0, detectionSightAngle, 0) * transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -detectionSightAngle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * PatrolDetectionDistance);
        Gizmos.DrawRay(transform.position, leftDirection * PatrolDetectionDistance);
        Gizmos.DrawRay(transform.position, transform.forward * PatrolDetectionDistance);
        
        // Agent 목적지 표시
        if (_navMeshAgent != null && _navMeshAgent.hasPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_navMeshAgent.destination, 0.5f);
            Gizmos.DrawLine(transform.position, _navMeshAgent.destination);
        }
    }
}