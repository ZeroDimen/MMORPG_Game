using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSaveData
{
    public int id;
    public int amount;
    public bool isEquipment;

    public ItemSaveData(int id, int amount, bool isEquipment)
    {
        this.id = id;
        this.amount = amount;
        this.isEquipment = isEquipment;
    }
}

[Serializable]
public class CharacterSaveData
{
    public string playerName;
    public int HP;
    public int MAXHP;
    public int LV;
    public int MAXEXP;
    public int EXP;
    public int ATK;     // 공격력
    public int DEF;     // 방어력
    public int DEX;     // 이동속도
    public Vector3 pos;
    public List<ItemSaveData> inventoryItems = new List<ItemSaveData>();
    public List<InstanceItem> equipmentItems = new List<InstanceItem>();
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
