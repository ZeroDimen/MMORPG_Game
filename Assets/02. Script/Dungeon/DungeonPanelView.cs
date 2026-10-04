using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonPanelView : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private TextMeshProUGUI readyMemberText;

    private void Start()
    {
        readyButton.onClick.AddListener(OnReadyButton);
        cancelButton.onClick.AddListener(OnCancelButton);
    }

    public void UpdateUI(int ready, int total)
    {
        readyMemberText.text = $"준비된 인원 : {ready} / {total}";
    }

    private void OnReadyButton()
    {
        DungeonSystem.instance.Accept();
    }

    private void OnCancelButton()
    {
        DungeonSystem.instance.Cancel();
    }
}
