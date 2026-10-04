using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartySearchView : MonoBehaviour
{
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button createPartyButton;
    [SerializeField] private TextMeshProUGUI createButtonText;

    [SerializeField] private GameObject createPartyPanel;
    [SerializeField] private GameObject modifyPanel;

    private void Start()
    {
        refreshButton.onClick.AddListener(OnRefresh);
        exitButton.onClick.AddListener(ExitButton);
        createPartyButton.onClick.AddListener(CreatePartyButton);
        PartySystem.instance.PartyMemberChanged += UpdateButtonText;
    }

    private void UpdateButtonText()
    {
        if (PartySystem.instance.MyParty == null)
        {
            createButtonText.text = "파티 생성";
            return;
        }
        createButtonText.text = PartySystem.instance.MyParty._manager == PhotonNetwork.NickName ? "파티 수정" : "파티 생성";
    }

    private void OnRefresh()
    {
        PartySystem.instance.RequestPartyListData();
    }

    private void ExitButton()
    {
        gameObject.SetActive(false);
    }

    private void CreatePartyButton()
    {
        if (PartySystem.instance.MyParty == null)
            createPartyPanel.SetActive(true);
        else if(PartySystem.instance.MyParty._manager == PhotonNetwork.NickName)
            modifyPanel.SetActive(true);
    }
}
