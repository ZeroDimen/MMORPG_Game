using System;
using UnityEngine;

public class CursorInteraction : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.PushState(Constants.EGameState.Interaction);
    }

    private void OnDisable()
    {
        GameManager.Instance.PopState(Constants.EGameState.Interaction);
    }
}
