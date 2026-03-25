using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartySignUpPanelView : MonoBehaviour
{
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button RefusalButton;
    [SerializeField] private TextMeshProUGUI nickName;

    private void Start()
    {
        acceptButton.onClick.AddListener(OnAcceptButton);
        RefusalButton.onClick.AddListener(OnRefusalButton);
    }

    public void UpdateUI(string playerName)
    {
        nickName.text = playerName;
    }

    private void OnAcceptButton()
    {
        
    }

    private void OnRefusalButton()
    {
        
    }
}
