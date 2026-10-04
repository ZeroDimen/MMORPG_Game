using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyTrackerListView : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image managerImage;

    private string partyMember;
    public void ViewMember(string member)
    {
        nickName.text = member;
        partyMember = member;

        ViewManagerImage();
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (partyMember != targetPlayer.NickName) return;
        var maxHp = (int)targetPlayer.CustomProperties["MaxHp"];
        var hp = (int)targetPlayer.CustomProperties["Hp"];

        hpBar.fillAmount = (float)hp / (float)maxHp;
    }

    private void ViewManagerImage()
    {
        if (PartySystem.instance.MyParty != null && PartySystem.instance.MyParty._manager == partyMember)
            managerImage.gameObject.SetActive(true);
        else
            managerImage.gameObject.SetActive(false);
    }
}
