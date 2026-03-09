using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GraphicSetting
{
    Resolution,
    GameMode,
    Luminosity,
    Frame,
    VSync,
    Texture,
    Shadow,
    AntiAliasing
}

public enum ResolutionList
{
    _1280X800,
    _1920X1080
}

public enum ScreenMode
{
    FullScreen,
    Window
}

public class SettingSelector : MonoBehaviour
{
    public GraphicSetting state;
    public string[] options;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI valueText;
    public int defalutIndex;
    private int currentIndex;
    

    private void Start()
    {
        previousButton.onClick.AddListener(OnPrevious);
        nextButton.onClick.AddListener(OnNext);
    }

    private void OnNext()
    {
        currentIndex = (currentIndex + 1) % options.Length;
        UpdateUI();
    }

    private void OnPrevious()
    {
        currentIndex = (currentIndex - 1 + options.Length) % options.Length;
        UpdateUI();
    }

    private void UpdateUI()
    {
        valueText.text = options[currentIndex];
        SettingManager.Instance.OnGraphicsSettingChanged(state, currentIndex, options[currentIndex]);
    }

    public void LoadData()
    {
        currentIndex = PlayerPrefs.GetInt(state.ToString(), defalutIndex);
        UpdateUI();
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(state.ToString(), currentIndex);
    }

    public void OnVsync(bool onVsync)
    {
        previousButton.interactable = !onVsync;
        nextButton.interactable = !onVsync;
        valueText.color = onVsync ? Color.gray : Color.white;
    }
}
