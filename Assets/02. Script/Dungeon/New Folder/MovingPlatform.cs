using System.Collections;
using UnityEngine;

/// <summary>
/// 위아래로만 이동하는 발판
/// riseHeight로 얼마나 올라갈지 설정, Gizmo로 범위 확인 가능
/// 
/// [카메라 떨림 방지]
/// - 이동은 FixedUpdate에서 처리해서 물리/카메라 타이밍에 맞춤
/// - 자식으로 붙은 플레이어(SetParent)도 FixedUpdate 타이밍에 함께 이동
/// - Cinemachine Brain의 UpdateMethod = LateUpdate 로 설정되어 있어야 함
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Header("이동 설정")]
    public float riseHeight = 5f;       // 얼마나 위로 올라갈지
    public float riseSpeed = 2f;        // 올라가는 속도
    public float descendSpeed = 3f;     // 내려오는 속도
    public float waitAtTop = 1f;        // 위에서 대기 시간
    public float waitAtBottom = 1f;     // 아래에서 대기 시간

    [Header("왕복 설정")]
    public bool loop = true;            // true = 계속 왕복, false = 위에서 멈춤

    // 내부 상태 머신
    private enum State { MovingUp, WaitTop, MovingDown, WaitBottom, Stopped }
    private State _state = State.MovingUp;
    private float _waitTimer;

    private Vector3 _bottomPos;
    private Vector3 _topPos;
    private Transform _ridingPlayer;

    void Start()
    {
        _bottomPos = transform.position;
        _topPos = _bottomPos + Vector3.up * riseHeight;
        _state = State.MovingUp;
    }

    // ─────────────────────────────────────────
    // FixedUpdate에서 이동 처리
    // → 자식인 플레이어도 같은 타이밍에 이동
    // → 카메라(LateUpdate)는 이동이 끝난 후 위치를 읽기 때문에 떨림 없음
    // ─────────────────────────────────────────
    void FixedUpdate()
    {
        switch (_state)
        {
            case State.MovingUp:
                MoveStep(_topPos, riseSpeed);
                if (ReachedTarget(_topPos))
                {
                    _state = State.WaitTop;
                    _waitTimer = waitAtTop;
                }
                break;

            case State.WaitTop:
                _waitTimer -= Time.fixedDeltaTime;
                if (_waitTimer <= 0f)
                {
                    if (!loop)
                    {
                        _state = State.Stopped;
                    }
                    else
                    {
                        _state = State.MovingDown;
                    }
                }
                break;

            case State.MovingDown:
                MoveStep(_bottomPos, descendSpeed);
                if (ReachedTarget(_bottomPos))
                {
                    _state = State.WaitBottom;
                    _waitTimer = waitAtBottom;
                }
                break;

            case State.WaitBottom:
                _waitTimer -= Time.fixedDeltaTime;
                if (_waitTimer <= 0f)
                {
                    _state = State.MovingUp;
                }
                break;

            case State.Stopped:
                // 아무것도 하지 않음
                break;
        }
    }

    private void MoveStep(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(
            transform.position, target, speed * Time.fixedDeltaTime);
    }

    private bool ReachedTarget(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) <= 0.01f)
        {
            transform.position = target;
            return true;
        }
        return false;
    }

    // ─────────────────────────────────────────
    // 탑승 / 하차 — SetParent 방식
    // ─────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _ridingPlayer = other.transform;
        _ridingPlayer.SetParent(transform);
        Debug.Log("[MovingPlatform] 탑승!");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_ridingPlayer != null)
        {
            _ridingPlayer.SetParent(null);
            _ridingPlayer = null;
        }
        Debug.Log("[MovingPlatform] 하차!");
    }

    // ─────────────────────────────────────────
    // Gizmo
    // ─────────────────────────────────────────
    void OnDrawGizmos()
    {
        Vector3 bottom = Application.isPlaying ? _bottomPos : transform.position;
        Vector3 top = bottom + Vector3.up * riseHeight;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(bottom, transform.localScale);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(top, transform.localScale);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            bottom + Vector3.up * (transform.localScale.y * 0.5f),
            top - Vector3.up * (transform.localScale.y * 0.5f)
        );

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(bottom + Vector3.up * (riseHeight * 0.5f), 0.15f);
    }
}
