using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public CanvasGroup fadePanel;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경 시 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    public IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadePanel.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            fadePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        fadePanel.alpha = targetAlpha;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main")
        {
            PhotonNetwork.IsMessageQueueRunning = true; // Scene 변경시 네트워크 메시지 일시정지 해제
            StartCoroutine(FadeOut());
        }
        else
        {
            Debug.Log("씬 로드 성공: " + scene.name);
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(Fade(0));
    }
}