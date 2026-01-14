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
}

public class SaveData : MonoBehaviour
{
    
}
