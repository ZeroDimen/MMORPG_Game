using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PartySignUpPanelView : MonoBehaviour
{
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button refusalButton;
    [SerializeField] private Button informationButton;
    [SerializeField] private TextMeshProUGUI nickName;

    [SerializeField] private GameObject statusPanel;

    private string _playerName;
    private string _managerName;
    private void Start()
    {
        acceptButton.onClick.AddListener(OnAcceptButton);
        refusalButton.onClick.AddListener(OnRefusalButton);
    }

    public void UpdateUI(string playerName, string managerName)
    {
        gameObject.SetActive(true);
        nickName.text = playerName;
        _playerName = playerName;
        _managerName = managerName;
        informationButton.onClick.AddListener(OnInformationButton);
    }

    private void OnInformationButton()
    {
        statusPanel.SetActive(true);
        var info = statusPanel.GetComponent<PartySignUpStatusView>();
        info.UpdateUI(_playerName);
    }

    private void OnAcceptButton()
    {
        PartySystem.instance.AnswerParticipationToServer(_managerName, _playerName, true);
        gameObject.SetActive(false);
    }

    private void OnRefusalButton()
    {
        PartySystem.instance.AnswerParticipationToServer(_managerName, _playerName, false);
        gameObject.SetActive(false);
    }
}
