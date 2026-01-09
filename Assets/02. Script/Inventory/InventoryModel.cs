using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public enum ItemType
{
    None, 
    Equipment, 
    Tool, 
    General, 
    Costume, 
    Mount
}

public enum SubItemType
{
    None,
    Weapon, Armor, Accessory,                               // Equipment
    Bag, TooolDecoration, MusicalInstrument, SheetMusic,    // Tool
    Consumable, Food, Material, Goods, Quest,               // General
    Costume, FashionDecoration, FashionWeapon,              // Fashion
    CompanionPet, Mount, MountEquipment                     // Mount
}

[Serializable]
public class InstanceItem
{
    // 기본 정보
    public int ID { get; }
    public string Name { get; }
    public ItemType Type { get; }
    public SubItemType SubType { get; }
    public int MaxCapacity { get; }
    public bool IsStackable { get; }
    public int Amount;
    public AssetReferenceSprite IconReference { get; }
    
    public InstanceItem(int id, string name, int amount, ItemType type, SubItemType subType, bool isStackable, AssetReferenceSprite iconReference)
    {
        ID = id;
        Name = name;
        Amount = amount;
        Type = type;
        SubType = subType;
        IsStackable = isStackable;
        IconReference = iconReference;
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
            _instanceItems.Add(new InstanceItem(itemID, originalData.itemName, amount, originalData.type,originalData.subType,
                originalData.isStackable, originalData.iconReference));
        }

        OnInventoryChanged?.Invoke();
    }

    public List<InstanceItem> GetItemsByType(ItemType filterType)
    {
        if (filterType == ItemType.None) return _instanceItems;
        return _instanceItems.FindAll(item => item.Type == filterType);
    }

    public void RemoveItem()
    {
        
    }
}