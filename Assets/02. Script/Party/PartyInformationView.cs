using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PartyInformationView : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button secedeButton;

    [SerializeField] private GameObject memberPrefab;

    private void Start()
    {
        confirmButton.onClick.AddListener(ConfirmButton);
        secedeButton.onClick.AddListener(SecedeButton);
    }

    private void ConfirmButton()
    {
        gameObject.SetActive(false);
    }

    private void SecedeButton()
    {
        PartySystem.instance.Secede(PhotonNetwork.LocalPlayer.NickName);
    }

    private void ViewMember()
    {
        
    }
}
