using UnityEngine;

/// <summary>
/// 위아래로만 이동하는 발판
/// riseHeight로 얼마나 올라갈지 설정, Gizmo로 범위 확인 가능
///
/// [카메라 떨림 방지]
/// - 이동은 FixedUpdate에서 처리
/// - 플레이어는 SetParent 대신 "이번 스텝 이동량(델타)"을 직접 더해서 따라오게 함
/// - Cinemachine Brain의 UpdateMethod = SmartUpdate(또는 FixedUpdate) 권장
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Header("이동 설정")]
    public float riseHeight = 5f;
    public float riseSpeed = 2f;
    public float descendSpeed = 3f;
    public float waitAtTop = 1f;
    public float waitAtBottom = 1f;

    [Header("왕복 설정")]
    public bool loop = true;

    private enum State { MovingUp, WaitTop, MovingDown, WaitBottom, Stopped }
    private State _state = State.MovingUp;
    private float _waitTimer;

    private Vector3 _bottomPos;
    private Vector3 _topPos;

    private Transform _ridingPlayer;
    private CharacterController _ridingCC;
    private Vector3 _lastPos;   // 직전 스텝 위치 (델타 계산용)

    void Start()
    {
        _bottomPos = transform.position;
        _topPos = _bottomPos + Vector3.up * riseHeight;
        _state = State.MovingUp;
        _lastPos = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 before = transform.position;

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
                    _state = loop ? State.MovingDown : State.Stopped;
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
                    _state = State.MovingUp;
                break;

            case State.Stopped:
                break;
        }

        // 이번 스텝에 발판이 움직인 양만큼 플레이어도 이동
        Vector3 delta = transform.position - before;
        if (_ridingPlayer != null && delta.sqrMagnitude > 0f)
        {
            if (_ridingCC != null && _ridingCC.enabled)
                _ridingCC.Move(delta);
            else
                _ridingPlayer.position += delta;
        }

        _lastPos = transform.position;
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

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _ridingPlayer = other.transform;
        _ridingCC = other.GetComponent<CharacterController>();
        Debug.Log("[MovingPlatform] 탑승!");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _ridingPlayer = null;
        _ridingCC = null;
        Debug.Log("[MovingPlatform] 하차!");
    }

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
            top - Vector3.up * (transform.localScale.y * 0.5f));

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(bottom + Vector3.up * (riseHeight * 0.5f), 0.15f);
    }
}