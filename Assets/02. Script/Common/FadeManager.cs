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
    

    public IEnumerator Fade(float targetAlpha, float duration = 1f)
    {
        float startAlpha = fadePanel.alpha;
        float time = 0f;

        while (time < duration)
        {
            fadePanel.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
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
        else if (scene.name == "Intro")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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