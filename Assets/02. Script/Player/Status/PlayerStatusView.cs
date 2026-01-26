using System;
using TMPro;
using UnityEngine;

public class PlayerStatusView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ATK;
    [SerializeField] private TextMeshProUGUI DEF;
    [SerializeField] private TextMeshProUGUI DEX;

    [SerializeField] private PlayerController player;

    private void Awake()
    {
        GameEvents.OnStatusChanged += UpdateStatusUI;
    }

    private void OnEnable()
    {
        UpdateStatusUI(player.Status);
    }

    private void UpdateStatusUI(PlayerStatus status)
    {
        ATK.text = "ATK : " + status.ATK;
        DEF.text = "DEF : " + status.DEF;
        DEX.text = "DEX : " + status.DEX;
    }
}
