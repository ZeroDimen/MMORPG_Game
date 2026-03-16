using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

[Serializable]
public struct Party
{
    public string _title;
    public string _manager;
    public List<string> _member;

    public Party(string title, string name)
    {
        _title = title;
        _manager = name;
        _member = new List<string>();
        _member.Add(_manager);
    }

    public bool CanParticipation(string name)
    {
        if (_member.Count < 4)
        {
            _member.Add(name);
            return true;
        }

        return false;
    }
    
    // public bool SecedeMember(string name)
    // {
    //     
    // }
}
[Serializable]
public class PartyDataWrapper
{
    public List<Party> partyList;
}

public partial class PartySystem : MonoBehaviour
{
    public static PartySystem instance;
    public List<Party> partyList = new List<Party>();
    [SerializeField] private GameObject partyPrefab;

    private List<PartyListView> currentViewParty;
    [SerializeField] private Transform partyParentsTransform;

    private PhotonView pv;
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
        currentViewParty = new List<PartyListView>();
    }
}
