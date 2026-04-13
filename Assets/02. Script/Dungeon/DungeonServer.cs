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
        
        if(!dungeonPartyList.Contains(party))
            dungeonPartyList.Add(party);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            foreach (var member in party._member.Where(member => player.NickName == member))
                pv.RPC(nameof(UpdatePanelUI), player, party.acceptMember, party._member.Count);
        }
    }

    [PunRPC]
    public void RequestAccept(string playerName)
    {
        Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(playerName));
        party.acceptMember++;

        if (party.acceptMember == party._member.Count)
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                foreach (var member in party._member.Where(member => player.NickName == member))
                {
                    pv.RPC(nameof(TeleportPlayer), player);
                    pv.RPC(nameof(OffPanel), player);
                    Debug.Log(player.NickName);
                }
            }

            return;
        }
        
        PartySystem.instance.ServerToClientPartyList();
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            foreach (var member in party._member.Where(member => player.NickName == member))
                pv.RPC(nameof(UpdatePanelUI), player, party.acceptMember, party._member.Count);
        }
    }

    [PunRPC]
    public void RequestCancel(string playerName)
    {
        Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(playerName));
        party.acceptMember = 0;
        dungeonPartyList.Remove(party);
        PartySystem.instance.ServerToClientPartyList();
        var message = "파티원 중 한명이 취소 하였습니다.";
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            foreach (var member in party._member.Where(member => player.NickName == member))
                pv.RPC(nameof(OnCancel), player, message);
        }
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
        
        foreach (var player in PhotonNetwork.PlayerList)
        {
            foreach (var member in party._member.Where(member => player.NickName == member))
                pv.RPC(nameof(UpdatePanelUI), player, party.acceptMember, party._member.Count);
        }
    }
}
