using System;
using System.Collections.Generic;
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
        MyBGMslider.onValueChanged.AddListener(AudioManager._instance.BgmVolume);
        MySFXslider.onValueChanged.AddListener((value) =>
        {
            AudioManager._instance.SFXVolume(value);
            MySfxValueChange(value);
        });
        OtherSFXslider.onValueChanged.AddListener(OtherSfxValueChange);
        
        
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

        MyBGMslider.value = AudioManager._instance.audioSources[0].volume;
        MySFXslider.value = AudioManager._instance.audioSources[1].volume;
        MyBGMtoggle.isOn = !AudioManager._instance.audioSources[0].mute;
        MySFXtoggle.isOn = !AudioManager._instance.audioSources[1].mute;
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
}
