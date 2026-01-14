using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    // 기본 정보
    public int id;
    public string itemName;
    public ItemType type;
    public SubItemType subType;
    public int maxCapacity;
    public bool isStackable;
    public string Information;
    
    // UI를 위한 정보
    public AssetReferenceSprite iconReference;
}
