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
        PartySystem.instance.partyMemberChanged += UpdateMemberUI;
    }

    private void ConfirmButton()
    {
        gameObject.SetActive(false);
    }

    private void SecedeButton()
    {
        PartySystem.instance.Secede(PhotonNetwork.LocalPlayer.NickName);
    }

    private void UpdateMemberUI()
    {
        foreach (var member in currentPartyMember)
            Destroy(member);
        currentPartyMember.Clear();
        
        var party = PartySystem.instance.MyParty;
        foreach (var member in party._member)
        {
            var obj = Instantiate(memberPrefab, partyListParentsTransform);
            obj.GetComponent<PartyInformationListView>().MemberView(member);
            currentPartyMember.Add(obj);
        }
    }
}
