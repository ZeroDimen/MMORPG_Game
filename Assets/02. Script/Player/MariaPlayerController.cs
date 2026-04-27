using System;
using Photon.Pun;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class MariaPlayerController :PlayerController
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Transform Maria_Head;
    
    private Collider _attackCollider;
    private MariaDamageManager _mariaDamageManager;

    protected override void Start()
    {
        base.Start();
        if (photonView.IsMine 
            && !PhotonNetwork.IsMasterClient
            && GameObject.Find("PlayerCam").GetComponent<CinemachineCamera>().Follow == null)
        {
            playerName.text = PhotonNetwork.NickName;
            playerName.color = Color.green;
            GameManager.LocalPlayer = gameObject;

            var vCamObj = GameObject.FindWithTag("PlayerCam");
            if (vCamObj != null)
            {
                var vCam = vCamObj.GetComponent<CinemachineCamera>();
                vCam.Follow = Maria_Head;
                vCam.LookAt = Maria_Head;
            }
            // GameObject.Find("PlayerCam").GetComponent<CinemachineCamera>().Follow = Maria_Head;
            PlayerStatusView.Instance.player = this;
            StartCoroutine(PlayerStatusView.Instance.UpdateStatusUIRoutine());
        }
        else
        {
            playerName.text = photonView.Owner.NickName;
            playerName.color = Color.red;
        }
    }
}