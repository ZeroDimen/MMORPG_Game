using System;
using System.Collections;
using UnityEngine;

public class FallingBrick : MonoBehaviour
{
    [Header("위치 설정")]
    public float waitHeight = 8f;
    public float groundOffset = 0.5f;

    [Header("낙하 설정")]
    public float fallSpeed = 15f;
    public float fallDelay = 0.5f;      // 처음 낙하 전 대기

    [Header("반복 설정")]
    public float riseSpeed = 1.5f;      // 올라가는 속도 (천천히)
    public float waitAtBottom = 1.5f;   // 바닥 대기 시간
    public float waitAtTop = 1.0f;      // 꼭대기 대기 시간 (다음 낙하 전)

    [Header("데미지")]
    public int damage = 30;

    private Vector3 startPos;
    private Vector3 groundPos;
    private Coroutine loopCoroutine;
    private bool isRunning = false;

    void Start()
    {
        startPos = transform.position;
        groundPos = new Vector3(
            startPos.x,
            startPos.y - waitHeight + groundOffset,
            startPos.z
        );
    }

    // ─────────────────────────────────────────
    // BrickTrigger에서 호출 — 반복 시작
    // ─────────────────────────────────────────
    public void StartLoop()
    {
        if (isRunning) return;
        isRunning = true;
        loopCoroutine = StartCoroutine(FallLoop());
    }

    // ─────────────────────────────────────────
    // BrickTrigger에서 호출 — 반복 중단 + 원위치
    // ─────────────────────────────────────────
    public void StopLoop()
    {
        if (!isRunning) return;
        isRunning = false;

        // 현재 진행 중인 코루틴 즉시 중단
        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);

        // 원위치로 천천히 복귀
        StartCoroutine(ReturnToStart());
    }

    // ─────────────────────────────────────────
    // 핵심: 무한 반복 낙하 루프
    // ─────────────────────────────────────────
    IEnumerator FallLoop()
    {
        // 처음 한 번만 딜레이 (긴장감)
        yield return new WaitForSeconds(fallDelay);

        while (isRunning)
        {
            // 1. 빠르게 낙하
            yield return StartCoroutine(MoveTo(groundPos, fallSpeed));

            // 2. 바닥에서 대기
            yield return new WaitForSeconds(waitAtBottom);

            // 3. 천천히 복귀
            yield return StartCoroutine(MoveTo(startPos, riseSpeed));

            // 4. 꼭대기에서 잠깐 대기 후 다시 낙하
            yield return new WaitForSeconds(waitAtTop);
        }
    }

    // ─────────────────────────────────────────
    // 통로 이탈 시 원위치 복귀
    // ─────────────────────────────────────────
    IEnumerator ReturnToStart()
    {
        yield return StartCoroutine(MoveTo(startPos, riseSpeed));
        Debug.Log("[FallingBrick] 원위치 복귀 완료");
    }

    // ─────────────────────────────────────────
    // 이동 코루틴
    // ─────────────────────────────────────────
    IEnumerator MoveTo(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
        transform.position = target;
    }

    // ─────────────────────────────────────────
    // 데미지 처리
    // ─────────────────────────────────────────
    // void OnCollisionEnter(Collision col)
    // {
    //     if (!col.gameObject.CompareTag("Player")) return;
    //     if (!isRunning) return;
    //
    //     var player = col.transform.GetComponent<PlayerController>();
    //     if (!player.photonView.IsMine) return;
    //     player.SetHit(15);
    //     
    //     Debug.Log($"[FallingBrick] 낙하 데미지: {damage}");
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (!isRunning) return;
        
        var player = other.transform.GetComponent<PlayerController>();
        if (!player.photonView.IsMine) return;
        if(player != null)
            player.SetHit(15);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isRunning ? Color.red : Color.yellow;
        Vector3 bottom = transform.position + Vector3.down * (waitHeight - groundOffset);
        Gizmos.DrawLine(transform.position, bottom);
        Gizmos.DrawWireCube(bottom, transform.localScale);
    }
}