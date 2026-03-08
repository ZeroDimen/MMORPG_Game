using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;
    [SerializeField] private Button[] TabButtons;
    [SerializeField] private GameObject[] TabMenu;
    
    private int currentTabIndex;

    public Volume globalVolume;
    private ColorAdjustments colorAdjustments;

    private List<SettingSelector> _selectors;
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        SettingSelector[] allSelectors = GetComponentsInChildren<SettingSelector>(true);
        _selectors = new List<SettingSelector>(allSelectors);
    }

    private void Start()
    {
        // TabButtons OnClick AddListener
        for (int i = 0; i < TabButtons.Length; i++)
        {
            int index = i;
            TabButtons[i].onClick.AddListener(() => SelectTab(index));
        }

        globalVolume.profile.TryGet(out colorAdjustments);
        LoadData();
    }

    private void LoadData()
    {
        foreach(var selector in _selectors)
            selector.LoadData();
    }

    public void SaveData()
    {
        foreach(var selector in _selectors)
            selector.SaveData();
    }

    private void SelectTab(int index)
    {
        TabMenu[currentTabIndex].gameObject.SetActive(false);
        TabMenu[index].gameObject.SetActive(true);
        currentTabIndex = index;
    }
    
    public void OnGraphicsSettingChanged(GraphicSetting type, int index, string option)
    {
        switch (type)
        {
            case GraphicSetting.Resolution :
                SetResolution(index);
                break;
            case GraphicSetting.GameMode :
                SetScreenMode(index);
                break;
            case GraphicSetting.Luminosity :
                float value = 0;
                if (index == 0)
                    value = -0.5f;
                else if(index == 1)
                    value = 0f;
                else if(index == 2)
                    value = 0.5f;
                colorAdjustments.postExposure.value = value;
                break;
            case GraphicSetting.Frame :
                int frameRates = int.Parse(option);
                Application.targetFrameRate = frameRates;
                break;
            case GraphicSetting.VSync :
                QualitySettings.vSyncCount = index;
                var selector = _selectors.Find(i => i.state == GraphicSetting.Frame);
                selector.OnVsync(index != 0);
                break;
            case GraphicSetting.Texture :
                QualitySettings.globalTextureMipmapLimit = index;
                break;
            case GraphicSetting.Shadow :
                switch (index)
                {
                    case 0 :
                        QualitySettings.shadowDistance = 100f;
                        break;
                    case 1 : 
                        QualitySettings.shadowDistance = 1000f;
                        break;
                    case 2 :
                        QualitySettings.shadowDistance = 2000f;
                        break;
                }
                break;
            case GraphicSetting.AntiAliasing :
                var cameraData = Camera.main.GetUniversalAdditionalCameraData();
                cameraData.antialiasing = (AntialiasingMode)index;
                // 0 : None, 1 : FXAA(성능), 2 : SMAA(보통), 3 : TAA(고품질)
                break;
        }
    }

    public void SetResolution(int index)
    {
        ResolutionList selected = (ResolutionList)index;
        switch (selected)
        {
            case ResolutionList._1280X800 :
                Screen.SetResolution(1280, 800, Screen.fullScreenMode);
                break;
            case ResolutionList._1920X1080 :
                Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
                break;
        }
    }

    public void SetScreenMode(int index)
    {
        ScreenMode selected = (ScreenMode)index;
        Debug.Log(selected);
        Debug.Log(111);
        switch (selected)
        {
            case ScreenMode.FullScreen :
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case ScreenMode.Window :
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }
}
