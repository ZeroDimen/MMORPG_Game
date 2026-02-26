using System;
using UnityEngine;

public class CursorInteraction : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.SetGameState(Constants.EGameState.Interaction);
    }

    private void OnDisable()
    {
        GameManager.Instance.SetGameState(Constants.EGameState.Play);
    }

    private void Update()
    {
        if(GameManager.Instance.GameState != Constants.EGameState.Interaction)
            GameManager.Instance.SetGameState(Constants.EGameState.Interaction);
    }
}
