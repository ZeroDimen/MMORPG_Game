using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public partial class PartySystem
{
    // Server 파티 리스트에 추가
    [PunRPC]
    public void CreatePartyButton(string title, string managerName, int type, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Party party = new Party(title, managerName, (JoinType)type);
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
    public void RequestParticipation(string managerName, string playerName, PhotonMessageInfo info)
    {
        Debug.Log("파티 신청 요청 받음");
        var party = partyList.Find(i => i._manager == managerName);
        if (party == null)
        {
            pv.RPC(nameof(Failure), info.Sender);
            return;
        }
        
        var isParticipation = party.CanParticipation(playerName);
        if (!isParticipation)
        {
            pv.RPC(nameof(Failure), info.Sender);
            return;
        }
        
        if (party._joinType == JoinType.Instant)
        {
            Debug.Log("즉시 참가임");
            party._member.Add(playerName);
            var partyData = JsonConvert.SerializeObject(party);
            pv.RPC(nameof(SuccessParticipation), info.Sender, partyData);
            ServerToClientPartyList();
        }
        else if(party._joinType == JoinType.Request)
        {
            Debug.Log("서버에서 방장에게 신청 보냄");
            pv.RPC(nameof(RequestParticipationToManager), RpcTarget.Others, managerName, playerName);
        }
    }

    [PunRPC]
    public void AnswerParticipationFromManager(string managerName, string playerName, bool answer, PhotonMessageInfo info)
    {
        Debug.Log("방장의 답변을 서버에서 받음");
        var party = partyList.Find(i => i._manager == managerName);
        if (party == null) return;
        
        var isParticipation = party.CanParticipation(playerName);
        if (!isParticipation) return;

        Player applicant = null;
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.NickName == playerName)
            {
                applicant = p;
                break;
            }
        }
        
        if (answer == false)
        {
            pv.RPC(nameof(Failure), applicant);
            return;
        }
        
        party._member.Add(playerName);
        var partyData = JsonConvert.SerializeObject(party);
        pv.RPC(nameof(SuccessParticipation), info.Sender, partyData);
        pv.RPC(nameof(SuccessParticipation), applicant, partyData);
        
        ServerToClientPartyList();
    }
    
    [PunRPC]
    public void Secede(string playerName, string data, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var partyData = JsonConvert.DeserializeObject<Party>(data);
        var party = partyList.Find(i => i._manager == partyData._manager);

        party?._member.Remove(playerName);
        pv.RPC(nameof(SuccessSecede), info.Sender);
    }
}
