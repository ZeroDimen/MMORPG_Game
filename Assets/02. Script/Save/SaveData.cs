using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSaveData
{
    public int id;
    public int amount;

    public ItemSaveData(int id, int amount)
    {
        this.id = id;
        this.amount = amount;
    }
}

[Serializable]
public class CharacterSaveData
{
    public string playerName;
    public int level;
    public int gold;
    public Vector3 pos;
    public List<ItemSaveData> inventoryItems = new List<ItemSaveData>();
    public List<QuestSaveData> activeQuests = new List<QuestSaveData>();
    public List<int> completedQuestIDs = new List<int>();
}

[Serializable]
public class QuestSaveData
{
    public int questID;
    public List<int> taskProgresses = new List<int>();
    public bool isPinned;

    public QuestSaveData(int id, List<int> progresses, bool pinned)
    {
        questID = id;
        taskProgresses = progresses;
        isPinned = pinned;
    }
}

public class SaveData : MonoBehaviour
{
    
}
