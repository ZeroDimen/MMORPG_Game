using UnityEngine;

public class BrickTrigger : MonoBehaviour
{
    [Header("연결된 낙하 벽돌들")]
    public FallingBrick[] bricks;

    // ─────────────────────────────────────────
    // 플레이어 입장 → 모든 벽돌 반복 시작
    // ─────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var brick in bricks)
            if (brick != null) brick.StartLoop();

        Debug.Log("[BrickTrigger] 벽돌 작동 시작!");
    }

    // ─────────────────────────────────────────
    // 플레이어 이탈 → 모든 벽돌 중단 + 원위치
    // ─────────────────────────────────────────
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var brick in bricks)
            if (brick != null) brick.StopLoop();

        Debug.Log("[BrickTrigger] 벽돌 작동 중단!");
    }
}