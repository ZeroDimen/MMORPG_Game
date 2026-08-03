using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviourPun
{
    [Header("AI")] [SerializeField] private float patrolWaitTime = 1f;
    [SerializeField] private float patrolChance = 30f;
    [SerializeField] private float patrolDetectionDistance = 10f;
    [SerializeField] private LayerMask detactionTargetLayerMask;
    [SerializeField] private float chaseWaitTime = 1f;
    [SerializeField] private float detectionSightAngle = 30f;
    [SerializeField] private float minimumRunDistance = 1f;
    [SerializeField] private float attackWaitTime = 0f;

    [Header("Status")] [SerializeField] protected internal EnemyStatus enemyStatus;

    public string partyId;

    // AI 관련
    public float PatrolWaitTime => patrolWaitTime;
    public float PatrolChance => patrolChance;
    public float PatrolDetectionDistance => patrolDetectionDistance;
    public float ChaseWaitTime => chaseWaitTime;
    public float DetectionSightAngle => detectionSightAngle;
    public float MinimumRunDistance => minimumRunDistance;
    public float AttackWaitTime => attackWaitTime;

    private Collider[] _detectionResults = new Collider[1];

    protected Animator _animator;
    protected NavMeshAgent _navMeshAgent;
    private Transform _targetTransform;

    private EnemyHPBarController _enemyHpBarController;

    [SerializeField] private AudioClip[] _audioClips;
    private AudioSource Audio { get; set; }

    // 상태 관리
    public EEnemyState State;
    protected internal Dictionary<EEnemyState, ICharacterState> _states;

    // Dead 연출
    private Rigidbody _rigidbody;
    private Collider _collider;

    // 스폰 위치
    public bool isDungeon = false;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        Audio = GetComponent<AudioSource>();

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
            { EEnemyState.Dead, enemyStateDead }
        };
        SetState(EEnemyState.Idle);

        // HP Bar 할당
        _enemyHpBarController = GetComponent<EnemyHPBarController>();
    }

    private void Start()
    {
        if (photonView.IsMine)
            AudioPanelView.instance.mySfxAudioSources.Add(Audio);
        else
            AudioPanelView.instance.otherSfxAudioSources.Add(Audio);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return; // IsMasterClient만 State를 수정
        if (State != EEnemyState.Dead && State != EEnemyState.None)
        {
            if(_navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
                _states[State].Update();
        }
    }

    public void SetState(EEnemyState state)
    {
        if (!PhotonNetwork.IsMasterClient) return; // IsMasterClient만 State를 수정
        if (State == state) return;
        if (State == EEnemyState.Dead) return;

        if (State != EEnemyState.None) _states[State].Exit();
        State = state;
        if (State != EEnemyState.None) _states[State].Enter();
        
        photonView.RPC(nameof(RPC_SetState), RpcTarget.Others, (int)state);
    }

    [PunRPC]
    public void RPC_SetState(int state)
    {
        if (State == (EEnemyState)state) return;
        if (State == EEnemyState.Dead) return;

        State = (EEnemyState)state;
    }

    public int SetHit(int damage)
    {
        if (State == EEnemyState.Dead) return 0;
        if (_enemyHpBarController)
        {
            enemyStatus.hp -= damage;
            float result = (float)enemyStatus.hp / enemyStatus.maxHp;
            _enemyHpBarController.SetHp(result);
            if (this is BossController)
                UIManager.Instance.UpdateBossHpBar(result);
                

            if (enemyStatus.hp <= 0)
            {
                // 사망 처리
                SetState(EEnemyState.Dead);

                _rigidbody.isKinematic = false;
                _rigidbody.useGravity = true;

                var direction = transform.forward;
                direction.y = 1f;
                direction = direction.normalized;
                var force = direction * 3f;

                _rigidbody.AddForce(force, ForceMode.Impulse);
                _collider.isTrigger = false;

                // 2초 후 비활성화
                StartCoroutine(DisableAfterDelay(3f));

                return enemyStatus.exp;
            }
            else
            {
                // 피격 처리
                SetState(EEnemyState.Hit);
                if (enemyStatus.maxHp / 3 <= damage)
                {
                    StartCoroutine(Knockback(transform.forward));
                }
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

// 타겟 Transform 외부 접근용
    public Transform TargetTransform => _targetTransform;

    // 점프 공격 이동 코루틴 (MasterClient 전용)
// AoE 인디케이터
    private GameObject _jumpIndicator;

    // MasterClient가 호출 → 모든 클라이언트에 인디케이터 표시
    public void ShowJumpIndicator(Vector3 worldPos, float sizeX, float sizeZ)
    {
        photonView.RPC(nameof(RpcShowJumpIndicator), RpcTarget.All, worldPos, sizeX, sizeZ);
    }

    public void HideJumpIndicator()
    {
        photonView.RPC(nameof(RpcHideJumpIndicator), RpcTarget.All);
    }

// 점프 착지 시 데미지 판정 RPC (MasterClient 호출 → 모든 클라이언트 동시 처리)
    public void RpcJumpLandingDamage(Vector3 landPos, Vector3 halfExtents, int damage)
    {
        photonView.RPC(nameof(DoJumpLandingDamage), RpcTarget.All, landPos, halfExtents, damage);
    }

    [PunRPC]
    public void DoJumpLandingDamage(Vector3 landPos, Vector3 halfExtents, int damage)
    {
        // 모든 클라이언트에서 OverlapBox로 범위 안 플레이어 감지
        var hits = Physics.OverlapBox(landPos, halfExtents, Quaternion.identity, detactionTargetLayerMask);
        foreach (var hit in hits)
        {
            var playerController = hit.GetComponent<PlayerController>();
            if (playerController == null) continue;
            // 자신의 클라이언트 소유 플레이어에게만 데미지 적용
            if (!playerController.photonView.IsMine) continue;
            GameManager.Instance.HitPlayer(playerController.photonView, damage);
        }
    }

// 일// 일반 근접 공격(Attack) 데미지 판정 RPC (1회만 호출되어야 함)
    public void RpcAttackDamage()
    {
        Vector3 localCenter = new Vector3(0.10f, 1.11f, 1.46f);
        Vector3 worldCenter = transform.TransformPoint(localCenter);
        Vector3 halfExtents = new Vector3(1.335f, 1.795f, 1.725f);
        Quaternion rot = transform.rotation;
        
        photonView.RPC(nameof(DoAttackDamage), RpcTarget.All, worldCenter, halfExtents, rot, enemyStatus.damage);
    }

    [PunRPC]
    public void DoAttackDamage(Vector3 boxCenter, Vector3 halfExtents, Quaternion rot, int damage)
    {
        var hits = Physics.OverlapBox(boxCenter, halfExtents, rot, detactionTargetLayerMask);
        foreach (var hit in hits)
        {
            var playerController = hit.GetComponent<PlayerController>();
            if (playerController == null) continue;
            if (!playerController.photonView.IsMine) continue;
            GameManager.Instance.HitPlayer(playerController.photonView, damage);
        }
    }



    [PunRPC]
    public void RpcShowJumpIndicator(Vector3 worldPos, float sizeX, float sizeZ)
    {
        if (_jumpIndicator != null) return;
        _jumpIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _jumpIndicator.name = "JumpAttackIndicator";
        Destroy(_jumpIndicator.GetComponent<Collider>());
        _jumpIndicator.transform.position = new Vector3(worldPos.x, 0.05f, worldPos.z);
        _jumpIndicator.transform.localScale = new Vector3(sizeX, 0.02f, sizeZ);
        _jumpIndicator.AddComponent<JumpAttackIndicator>();
    }

    [PunRPC]
    public void RpcHideJumpIndicator()
    {
        if (_jumpIndicator == null) return;
        Destroy(_jumpIndicator);
        _jumpIndicator = null;
    }

    public IEnumerator JumpToTarget(Vector3 targetPos, float duration, float jumpHeight)
    {
        Vector3 startPos = transform.position;
        // Y는 시작/끝 위치 기준, 수평 이동은 XZ만
        Vector3 endPos = new Vector3(targetPos.x, startPos.y, targetPos.z);
        float elapsed = 0f;

        _navMeshAgent.isStopped = true;
        _navMeshAgent.velocity = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // XZ: lerp, Y: 포물선
            Vector3 pos = Vector3.Lerp(startPos, endPos, t);
            pos.y += jumpHeight * Mathf.Sin(Mathf.PI * t);
            transform.position = pos;
            _navMeshAgent.nextPosition = pos;
            yield return null;
        }

        transform.position = endPos;
        _navMeshAgent.isStopped = false;
        _navMeshAgent.Warp(endPos);
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

    [PunRPC]
    public void GiveSfxPlay(string clipName, bool islong = false)
    {
        var id = photonView.ViewID;
        photonView.RPC(nameof(ReceiveSfxPlay), RpcTarget.Others, clipName, id, islong);
    }

    [PunRPC]
    public void ReceiveSfxPlay(string clipName, int viewId, bool islong)
    {
        if (photonView.ViewID == viewId)
            SfxPlay(clipName, islong);
    }

    public void SfxPlay(string clipName, bool islong) // 효과음을 출력하는 함수
    {
        foreach (var clip in _audioClips)
        {
            if (clip.name == clipName)
            {
                if (!islong)
                {
                    Audio.PlayOneShot(clip);
                    return;
                }
                else
                {
                    Audio.clip = clip;
                    Audio.Play();
                    return;
                }
            }
        }

        Debug.Log($"{clipName} not found");
    }

    [PunRPC]
    public void GiveSfxStop()
    {
        Audio.Stop();
        var id = photonView.ViewID;
        photonView.RPC(nameof(RecieveSfxStop), RpcTarget.Others, id);
    }

    [PunRPC]
    public void RecieveSfxStop(int viewID)
    {
        if (photonView.ViewID == viewID)
            Audio.Stop();
    }

    // MasterClient에서 호출 → 모든 클라이언트의 Animator에 Trigger를 동기화
    public void RpcSetTrigger(int paramHash)
    {
        photonView.RPC(nameof(RPC_SetAnimatorTrigger), RpcTarget.All, paramHash);
    }

    // 모든 클라이언트에서 실행 — Animator Trigger 파라미터 적용
    [PunRPC]
    public void RPC_SetAnimatorTrigger(int paramHash)
    {
        _animator.SetTrigger(paramHash);
    }

    // MasterClient에서 호출 → 모든 클라이언트의 Animator에 Bool을 동기화
    public void RpcSetBool(int paramHash, bool value)
    {
        photonView.RPC(nameof(RPC_SetAnimatorBool), RpcTarget.All, paramHash, value);
    }

    // 모든 클라이언트에서 실행 — Animator Bool 파라미터 적용
    [PunRPC]
    public void RPC_SetAnimatorBool(int paramHash, bool value)
    {
        _animator.SetBool(paramHash, value);
    }

    // MasterClient에서 호출 → 모든 클라이언트의 Animator에 Float을 동기화
    public void RpcSetFloat(int paramHash, float value)
    {
        photonView.RPC(nameof(RPC_SetAnimatorFloat), RpcTarget.All, paramHash, value);
    }

    // 모든 클라이언트에서 실행 — Animator Float 파라미터 적용
    [PunRPC]
    public void RPC_SetAnimatorFloat(int paramHash, float value)
    {
        _animator.SetFloat(paramHash, value);
    }
}