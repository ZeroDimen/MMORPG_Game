using System;
using Photon.Pun;
using UnityEngine;

public class DungeonPortal : MonoBehaviour
{
    private bool isPlayerNearby;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && GameManager.Instance.GameState == Constants.EGameState.Play)
            PortalInteraction();
    }

    private void PortalInteraction()
    {
        DungeonSystem.instance.OnDungeonPanel();
        isPlayerNearby = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView targetPV = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && targetPV.IsMine)
            isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView targetPV = other.GetComponent<PhotonView>();
        if (other.CompareTag("Player") && targetPV.IsMine)
            isPlayerNearby = false;
    }
}
