using System;
using System.Collections;
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
    [SerializeField] private TextMeshProUGUI timer;

    [SerializeField] private GameObject statusPanel;

    private string _playerName;
    private string _managerName;

    private int limitTime = 15;
    private void Start()
    {
        acceptButton.onClick.AddListener(OnAcceptButton);
        refusalButton.onClick.AddListener(OnRefusalButton);
        StartCoroutine(Timer());
    }

    public void UpdateUI(string playerName, string managerName)
    {
        gameObject.SetActive(true);
        nickName.text = playerName;
        _playerName = playerName;
        _managerName = managerName;
        informationButton.onClick.AddListener(OnInformationButton);
    }

    private IEnumerator Timer()
    {
        while (limitTime > 0)
        {
            timer.text = $"{limitTime}초 남았습니다.";
            limitTime--;
            yield return new WaitForSeconds(1f);
        }
        OnRefusalButton();
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
