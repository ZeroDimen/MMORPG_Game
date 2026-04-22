using System.Collections;
using UnityEngine;

/// <summary>
/// 위아래로만 이동하는 발판
/// riseHeight로 얼마나 올라갈지 설정, Gizmo로 범위 확인 가능
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

    private Vector3 bottomPos;
    private Vector3 topPos;
    private Transform ridingPlayer;

    void Start()
    {
        bottomPos = transform.position;
        topPos = bottomPos + Vector3.up * riseHeight;

        StartCoroutine(PlatformLoop());
    }

    IEnumerator PlatformLoop()
    {
        while (true)
        {
            // 1. 아래 → 위로 올라가기
            yield return StartCoroutine(MoveTo(topPos, riseSpeed));

            // 2. 위에서 대기
            yield return new WaitForSeconds(waitAtTop);

            if (!loop) yield break;

            // 3. 위 → 아래로 내려가기
            yield return StartCoroutine(MoveTo(bottomPos, descendSpeed));

            // 4. 아래에서 대기
            yield return new WaitForSeconds(waitAtBottom);
        }
    }

    IEnumerator MoveTo(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }

    // ─────────────────────────────────────────
    // 탑승 / 하차 — SetParent 방식
    // ─────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ridingPlayer = other.transform;
        ridingPlayer.SetParent(transform);
        Debug.Log("[MovingPlatform] 탑승!");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (ridingPlayer != null)
        {
            ridingPlayer.SetParent(null);
            ridingPlayer = null;
        }
        Debug.Log("[MovingPlatform] 하차!");
    }

    // ─────────────────────────────────────────
    // Gizmo
    // ─────────────────────────────────────────
    void OnDrawGizmos()
    {
        Vector3 bottom = Application.isPlaying ? bottomPos : transform.position;
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