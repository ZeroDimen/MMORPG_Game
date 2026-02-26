using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;
    
    public AudioSource[] audioSources;
    
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
    
    public void BgmPlay(string clipName) // 효과음을 출력하는 함수
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

    public void BgmVolume(float volume)
    {
        audioSources[0].volume = volume;
    }
    
    public void SFXVolume(float volume)
    {
        audioSources[1].volume = volume;
    }

    public void IsSoundMute(bool isMute, string target)
    {
        if (target == "BGM")
            audioSources[0].mute = !isMute;
        else if (target == "SFX")
            audioSources[1].mute = !isMute;
    }

    public void SfxStop()
    {
        audioSources[1].Stop();
    }
    
    public void SfxPlay(string clipName) // 효과음을 출력하는 함수
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
