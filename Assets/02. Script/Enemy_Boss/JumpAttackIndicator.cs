using UnityEngine;

/// <summary>
/// 보스 점프 공격 AoE 인디케이터
/// 바닥에 생성되어 체공 중 위험 구역을 플레이어에게 표시
/// </summary>
public class JumpAttackIndicator : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 3.0f;
    [SerializeField] private float minAlpha   = 0.2f;
    [SerializeField] private float maxAlpha   = 0.7f;

    private MeshRenderer _meshRenderer;
    private Material     _material;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        // 반투명 빨간 머티리얼 런타임 생성
        _material = new Material(Shader.Find("Sprites/Default"));
        _material.color = new Color(1f, 0.1f, 0.1f, maxAlpha);
        _meshRenderer.material = _material;
    }

    private void Update()
    {
        // 알파 펄스 효과
        float alpha = Mathf.Lerp(minAlpha, maxAlpha,
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
        Color c = _material.color;
        c.a = alpha;
        _material.color = c;
    }

    private void OnDestroy()
    {
        if (_material != null)
            Destroy(_material);
    }
}
