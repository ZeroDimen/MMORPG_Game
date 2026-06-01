using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class DungeonSystem : MonoBehaviourPunCallbacks
{
    public static DungeonSystem instance;
    private PhotonView pv;
    private List<Party> dungeonPartyList;
    [SerializeField] private DungeonLight dungeonLight;
    [SerializeField] private Image fadePanel;

    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private GameObject dungeonPanel;

    [SerializeField] private GameObject elevator;
    public const int MonsterNum = 3;
    private bool _hasPlayed = false;
    private Dictionary<string, int> partyKillCount;
    [SerializeField] private Transform fieldSpawnPos;
    [SerializeField] private GameObject exitDungeonButton;
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
        dungeonPartyList = new List<Party>();
        partyKillCount = new Dictionary<string, int>();
    }

    private void SendRpcToPartyMembers(Party party, string rpcName, params object[] parameters)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            foreach (var member in party._member.Where(member => player.NickName == member))
                pv.RPC(rpcName, player, parameters);
        }
    }
}
