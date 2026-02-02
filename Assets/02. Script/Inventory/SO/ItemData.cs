using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public enum ItemType
{
    None, 
    Equipment, 
    Tool, 
    General, 
    Fashion, 
    Mount
}

[Serializable]
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
public enum DetailItemType
{
    None,
    Weapon, Earring, Necklace, Ring, Bracelet, Hat, Top, Gloves, Bottom, Shoes
}

[Serializable]
public enum StatType
{
    None,
    HP, MP, EXP, 
    ATK, DEF, DEX
}

[Serializable]
public class StatBonus
{
    public StatType type;
    public int value;
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    // 기본 정보
    public int id;
    public string itemName;
    public ItemType type;
    public SubItemType subType;
    public DetailItemType detailType;
    public int maxCapacity;
    public bool isStackable;
    public string Information;
    public List<StatBonus> statBonusList;
    
    // UI를 위한 정보
    public AssetReferenceSprite iconReference;
}
