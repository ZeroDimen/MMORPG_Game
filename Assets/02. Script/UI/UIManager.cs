using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panel")]
    public GameObject QuestPanel;
    public GameObject InventoryPanel;
    public GameObject PartySearchPanel;
    public GameObject MenuPanel;
    public GameObject BossHpBar;
    public GameObject GameOverPanel;

    [SerializeField]
    public BossHpBar _bossHpBar;
    [SerializeField] private Button _respawnButton;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnQuestPanel()
    {
        QuestPanel.SetActive(!QuestPanel.activeSelf);
    }

    public void OnInventoryPanel()
    {
        InventoryPanel.SetActive(!InventoryPanel.activeSelf);
    }

    public void OnPartySearchPanel()
    {
        PartySearchPanel.SetActive(!PartySearchPanel.activeSelf);
    }

    public void OnMenuPanel()
    {
        MenuPanel.SetActive(!MenuPanel.activeSelf);
    }

    public void OnBossHpBar()
    {
        BossHpBar.SetActive(true);
        _bossHpBar.SetMaxHp();
    }
    
    public void OffBossHpBar()
    {
        BossHpBar.SetActive(false);
    }
    
    public void UpdateBossHpBar(float hp)
    {
        _bossHpBar.SetHp(hp);
    }

    public void OnGameOverPanel(Action action)
    {
        GameOverPanel.SetActive(true);
        _respawnButton.onClick.AddListener(() =>
        {
            GameOverPanel.SetActive(false);
            action?.Invoke();
        });
    }
}
