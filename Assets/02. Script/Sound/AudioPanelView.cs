using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioPanelView : MonoBehaviour
{
    public static AudioPanelView instance;
    
    public Slider MyBGMslider;
    public Slider MySFXslider;
    public Slider OtherSFXslider;
    public Toggle MyBGMtoggle;
    public Toggle MySFXtoggle;
    public Toggle OtherSFXtoggle;
    public TextMeshProUGUI MyBGMvalue;
    public TextMeshProUGUI MySFXvalue;
    public TextMeshProUGUI OtherSFXvalue;

    public List<AudioSource> mySfxAudioSources;
    public List<AudioSource> otherSfxAudioSources;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        MyBGMslider.onValueChanged.AddListener((value) =>
        {
            AudioManager._instance.BgmVolume(value);
            MyBGMvalue.text = Mathf.RoundToInt(value * 100).ToString();
        });
        MySFXslider.onValueChanged.AddListener((value) =>
        {
            AudioManager._instance.SFXVolume(value);
            MySfxValueChange(value);
            MySFXvalue.text = Mathf.RoundToInt(value * 100).ToString();
        });
        OtherSFXslider.onValueChanged.AddListener((value) =>
        {
            OtherSfxValueChange(value);
            OtherSFXvalue.text = Mathf.RoundToInt(value * 100).ToString();
        });
        
        
        MyBGMtoggle.onValueChanged.AddListener((value) =>
        {
            AudioManager._instance.IsSoundMute(value, "BGM");
        });
        MySFXtoggle.onValueChanged.AddListener((value) =>
        {
            AudioManager._instance.IsSoundMute(value, "SFX");
            MySfxMute(value);
        });
        OtherSFXtoggle.onValueChanged.AddListener(OtherSfxMute);
    }

    private void MySfxValueChange(float value)
    {
        foreach (var sound in mySfxAudioSources)
        {
            if(sound != null)
                sound.volume = value;
        }
    }

    private void MySfxMute(bool value)
    {
        foreach (var sound in mySfxAudioSources)
        {
            if (sound != null)
                sound.mute = !value;
        }
    }

    private void OtherSfxValueChange(float value)
    {
        foreach (var sound in otherSfxAudioSources)
        {
            if(sound != null)
                sound.volume = value;
        }
    }
    private void OtherSfxMute(bool value)
    {
        foreach (var sound in otherSfxAudioSources)
        {
            if (sound != null)
                sound.mute = !value;
        }
    }

    public void DataSave()
    {
        // Slider
        PlayerPrefs.SetFloat("MyBGMslider", MyBGMslider.value);
        PlayerPrefs.SetFloat("MySFXslider", MySFXslider.value);
        PlayerPrefs.SetFloat("OtherSFXslider", OtherSFXslider.value);
        
        // Toggle
        PlayerPrefs.SetInt("MyBGMtoggle", MyBGMtoggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("MySFXtoggle", MySFXtoggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("OtherSFXtoggle", OtherSFXtoggle.isOn ? 1 : 0);
        
        PlayerPrefs.Save();
    }

    public void DataLoad()
    {
        // Data Load
        float bgmVol = PlayerPrefs.GetFloat("MyBGMslider", 1f);
        float sfxVol = PlayerPrefs.GetFloat("MySFXslider", 1f);
        float otherSfxVol = PlayerPrefs.GetFloat("OtherSFXslider", 0.7f);

        bool bgmOn = PlayerPrefs.GetInt("MyBGMtoggle", 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt("MySFXtoggle", 1) == 1;
        bool otherSfxOn = PlayerPrefs.GetInt("OtherSFXtoggle", 1) == 1;
        
        // UI
        MyBGMslider.value = bgmVol;
        MySFXslider.value = sfxVol;
        OtherSFXslider.value = otherSfxVol;

        MyBGMtoggle.isOn = bgmOn;
        MySFXtoggle.isOn = sfxOn;
        OtherSFXtoggle.isOn = otherSfxOn;
        
        MyBGMvalue.text = Mathf.RoundToInt(bgmVol * 100).ToString();
        MySFXvalue.text = Mathf.RoundToInt(sfxVol * 100).ToString();
        OtherSFXvalue.text = Mathf.RoundToInt(otherSfxVol * 100).ToString();
        
        // Audio Function
        AudioManager._instance.BgmVolume(bgmVol);
        AudioManager._instance.SFXVolume(sfxVol);
        AudioManager._instance.IsSoundMute(bgmOn, "BGM");
        AudioManager._instance.IsSoundMute(sfxOn, "SFX");
        
        MySfxValueChange(sfxVol);
        OtherSfxValueChange(otherSfxVol);
    }
}
