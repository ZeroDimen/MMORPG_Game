using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : Singleton<QuestManager>
{
    public List<QuestData> activeQuests = new List<QuestData>();
    public List<QuestData> pinnedQuests = new List<QuestData>();
    public List<int> completedQuestIDs = new List<int>();
    private const int MaxPinCount = 5;

    private void OnEnable()
    {
        GameEvents.OnQuestProgressUpdated += HandleProgressUpdate;
        GameEvents.OnQuestAccepted += AddQuest;
        GameEvents.OnQuestPinChanged += ToggleQuestPin;
        GameEvents.OnQuestCompleted += HandleQuestCompleted;
    }


    private void OnDisable()
    {
        GameEvents.OnQuestProgressUpdated -= HandleProgressUpdate;
        GameEvents.OnQuestAccepted -= AddQuest;
        GameEvents.OnQuestPinChanged -= ToggleQuestPin;
        GameEvents.OnQuestCompleted -= HandleQuestCompleted;
    }

    private void HandleQuestCompleted(QuestData quest)
    {
        if(!completedQuestIDs.Contains(quest.questID))
            completedQuestIDs.Add(quest.questID);

        activeQuests.Remove(quest);
        pinnedQuests.Remove(quest);
        
        GameEvents.OnQuestListChanged?.Invoke();
    }
    private void ToggleQuestPin(QuestData quest, bool isPin)
    {
        if (isPin)
        {
            if (pinnedQuests.Count >= MaxPinCount)
            {
                Debug.LogWarning("트래커가 가득 찼습니다! 최대 5개만 등록 가능합니다.");
                return;
            }
            if(!pinnedQuests.Contains(quest)) pinnedQuests.Add(quest);
        }
        else
        {
            pinnedQuests.Remove(quest);
        }
        GameEvents.OnQuestListChanged?.Invoke();
    }

    private void AddQuest(QuestData quest)
    {
        if (!activeQuests.Contains(quest))
        {
            QuestData newQuest = Instantiate(quest);
            newQuest.currentAmount = 0;
            
            activeQuests.Add(newQuest);
            if(pinnedQuests.Count < MaxPinCount)
                ToggleQuestPin(newQuest, true);
            
            GameEvents.OnQuestListChanged?.Invoke();
        }
    }

    public void HandleProgressUpdate(QuestType type, int targetID, int amount = 1)
    {
        for(int i = activeQuests.Count - 1; i >= 0; i--)
        {
            QuestData quest = activeQuests[i];
            
            if(quest.IsCompleted) continue;

            if (quest.questType == type && quest.targetID == targetID)
            {
                quest.currentAmount += amount;

                if (quest.IsCompleted)
                {
                    GameEvents.OnQuestCompleted?.Invoke(quest);
                }
            }
        }
        GameEvents.OnQuestListChanged?.Invoke();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
    }
}
