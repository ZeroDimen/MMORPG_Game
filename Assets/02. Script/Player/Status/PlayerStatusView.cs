using System;
using TMPro;
using UnityEngine;

public class PlayerStatusView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ATK;
    [SerializeField] private TextMeshProUGUI DEF;
    [SerializeField] private TextMeshProUGUI DEX;

    public PlayerController player;
    public static PlayerStatusView Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        GameEvents.OnStatusChanged += UpdateStatusUI;
    }

    private void OnEnable()
    {
        if(player != null)
            UpdateStatusUI(player.Status);
    }

    private void UpdateStatusUI(PlayerStatus status)
    {
        ATK.text = "ATK : " + status.ATK;
        DEF.text = "DEF : " + status.DEF;
        DEX.text = "DEX : " + status.DEX;
    }
}
