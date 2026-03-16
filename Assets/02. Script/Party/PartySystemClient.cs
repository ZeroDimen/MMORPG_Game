using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine;

public partial class PartySystem
{
    // Server 에게 파티 생성 요청
    public void RequestCreateParty(string title, string managerName)
    {
        pv.RPC(nameof(CreatePartyButton), RpcTarget.MasterClient, title, managerName);
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
            if (MyParty != null && MyParty._manager == party._manager)
                MyParty = party;
            var obj = Instantiate(partyPrefab, partyParentsTransform);
            var partyListView = obj.GetComponent<PartyListView>();
            partyListView.ViewParty(party);
            currentViewParty.Add(partyListView);
        }
        partyMemberChanged?.Invoke();
    }

    public void Participation(string managerName, string playerName)
    {
        pv.RPC(nameof(ParticipationButton), RpcTarget.MasterClient, managerName, playerName);
    }
    
    
    [PunRPC]
    public void SuccessParticipation(string data)
    {
        // 성공
        var party = JsonConvert.DeserializeObject<Party>(data);
        MyParty = party;
    }

    [PunRPC]
    public void Failure()
    {
        // 실패했다고 창 띄우기
        Debug.Log("실패");
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
