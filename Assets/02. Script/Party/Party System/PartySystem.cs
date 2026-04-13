using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

[Serializable]
public enum JoinType
{
    Instant,
    Request
}

[Serializable]
public class Party
{
    public string _title;
    public string _manager;
    public List<string> _member;
    public JoinType _joinType;
    public int acceptMember;

    public Party(string title, string name, JoinType joinType)
    {
        _title = title;
        _manager = name;
        _member = new List<string> { _manager };
        _joinType = joinType;
        acceptMember = 0;
    }

    public bool CanParticipation(string name)
    {
        if (_member.Count >= 4) return false;
        return true;
    }

    public bool IsMyParty(string name)
    {
        return _member.Contains(name);
    }
}

public partial class PartySystem : MonoBehaviourPunCallbacks
{
    public static PartySystem instance;
    public List<Party> partyList = new List<Party>();
    [SerializeField] private GameObject partyPrefab;
    [SerializeField] private GameObject messagePrefab;

    private List<PartyListView> currentViewParty;
    [SerializeField] private Transform partyParentsTransform;

    private PhotonView pv;
    public Party MyParty { private set; get; }
    public event Action PartyMemberChanged;
    public event Action<string, string> OnRequestPartySignUp;
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
        OnRequestPartySignUp += GetComponentInChildren<PartySignUpPanelView>(true).UpdateUI;
    }
}
