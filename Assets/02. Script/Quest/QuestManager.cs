using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : Singleton<QuestManager>
{
    [Header("Database")] [SerializeField] private QuestDatabase questDatabase;
    
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
            
            activeQuests.Add(newQuest);
            if(pinnedQuests.Count < MaxPinCount)
                ToggleQuestPin(newQuest, true);
            
            GameEvents.OnQuestListChanged?.Invoke();
        }
    }

    public void HandleProgressUpdate(QuestType type, int targetID, int amount = 1)
    {
        // 퀘스트 완료시 리스트에서 제거하게 되는데 리스트 순회 도중에 제거하기 때문에 인덱스가 꼬임
        // 따라서 인덱스가 안꼬이게 뒤에서부터 시작
        for(int i = activeQuests.Count - 1; i >= 0; i--)
        {
            QuestData quest = activeQuests[i];
            if(quest.IsCompleted) continue;

            if (quest.isSequential)
            {
                var task = quest.CurrentTask;

                if (task.questType == type && task.targetID == targetID)
                {
                    task.currentAmount += amount;

                    if (quest.IsCompleted)
                    {
                        GameEvents.OnQuestCompleted?.Invoke(quest);
                    }
                }
            }
            else
            {
                for (int j = 0; j < quest.questTasks.Count; j++)
                {
                    var task = quest.questTasks[j];

                    if (task.questType == type && task.targetID == targetID)
                    {
                        task.currentAmount += amount;

                        if (quest.IsCompleted)
                        {
                            GameEvents.OnQuestCompleted?.Invoke(quest);
                        }
                    }
                }
            }
        }
        GameEvents.OnQuestListChanged?.Invoke();
    }

    public bool IsCompleteQuestByNPCID(QuestType type, int npcID, out string key)
    {
        foreach (var quest in activeQuests)
        {
            var task = quest.CurrentTask;

            if (task.questType == type && task.targetID == npcID)
            {
                key = task.questCompleteTalkKey;
                return true;
            }
        }

        key = null;
        return false;
    }

    public bool IsProgressingQuestByQuestID(int questID)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.questID == questID)
                return true;
        }

        return false;
    }
    
    public List<QuestSaveData> GetActiveQuestSaveData()
    {
        List<QuestSaveData> saveDataList = new List<QuestSaveData>();
        foreach (var quest in activeQuests)
        {
            List<int> progresses = new List<int>();
            foreach(var task in quest.questTasks)
                progresses.Add(task.currentAmount);
            saveDataList.Add(new QuestSaveData(quest.questID, progresses ,pinnedQuests.Contains(quest)));
        }
        return saveDataList;
    }

    public void LoadQuestData(List<QuestSaveData> activeData, List<int> completedIDs)
    {
        activeQuests.Clear();
        pinnedQuests.Clear();
        completedQuestIDs = new List<int>(completedIDs);

        foreach (var data in activeData)
        {
            QuestData origin = questDatabase.GetQuestByID(data.questID);
            if (origin != null)
            {
                QuestData newQuest = Instantiate(origin);
                
                for(int i = 0; i < newQuest.questTasks.Count; i++)
                    if (i < data.taskProgresses.Count)
                        newQuest.questTasks[i].currentAmount = data.taskProgresses[i];
                
                activeQuests.Add(newQuest);
                if(data.isPinned)
                    pinnedQuests.Add(newQuest);
            }
        }
        GameEvents.OnQuestListChanged?.Invoke();
    }
    
    [ContextMenu("Kill Monster")]
    public void KillTest()
    {
        HandleProgressUpdate(QuestType.Kill, 0, 1);
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
    }
}
