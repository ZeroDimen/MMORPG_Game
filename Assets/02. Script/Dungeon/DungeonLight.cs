using System;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonLight : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    private float _directionValue;
    private float _ambientValue;
    private float _reflectionValue;

    private void Start()
    {
        _directionValue = directionalLight.intensity;
        _ambientValue = RenderSettings.ambientIntensity;
        _reflectionValue = RenderSettings.reflectionIntensity;
    }

    public void EnterDungeon()
    {
        directionalLight.intensity = 0;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientIntensity = 0;
        RenderSettings.reflectionIntensity = 0;

        RenderSettings.fog = true;
        RenderSettings.fogDensity = 0.05f;
    }

    public void ExitDungeon()
    {
        directionalLight.intensity = _directionValue;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientIntensity = _ambientValue;
        RenderSettings.reflectionIntensity = _reflectionValue;
        RenderSettings.fog = false;
    }
}
