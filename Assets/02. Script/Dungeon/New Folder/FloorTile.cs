using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어가 밟으면 흔들리다가 사라지고, 일정 시간 후 다시 나타나는 바닥
/// </summary>
public class FloorTile : MonoBehaviour
{
    [Header("타이밍 설정")]
    public float shakeDelay = 0.8f;
    public float disappearTime = 2.0f;
    public float shakeAmount = 0.05f;

    [Header("데미지 (낙사)")]
    public int fallDamage = 50;

    private bool isShaking = false;
    private Vector3 originPos;
    private Renderer rend;
    private Collider col;

    void Start()
    {
        originPos = transform.position;
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }

    // ─────────────────────────────────────────
    // FloorTileTrigger에서 호출
    // ─────────────────────────────────────────
    public void Activate()
    {
        if (isShaking) return;
        StartCoroutine(ShakeAndDisappear());
    }

    IEnumerator ShakeAndDisappear()
    {
        isShaking = true;

        // 1. 흔들리는 연출
        float elapsed = 0f;
        while (elapsed < shakeDelay)
        {
            float offsetX = Random.Range(-shakeAmount, shakeAmount);
            float offsetZ = Random.Range(-shakeAmount, shakeAmount);
            transform.position = originPos + new Vector3(offsetX, 0f, offsetZ);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originPos;

        // 2. 렌더러 + 콜라이더 비활성화
        rend.enabled = false;
        col.enabled = false;

        // 3. 대기
        yield return new WaitForSeconds(disappearTime);

        // 4. 복귀
        rend.enabled = true;
        col.enabled = true;
        isShaking = false;

        Debug.Log("[FloorTile] 바닥 복귀!");
    }
}