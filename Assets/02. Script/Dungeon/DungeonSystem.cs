using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public partial class DungeonSystem : MonoBehaviourPunCallbacks
{
    public static DungeonSystem instance;
    private PhotonView pv;
    private List<Party> dungeonPartyList;

    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private GameObject dungeonPanel;

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
    }
}
