using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class DungeonCutsceneController : MonoBehaviour
{
    private float fogFadeDuration = 10f;
    public PlayableDirector timeline;
    private bool hasPlayed = false;

    public void OnFogFadeOut()
    {
        StartCoroutine(FogFadeOut());
    }

    public void OnCutsceneStart()
    {
        if (GameManager.LocalPlayer == null)
        {
            Debug.LogError("LocalPlayer가 null입니다!");
            return;
        }
        
        PlayerInput playerInput = GameManager.LocalPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;
        RenderSettings.fogDensity = 0.1f;
        Debug.Log("이거 작동이 안되는건가?");
        Debug.Log(RenderSettings.fogDensity);
    }

    public void OnCutsceneEnd()
    {
        PlayerInput playerInput = GameManager.LocalPlayer.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = true;
    }

    private IEnumerator FogFadeOut()
    {
        float startDensity = RenderSettings.fogDensity;
        float elapsed = 0f;

        while (elapsed < fogFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fogFadeDuration;
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, 0f, t);
            yield return null;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            // hasPlayed = true; // 한 번만 재생
            timeline.Play();
        }
    }
}