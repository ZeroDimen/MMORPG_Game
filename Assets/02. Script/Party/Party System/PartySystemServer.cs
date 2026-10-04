using System.Linq;
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
        pv.RPC(nameof(UpdateListUI), RpcTarget.Others, data);
    }

    [PunRPC]
    public void RequestParticipation(string managerName, string playerName, PhotonMessageInfo info)
    {
        var party = partyList.Find(i => i._manager == managerName);
        if (party == null)
        {
            var message = "방을 찾을 수 없습니다.";
            pv.RPC(nameof(Failure), info.Sender, message);
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
            party._member.Add(playerName);
            var partyData = JsonConvert.SerializeObject(party);
            pv.RPC(nameof(SuccessParticipation), info.Sender, partyData);
            DungeonSystem.instance.RequestUpdateUI(party);
            ServerToClientPartyList();
        }
        else if(party._joinType == JoinType.Request)
        {
            pv.RPC(nameof(RequestParticipationToManager), RpcTarget.Others, managerName, playerName);
        }
    }

    [PunRPC]
    public void AnswerParticipationFromManager(string managerName, string playerName, bool answer, string message, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        var party = partyList.Find(i => i._manager == managerName);
        if (party == null) return;
        
        Player applicant = null;
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.NickName == playerName && p.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                applicant = p;
                break;
            }
        }

        if (applicant == null)
        {
            var m = "신청자를 찾을 수 없습니다.";
            pv.RPC(nameof(ShowMessage), info.Sender, m);
            return;
        }

        if (partyList.Any(p => p.IsMyParty(applicant.NickName)))
        {
            var m = "상대방은 이미 파티가 있습니다.";
            pv.RPC(nameof(Failure), info.Sender, m);
            return;
        }

        if (answer == false)
        {
            pv.RPC(nameof(Failure), applicant, message);
            return;
        }

        if (!party.CanParticipation(playerName)) return;
        
        party._member.Add(playerName);
        var partyData = JsonConvert.SerializeObject(party);
        pv.RPC(nameof(SuccessParticipation), info.Sender, partyData);
        pv.RPC(nameof(SuccessParticipation), applicant, partyData);
        DungeonSystem.instance.RequestUpdateUI(party);
        
        ServerToClientPartyList();
    }
    
    [PunRPC]
    public void Secede(string playerName, string data, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var partyData = JsonConvert.DeserializeObject<Party>(data);
        var party = partyList.Find(i => i._manager == partyData._manager);

        if (party != null)
        {
            if (party._member.Count <= 1)
                partyList.Remove(party);
            else
            {
                if (party._manager == playerName)
                {
                    var nextManager = party._member.FirstOrDefault(m => m != playerName);
                    if (nextManager != null) party._manager = nextManager;
                }
                party._member.Remove(playerName);
                DungeonSystem.instance.RequestUpdateUI(party);
            }
        }
        pv.RPC(nameof(SuccessSecede), info.Sender);
        DungeonSystem.instance.RequestCancel(party, playerName);
    }

    [PunRPC]
    public void RequestModifyParty(string data)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var partyData = JsonConvert.DeserializeObject<Party>(data);
        var index = partyList.FindIndex(i => i._manager == partyData._manager);
        if (index != -1)
        {
            partyList[index] = partyData;
            ServerToClientPartyList();
        }
    }

    [PunRPC]
    public void RequestDelegateManager(string data, string playerName)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var partyData = JsonConvert.DeserializeObject<Party>(data);
        var party = partyList.Find(i => i._manager == partyData._manager);
        if (party != null)
        {
            party._manager = playerName;
            ServerToClientPartyList();
        }
    }
}
