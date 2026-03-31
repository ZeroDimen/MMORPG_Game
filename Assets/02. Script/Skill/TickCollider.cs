using System.Collections;
using UnityEngine;

public class TickCollider : MonoBehaviour
{
    public float tickInterval = 1f; // 오브젝트를 껐다 켤 간격

    [SerializeField] private Collider collider;

    void Awake()
    {
        if (collider == null)
        {
            enabled = false; // 콜라이더가 없으면 스크립트를 비활성화
            return;
        }
        
        collider.enabled = true;
    }
    void OnEnable()
    {
        StartCoroutine(TickSequence());
    }

    IEnumerator TickSequence()
    {
        while (true)
        {
            collider.enabled = !collider.enabled;
            yield return new WaitForSeconds(tickInterval);
        }
        
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }
}