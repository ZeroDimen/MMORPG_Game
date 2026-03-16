using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine;

public partial class PartySystem
{
    // Server 파티 리스트에 추가
    [PunRPC]
    public void CreatePartyButton(string title, string managerName, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Party party = new Party(title, managerName);
        var partyData = JsonConvert.SerializeObject(party);
        partyList.Add(party);
        pv.RPC(nameof(SuccessParticipation), info.Sender, partyData);
        ServerToClientPartyList();
    }
    // Server에 있는 파티 리스트를 각 클라이언트로 전송
    [PunRPC]
    public void ServerToClientPartyList()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var data = JsonConvert.SerializeObject(partyList);
        pv.RPC(nameof(UpdateListUI), RpcTarget.All, data);
    }

    [PunRPC]
    public void ParticipationButton(string managerName, string playerName, PhotonMessageInfo info)
    {
        var party = partyList.Find(i => i._manager == managerName);
        var partyData = JsonConvert.SerializeObject(party);
        var isParticipation = party.CanParticipation(playerName);

        if (isParticipation)
        {
            pv.RPC(nameof(SuccessParticipation), info.Sender, partyData);
            ServerToClientPartyList();
        }
        else
        {
            pv.RPC(nameof(Failure), info.Sender);
        }
    }
}
