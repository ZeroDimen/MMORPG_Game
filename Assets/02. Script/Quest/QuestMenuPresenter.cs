using System.Collections.Generic;
using UnityEngine;

public class QuestMenuPresenter : MonoBehaviour
{
    [SerializeField] private GameObject menuEntryPrefab;
    [SerializeField] private Transform contentParent;

    private List<GameObject> spawnedEntries = new List<GameObject>();

    private void OnEnable()
    {
        GameEvents.OnQuestListChanged += RefreshMenu;
        GameEvents.OnInteraction?.Invoke(true);
    }

    private void OnDisable()
    {
        GameEvents.OnQuestListChanged -= RefreshMenu;
        GameEvents.OnInteraction?.Invoke(false);
    }

    public void RefreshMenu()
    {
        foreach(var entry in spawnedEntries)
            Destroy(entry);
        spawnedEntries.Clear();

        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            GameObject go = Instantiate(menuEntryPrefab, contentParent);
            QuestMenuEntry entry = go.GetComponent<QuestMenuEntry>();
            entry.Setup(quest);
            spawnedEntries.Add(go);
        }
    }
}
