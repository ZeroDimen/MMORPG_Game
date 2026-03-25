using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyTrackerView : MonoBehaviour
{
    [SerializeField] private GameObject memberPrefab;
    private List<GameObject> currentPartyMember;

    private void Start()
    {
        currentPartyMember = new List<GameObject>();
        PartySystem.instance.PartyMemberChanged += UpdateMemberUI;
    }

    private void UpdateMemberUI()
    {
        foreach(var member in currentPartyMember)
            Destroy(member);
        currentPartyMember.Clear();
        
        var party = PartySystem.instance.MyParty;
        if (party == null) return;
        foreach (var member in party._member)
        {
            var obj = Instantiate(memberPrefab, transform);
            obj.GetComponent<PartyTrackerListView>().ViewMember(member);
            currentPartyMember.Add(obj);
        }
    }
}
