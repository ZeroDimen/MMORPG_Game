using System;
using UnityEngine;

public class CursorInteraction : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.PushState(Constants.EGameState.Interaction);
        StartCoroutine(ForceCursor());
    }

    private void OnDisable()
    {
        GameManager.Instance.PopState(Constants.EGameState.Interaction);
    }
    
    private System.Collections.IEnumerator ForceCursor()
    {
        // 한 프레임 뒤에 커서를 확실히 리프레시
        yield return null;
        Cursor.lockState = CursorLockMode.Locked;  // 잠깐 잠갔다
        Cursor.lockState = CursorLockMode.None;    // 바로 풀기 → 에디터가 커서 재인식
        Cursor.visible = true;
    }
}
