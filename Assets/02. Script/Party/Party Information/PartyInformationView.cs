using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PartyInformationView : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button secedeButton;
    [SerializeField] private GameObject memberPrefab;
    [SerializeField] private Transform partyListParentsTransform;

    [SerializeField] private GameObject memberStatusPanel;

    private List<GameObject> currentPartyMember;
    private void Awake()
    {
        confirmButton.onClick.AddListener(ConfirmButton);
        secedeButton.onClick.AddListener(SecedeButton);

        currentPartyMember = new List<GameObject>();
    }

    private void OnEnable()
    {
        UpdateMemberUI();
    }

    private void Start()
    {
        PartySystem.instance.PartyMemberChanged += UpdateMemberUI;
    }

    private void ConfirmButton()
    {
        gameObject.SetActive(false);
    }

    private void SecedeButton()
    {
        if (PartySystem.instance.MyParty == null) return;
        PartySystem.instance.RequestSecede(PhotonNetwork.LocalPlayer.NickName);
        gameObject.SetActive(false);
    }

    private void UpdateMemberUI()
    {
        foreach (var member in currentPartyMember)
            Destroy(member);
        currentPartyMember.Clear();
        
        var party = PartySystem.instance.MyParty;
        if (party == null) return;
        foreach (var member in party._member)
        {
            var obj = Instantiate(memberPrefab, partyListParentsTransform);
            var listView = obj.GetComponent<PartyInformationListView>(); 
            listView.MemberView(member);
            listView.memberStatusPanel = memberStatusPanel;
            currentPartyMember.Add(obj);
        }
    }
}
