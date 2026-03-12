using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PartyListView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI numberOfpeple;
    [SerializeField] private Button participation;

    public string managerName;
    
    private void Start()
    {
        participation.onClick.AddListener(ParticipationButton);
    }

    public void ViewParty(Party party)
    {
        title.text = party._title;
        numberOfpeple.text = $"{party._member.Count} / 4";
        participation.onClick.AddListener(ParticipationButton);
        managerName = party._manager;
    }

    private void ParticipationButton()
    {
        PartySystem.instance.Participation(managerName, PhotonNetwork.LocalPlayer.NickName);
    }
}
