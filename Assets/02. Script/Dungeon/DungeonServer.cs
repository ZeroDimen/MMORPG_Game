using System.Collections.Generic;
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
    
    public void RequestSpawnBoss(string playerName)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(playerName));
        if (party == null) return;

        // HashSet.Add는 이미 있으면 false 반환 → 한 줄로 파티별 중복 차단
        if (!bossSpawnedParties.Add(party._manager)) return;

        GameManager.Instance.SpawnBossInDungeon(party._manager);
        SendRpcToPartyMembers(party, nameof(OnBossSpawnEffect)); // 파티 전원에게 등장 연출 재생
    }

    // 보스 처치 시 마스터에서 호출 → 파티 전원에게 클리어 연출/자동 복귀 브로드캐스트
    public void OnBossDefeated(string partyId)
    {
        Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(partyId));
        if (party == null) return;

        SendRpcToPartyMembers(party, nameof(OnBossClear));
    }

    public void RequestTimeline(string playerName)
    {
        Party party = PartySystem.instance.partyList.Find(i => i.IsMyParty(playerName));
        if (!PhotonNetwork.IsMasterClient) return;

        SendRpcToPartyMembers(party, nameof(PlayTimeline));
    }
}
