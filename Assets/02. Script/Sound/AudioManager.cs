using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;

    public AudioSource[] audioSources;

    [SerializeField] private AudioMixer mixer;   // 인스펙터에서 MainMixer 연결

    private const string BGM_PARAM = "BGMVolume";
    private const string SFX_PARAM = "SFXVolume";
    private const string OTHER_SFX_PARAM = "OtherSFXVolume";

    private float _bgmVolume = 1f;
    private float _sfxVolume = 1f;
    private float _otherSfxVolume = 1f;

    public static AudioManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            AudioListener.pause = true;
    }

    public void BgmPlay(string clipName)
    {
        foreach (var clip in bgmClips)
        {
            if (clip.name == clipName)
            {
                audioSources[0].clip = clip;
                audioSources[0].Play();
                return;
            }
        }
        Debug.Log($"{clipName} not found");
    }

    // ───── 볼륨 (0~1 슬라이더 → dB 변환) ─────
    public void BgmVolume(float value)
    {
        Debug.Log($"[BGM] slider value = {value}");
        _bgmVolume = value;
        SetVolume(BGM_PARAM, value);
    }

    public void SFXVolume(float value)
    {
        _sfxVolume = value;
        SetVolume(SFX_PARAM, value);
    }

    public void OtherSFXVolume(float value)
    {
        _otherSfxVolume = value;
        SetVolume(OTHER_SFX_PARAM, value);
    }

    private void SetVolume(string param, float value)
    {
        float dB = (value <= 0.0001f) ? -80f : Mathf.Log10(value) * 20f;
        mixer.SetFloat(param, dB);
        bool ok = mixer.SetFloat(param, dB);
        Debug.Log($"[SetVolume] {param} = {dB}dB, success={ok}");
    }

    // ───── 음소거 (토글 isOn: true=소리켜짐, false=음소거) ─────
    public void IsSoundMute(bool isOn, string target)
    {
        if (target == "BGM")
            ApplyMute(BGM_PARAM, isOn, _bgmVolume);
        else if (target == "SFX")
            ApplyMute(SFX_PARAM, isOn, _sfxVolume);
        else if (target == "OtherSFX")
            ApplyMute(OTHER_SFX_PARAM, isOn, _otherSfxVolume);
    }

    private void ApplyMute(string param, bool isOn, float restoreValue)
    {
        if (!isOn)
            mixer.SetFloat(param, -80f);
        else
            SetVolume(param, restoreValue);
    }

    // ───── SFX ─────
    public void SfxStop()
    {
        audioSources[1].Stop();
    }

    public void SfxPlay(string clipName)
    {
        foreach (var clip in sfxClips)
        {
            if (clip.name == clipName)
            {
                audioSources[1].PlayOneShot(clip);
                return;
            }
        }
        Debug.Log($"{clipName} not found");
    }
}