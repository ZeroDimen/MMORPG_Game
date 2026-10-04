using UnityEngine;

/// <summary>
/// 바닥 위에 살짝 띄워서 배치하는 감지용 트리거
/// FloorTile 오브젝트의 실제 콜라이더와 분리하여 플레이어 감지
/// </summary>
public class FloorTileTrigger : MonoBehaviour
{
    [Header("연결된 실제 바닥")]
    public FloorTile targetTile;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        targetTile.Activate();
    }
}