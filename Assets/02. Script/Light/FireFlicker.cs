using UnityEngine;

[RequireComponent(typeof(Light))]
public class FireFlicker : MonoBehaviour
{
    private Light fireLight;

    [Header("기본 밝기")]
    public float baseIntensity = 1.5f;

    [Header("변동 범위")]
    public float intensityVariation = 0.5f;

    [Header("변동 속도")]
    public float flickerSpeed = 2f;

    [Header("Range 변동")]
    public float baseRange = 6f;
    public float rangeVariation = 1f;

    private float noiseOffset;

    void Start()
    {
        fireLight = GetComponent<Light>();
        noiseOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseOffset);

        fireLight.intensity = baseIntensity + (noise - 0.5f) * 2f * intensityVariation;
        fireLight.range = baseRange + (noise - 0.5f) * 2f * rangeVariation;
    }
}