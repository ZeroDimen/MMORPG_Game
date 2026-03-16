using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine;

public partial class PartySystem
{
    // Server 파티 리스트에 추가
    [PunRPC]
    public void CreatePartyButton(string title, string managerName)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Party party = new Party(title, managerName);
        partyList.Add(party);
        ServerToClientPartyList();
    }
    // Server에 있는 파티 리스트를 각 클라이언트로 전송
    [PunRPC]
    public void ServerToClientPartyList()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        // var data = PartyListToJson();
        var data = JsonConvert.SerializeObject(partyList);
        pv.RPC(nameof(UpdateListUI), RpcTarget.All, data);
    }
    // 파티 리스트를 json으로 변환
    private string PartyListToJson()
    {
        PartyDataWrapper partyData = new PartyDataWrapper();
        partyData.partyList = partyList;

        var data = JsonUtility.ToJson(partyData, true);
        return data;
    }
    
    [PunRPC]
    public void ParticipationButton(string managerName, string playerName, PhotonMessageInfo info)
    {
        var party = partyList.Find(i => i._manager == managerName);
        var isParticipation = party.CanParticipation(playerName);

        if (isParticipation)
        {
            pv.RPC(nameof(SuccessParticipation), info.Sender);
            ServerToClientPartyList();
        }
        else
        {
            pv.RPC(nameof(Failure), info.Sender);
        }
    }
}
