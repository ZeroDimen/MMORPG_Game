using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrackerPresenter : MonoBehaviour
{
    [SerializeField] private GameObject questEntryPrefab;
    [SerializeField] private Transform container;

    private List<GameObject> spawnedEntries = new List<GameObject>();

    private void OnEnable()
    {
        GameEvents.OnQuestListChanged += RefreshTracker;
    }

    private void OnDisable()
    {
        GameEvents.OnQuestListChanged -= RefreshTracker;
    }

    private void RefreshTracker()
    {
        foreach(var entry in spawnedEntries)
            Destroy(entry);
        spawnedEntries.Clear();

        foreach (var quest in QuestManager.Instance.pinnedQuests)
        {
            GameObject go = Instantiate(questEntryPrefab, container);
            go.GetComponent<QuestEntry>().Setup(quest);
            spawnedEntries.Add(go);
        }
    }
}
