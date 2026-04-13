using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;

public partial class PartySystem
{
    // Server 에게 파티 생성 요청
    public void RequestCreateParty(string title, string managerName, JoinType type)
    {
        pv.RPC(nameof(CreatePartyButton), RpcTarget.MasterClient, title, managerName, (int)type);
    }

    // Server에게 파티 리스트 요청
    public void RequestPartyListData()
    {
        pv.RPC(nameof(ServerToClientPartyList), RpcTarget.MasterClient);
    }
    // Server에게 받은 데이터로 화면 갱신
    [PunRPC]
    public void UpdateListUI(string data)
    {
        partyList = JsonConvert.DeserializeObject<List<Party>>(data);
        
        foreach (var party in currentViewParty)
            Destroy(party.gameObject);
        currentViewParty.Clear();
        
        foreach (var party in partyList)
        {
            if (party.IsMyParty(PhotonNetwork.NickName))
                MyParty = party;
            var obj = Instantiate(partyPrefab, partyParentsTransform);
            var partyListView = obj.GetComponent<PartyListView>();
            partyListView.ViewParty(party);
            currentViewParty.Add(partyListView);
        }
        PartyMemberChanged?.Invoke();
    }

    public void Participation(string managerName, string playerName)
    {
        pv.RPC(nameof(RequestParticipation), RpcTarget.MasterClient, managerName, playerName);
    }
    
    
    [PunRPC]
    public void SuccessParticipation(string data)
    {
        // 성공
        var party = JsonConvert.DeserializeObject<Party>(data);
        MyParty = party;
        Debug.Log("파티 가입 성공~");
    }

    [PunRPC]
    public void Failure(string message)
    {
        ShowMessage(message);
    }

    public void RequestSecede(string playerName)
    {
        var myPartyData = JsonConvert.SerializeObject(MyParty);
        pv.RPC(nameof(Secede), RpcTarget.MasterClient, playerName, myPartyData);
    }

    [PunRPC]
    public void SuccessSecede()
    {
        MyParty = null;
        RequestPartyListData();
    }

    [PunRPC]
    public void RequestParticipationToManager(string managerName, string playerName)
    {
        if (managerName != PhotonNetwork.NickName) return;
        OnRequestPartySignUp?.Invoke(playerName, managerName);
    }

    public void AnswerParticipationToServer(string managerName, string playerName, bool answer, string message)
    {
        pv.RPC(nameof(AnswerParticipationFromManager), RpcTarget.MasterClient, managerName, playerName, answer, message);
    }

    [PunRPC]
    public void ShowMessage(string context)
    {
        GameObject panel = Instantiate(messagePrefab, transform);
        var cs = panel.GetComponent<MessageView>();
        cs.ViewText(context);
    }

    public void ModifySetting()
    {
        var myPartyData = JsonConvert.SerializeObject(MyParty);
        pv.RPC(nameof(RequestModifyParty), RpcTarget.MasterClient, myPartyData);
    }

    public void DelegateManager(string playerName)
    {
        var myPartyData = JsonConvert.SerializeObject(MyParty);
        pv.RPC(nameof(RequestDelegateManager), RpcTarget.MasterClient, myPartyData, playerName);
    }
}
