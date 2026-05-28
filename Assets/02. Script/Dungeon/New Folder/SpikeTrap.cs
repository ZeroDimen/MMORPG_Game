using System.Collections;
using UnityEngine;

/// <summary>
/// 주기적으로 바닥에서 솟아올랐다가 내려가는 스파이크 함정
/// </summary>
public class SpikeTrap : MonoBehaviour
{
    [Header("이동 설정")]
    public float riseHeight = 2.0f;     // 솟아오르는 높이
    public float riseSpeed = 8.0f;      // 올라오는 속도 (빠르게)
    public float descendSpeed = 2.0f;   // 내려가는 속도 (천천히)

    [Header("타이밍 설정")]
    public float interval = 3.0f;       // 발동 주기 (초)
    public float stayUpTime = 1.0f;     // 올라온 상태 유지 시간
    public float startDelay = 0f;       // 스파이크마다 다른 딜레이 (엇박자 효과)

    [Header("데미지")]
    public int damage = 25;

    private Vector3 downPos;            // 내려간 위치
    private Vector3 upPos;              // 올라온 위치
    private bool isUp = false;
    private bool isDamaging = false;    // 올라오는 중일 때만 데미지

    void Start()
    {
        downPos = transform.position;
        upPos = downPos + new Vector3(0f, riseHeight, 0f);

        // 각 스파이크마다 startDelay를 다르게 주면 엇박자로 작동!
        StartCoroutine(LoopSpike());
    }

    IEnumerator LoopSpike()
    {
        // 첫 발동 딜레이 (엇박자 효과)
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            // 1. 대기
            yield return new WaitForSeconds(interval);

            // 2. 빠르게 솟아오름
            isDamaging = true;
            yield return StartCoroutine(MoveTo(upPos, riseSpeed));
            isDamaging = false;

            // 3. 위에서 잠시 대기
            yield return new WaitForSeconds(stayUpTime);

            // 4. 천천히 내려감
            yield return StartCoroutine(MoveTo(downPos, descendSpeed));
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

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!isDamaging) return;  // 올라오는 순간에만 데미지

        // var hp = other.GetComponent<PlayerHealth>();
        // if (hp != null) hp.TakeDamage(damage);
        Debug.Log($"[SpikeTrap] 스파이크 데미지: {damage}");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 up = transform.position + Vector3.up * riseHeight;
        Gizmos.DrawLine(transform.position, up);
        Gizmos.DrawWireCube(up, transform.localScale);
    }
}