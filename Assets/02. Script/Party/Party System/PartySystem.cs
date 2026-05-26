using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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
