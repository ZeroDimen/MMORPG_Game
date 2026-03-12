using System;
using UnityEngine;
using UnityEngine.UI;

public class PartySearchView : MonoBehaviour
{
    [SerializeField] private Button refreshButton;

    private void Start()
    {
        refreshButton.onClick.AddListener(OnRefresh);
    }

    public void OnRefresh()
    {
        PartySystem.instance.UpdateListUI();
    }
}
