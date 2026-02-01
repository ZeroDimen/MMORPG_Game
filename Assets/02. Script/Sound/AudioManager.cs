using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;
    
    [SerializeField] private AudioSource[] audioSources;
    
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

    public void SfxStop()
    {
        audioSources[1].Stop();
    }
    
    public void SfxPlay(string clipName, bool islong = false) // 효과음을 출력하는 함수
    {
        foreach (var clip in sfxClips)
        {
            if (clip.name == clipName)
            {
                if (!islong)
                {
                    audioSources[1].PlayOneShot(clip);
                    return;
                }
                else
                {
                    audioSources[1].clip = clip;
                    audioSources[1].Play();
                    return;
                }
            }
        }

        Debug.Log($"{clipName} not found");
    }
}
