using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public struct Party
{
    public string _title;
    public string _manager;
    public List<string> _member;

    public Party(string title, string name)
    {
        _title = title;
        _manager = name;
        _member = new List<string>();
        _member.Add(_manager);
    }

    public bool CanParticipation(string name)
    {
        if (_manager.Length < 4)
        {
            _member.Add(name);
            return true;
        }

        return false;
    }

    // public bool SecedeMember(string name)
    // {
    //     
    // }
}

public class PartySystem : MonoBehaviour
{
    public static PartySystem instance;
    private List<Party> partyList = new List<Party>();
    [SerializeField] private GameObject partyPrefab;

    private List<PartyListView> currentViewParty;
    [SerializeField] private Transform partyParentsTransform;

    private PhotonView pv;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void CreateParty(string title, string managerName)
    {
        pv.RPC(nameof(CreatePartyButton), RpcTarget.MasterClient, title, managerName);
    }

    [PunRPC]
    public void CreatePartyButton(string title, string managerName)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Party party = new Party(title, managerName);
        partyList.Add(party);
        pv.RPC(nameof(UpdateListUI), RpcTarget.All);
    }

    [PunRPC]
    public void UpdateListUI()
    {
        foreach (var party in currentViewParty)
            Destroy(party);
        currentViewParty.Clear();
        
        foreach (var party in partyList)
        {
            var obj = Instantiate(partyPrefab, partyParentsTransform);
            var partyListView = obj.GetComponent<PartyListView>();
            partyListView.ViewParty(party);
            currentViewParty.Add(partyListView);
        }
    }
    
    public void Participation(string managerName, string playerName)
    {
        if (!pv.IsMine) return;
        pv.RPC(nameof(ParticipationButton), RpcTarget.MasterClient, managerName, playerName);
    }

    [PunRPC]
    public void ParticipationButton(string managerName, string playerName, PhotonMessageInfo info)
    {
        var party = partyList.Find(i => i._manager == managerName);
        var isParticipation = party.CanParticipation(playerName);

        if (isParticipation)
            pv.RPC(nameof(SuccessParticipation), info.Sender, playerName);
        else
            pv.RPC(nameof(Failure), info.Sender, playerName);

    }

    [PunRPC]
    public void SuccessParticipation()
    {
        UpdateListUI();
    }

    [PunRPC]
    public void Failure()
    {
        // 실패했다고 창 띄우기
    }

    public void Secede(string playerName)
    {
        pv.RPC(nameof(ApplySecede), RpcTarget.MasterClient, playerName);
    }

    [PunRPC]
    public void ApplySecede(string playerName)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
    }
}
