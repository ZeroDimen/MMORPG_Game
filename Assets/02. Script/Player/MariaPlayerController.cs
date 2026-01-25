using Photon.Pun;
using TMPro;
using UnityEngine;

public class MariaPlayerController :PlayerController
{
    [SerializeField] private TMP_Text playerName;

    protected override void Start()
    {
        base.Start();
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
}