using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class MariaPlayerController :PlayerController
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Collider attackCollider;

    private void Start()
    {
        if (photonView.IsMine)
        {
            playerName.text = PhotonNetwork.NickName;
            playerName.color = Color.green;
        }
        else
        {
            playerName.text = photonView.Owner.NickName;
            playerName.color = Color.red;
        }
    }

    public void EnableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
    }
    
    public void DisableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }
}