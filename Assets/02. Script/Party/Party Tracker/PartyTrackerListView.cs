using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyTrackerListView : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private Image hpBar;

    private string partyMember;
    public void ViewMember(string member)
    {
        nickName.text = member;
        partyMember = member;
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (partyMember != targetPlayer.NickName) return;
        var maxHp = (int)targetPlayer.CustomProperties["MaxHp"];
        var hp = (int)targetPlayer.CustomProperties["Hp"];

        hpBar.fillAmount = (float)hp / (float)maxHp;
    }
}
