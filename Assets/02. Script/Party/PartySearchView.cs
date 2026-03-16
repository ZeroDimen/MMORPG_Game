using System;
using UnityEngine;
using UnityEngine.UI;

public class PartySearchView : MonoBehaviour
{
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button createPartyButton;

    [SerializeField] private GameObject createPartyPanel;

    private void Start()
    {
        refreshButton.onClick.AddListener(OnRefresh);
        exitButton.onClick.AddListener(ExitButton);
        createPartyButton.onClick.AddListener(CreatePartyButton);
    }

    private void OnRefresh()
    {
        PartySystem.instance.RequestPartyListData();
    }

    private void ExitButton()
    {
        gameObject.SetActive(false);
    }

    private void CreatePartyButton()
    {
        createPartyPanel.SetActive(true);
    }
}
