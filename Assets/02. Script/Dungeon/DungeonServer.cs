using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public partial class DungeonSystem
{
    [PunRPC]
    public void RequestPanel(string playerName, PhotonMessageInfo info)
    {
        Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(playerName));

        if (party == null)
        {
            string message = "파티가 없습니다. 파티 가입 후 시도해 주십시오.";
            pv.RPC(nameof(OnMessagePanel), info.Sender, message);
            return;
        }

        if (party._manager != playerName)
        {
            string message = "파티장이 아닙니다. 이 동작은 파티장만 가능합니다.";
            pv.RPC(nameof(OnMessagePanel), info.Sender, message);
            return;
        }

        if (!dungeonPartyList.Contains(party))
            dungeonPartyList.Add(party);

        SendRpcToPartyMembers(party, nameof(UpdatePanelUI), party.acceptMember, party._member.Count);
    }

    [PunRPC]
    public void RequestAccept(string playerName)
    {
        Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(playerName));
        party.acceptMember++;

        if (party.acceptMember == party._member.Count)
        {
            SendRpcToPartyMembers(party, nameof(TeleportPlayer));
            SendRpcToPartyMembers(party, nameof(OffPanel));

            for (int i = 0; i < MonsterNum; i++)
            {
                if (_hasPlayed == false)
                    GameManager.Instance.SpawnMonsterInDungeon(2, party._manager);
            }
            _hasPlayed = true;
            return;
        }

        PartySystem.instance.ServerToClientPartyList();

        SendRpcToPartyMembers(party, nameof(UpdatePanelUI), party.acceptMember, party._member.Count);
    }

    [PunRPC]
    public void RequestCancel(string playerName)
    {
        Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(playerName));
        party.acceptMember = 0;
        dungeonPartyList.Remove(party);
        PartySystem.instance.ServerToClientPartyList();
        var message = "파티원 중 한명이 취소 하였습니다.";

        SendRpcToPartyMembers(party, nameof(OnCancel), message);
    }

    public void RequestCancel(Party party, string playerName)
    {
        if (!dungeonPartyList.Contains(party)) return;

        party.acceptMember = 0;
        dungeonPartyList.Remove(party);
        PartySystem.instance.ServerToClientPartyList();
        var message = "파티원 중 한명이 탈퇴하였습니다.";

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (playerName == player.NickName)
            {
                pv.RPC(nameof(OffPanel), player);
                continue;
            }
            foreach (var member in party._member.Where(member => player.NickName == member))
                pv.RPC(nameof(OnCancel), player, message);
        }
    }

    public void RequestUpdateUI(Party party)
    {
        if (!dungeonPartyList.Contains(party)) return;

        SendRpcToPartyMembers(party, nameof(UpdatePanelUI), party.acceptMember, party._member.Count);
    }

    public void KillMonster(string partyId)
    {
        if (!partyKillCount.ContainsKey(partyId))
            partyKillCount[partyId] = 0;

        partyKillCount[partyId]++;

        if (partyKillCount[partyId] >= MonsterNum)
        {
            Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(partyId));
            SendRpcToPartyMembers(party, nameof(OnElevator));
        }
    }

    public void RequestTimeline(string playerName)
    {
        Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(playerName));
        if (!PhotonNetwork.IsMasterClient) return;

        SendRpcToPartyMembers(party, nameof(PlayTimeline));
    }
}
