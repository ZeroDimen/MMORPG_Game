using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class InstanceItem
{
    // 기본 정보
    public int ID { get; }
    public string Name { get; }
    public string Information { get; }
    public ItemType Type { get; }
    public SubItemType SubType { get; }
    public DetailItemType DetailType { get; }
    public int MaxCapacity { get; }
    public bool IsStackable { get; }
    public int Amount;
    public bool isEquip;
    public AssetReferenceSprite IconReference { get; }
    public List<StatBonus> _statBonusList;
    
    public InstanceItem(int id, string name, string information, int amount,
        ItemType type, SubItemType subType, DetailItemType detailType,
        bool isStackable, AssetReferenceSprite iconReference, List<StatBonus> statBonusList)
    {
        ID = id;
        Name = name;
        Information = information;
        Amount = amount;
        Type = type;
        SubType = subType;
        DetailType = detailType;
        IsStackable = isStackable;
        IconReference = iconReference;
        _statBonusList = statBonusList;
    }
}

public class InventoryModel
{
    private List<InstanceItem> _instanceItems = new List<InstanceItem>();
    public Action OnInventoryChanged;
    private ItemDataBase _database;

    private int maxCapacity = 30;
    public int MaxCapacity => maxCapacity;

    private int currentCapacity;
    
    public InventoryModel(ItemDataBase db)
    {
        _database = db;
        GameEvents.OnQuestCompleted += GiveQuestReward;
    }

    private void GiveQuestReward(QuestData completedQuest)
    {
        if (completedQuest.rewardItem != null)
        {
            AddItem(completedQuest.rewardItem.id, completedQuest.rewardAmount);
            Debug.Log($"[Reward] {completedQuest.title} 보상 획득: " +
                      $"{completedQuest.rewardItem.itemName} x{completedQuest.rewardAmount}");
        }
    }

    public void AddItem(int itemID, int amount)
    {
        ItemData originalData = _database.GetItemById(itemID);
        if (originalData == null) return;
        var existingItem = _instanceItems.Find(i => i.ID == itemID && originalData.isStackable);

        if (existingItem != null)
        {
            int remainingSpace = originalData.maxCapacity - existingItem.Amount;

            if (amount <= remainingSpace)
                existingItem.Amount += amount;
            else
            {
                // 여유 공간을 넘치는 경우: 일단 현재 슬롯을 꽉 채우고
                existingItem.Amount = originalData.maxCapacity;
                AddItem(itemID, amount - remainingSpace); // 남은 건 다시 추가 (재귀)
                return;
            }
        }
        else
        {
            _instanceItems.Add(new InstanceItem(itemID, originalData.itemName, originalData.Information,
                amount, originalData.type,originalData.subType, originalData.detailType,
                originalData.isStackable, originalData.iconReference, originalData.statBonusList));
        }

        OnInventoryChanged?.Invoke();
        GameEvents.OnQuestProgressUpdated?.Invoke(QuestType.Gather, itemID, amount);
    }

    public List<InstanceItem> GetItemsByType(ItemType filterType)
    {
        if (filterType == ItemType.None) return _instanceItems;
        return _instanceItems.FindAll(item => item.Type == filterType);
    }
    
    public List<ItemSaveData> GetSaveData()
    {
        List<ItemSaveData> saveDataList = new List<ItemSaveData>();
        foreach (var item in _instanceItems)
        {
            // 아이템의 ID와 수량만 저장용 클래스에 담습니다.
            saveDataList.Add(new ItemSaveData(item.ID, item.Amount, item.isEquip));
        }
        return saveDataList;
    }
    
    public void LoadData(List<ItemSaveData> savedItems)
    {
        _instanceItems.Clear(); // 기존 아이템 비우기

        foreach (var save in savedItems)
        {
            // 아이템 데이터베이스에서 ID로 원본 ScriptableObject를 찾음
            ItemData origin = _database.GetItemById(save.id);
        
            if (origin != null)
            {
                // 원본 정보를 바탕으로 인스턴스 생성 및 수량 설정
                var item = new InstanceItem(
                    origin.id,
                    origin.itemName,
                    origin.Information,
                    save.amount,
                    origin.type,
                    origin.subType,
                    origin.detailType,
                    origin.isStackable,
                    origin.iconReference,
                    origin.statBonusList);
                
                _instanceItems.Add(item);

                if (save.isEquipment)
                {
                    EquipmentPresenter.Instance.OnEquipment(item, true);
                }
            }
        }

        // 데이터가 바뀌었으므로 View를 새로고침하도록 이벤트 발생
        OnInventoryChanged?.Invoke();
    }
}