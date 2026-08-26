using System;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmbushTrigger : MonoBehaviour
{
    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(1);
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>();
        if (pc == null || !pc.photonView.IsMine) return;
        Debug.Log(2);
        _triggered = true;
        DungeonSystem.instance.OnAmbush(PhotonNetwork.NickName);
    }
}
