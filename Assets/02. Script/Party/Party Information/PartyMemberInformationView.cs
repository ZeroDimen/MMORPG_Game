using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberInformationView : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI lvText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI dexText;

    private void Start()
    {
        exitButton.onClick.AddListener(OnExitButton);
    }

    private void OnExitButton()
    {
        // atkText.text = "";
        // lvText.text = "";
        // defText.text = "";
        // dexText.text = "";
        gameObject.SetActive(false);
    }

    public void UpdateUI(string playerName)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.NickName == playerName)
            {
                var atk = (int)player.CustomProperties["ATK"];
                var lv = (int)player.CustomProperties["LV"];
                var def = (int)player.CustomProperties["DEF"];
                var dex = (int)player.CustomProperties["DEX"];
                
                atkText.text = $"ATK : {atk.ToString()}";
                lvText.text = $"LV : {lv.ToString()}";
                defText.text = $"DEF : {def.ToString()}";
                dexText.text = $"DEX : {dex.ToString()}";
            }
        }
    }
}
