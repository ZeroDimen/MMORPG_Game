using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PartyCreateView : MonoBehaviour
{
    [SerializeField] private InputField title;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private PhotonView pv;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmButton);
        cancelButton.onClick.AddListener(OnCancelButton);
        pv = GetComponent<PhotonView>();
    }

    private void OnConfirmButton()
    {
        if (!pv.IsMine) return;
        PartySystem.instance.CreateParty(title.text, PhotonNetwork.LocalPlayer.NickName);
    }

    private void OnCancelButton()
    {
        title.text = "";
        gameObject.SetActive(false);
    }
}
