using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panel")]
    public GameObject QuestPanel;
    public GameObject InventoryPanel;
    public GameObject PartySearchPanel;
    public GameObject MenuPanel;

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
}
