using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDataBase : ScriptableObject, ISerializationCallbackReceiver
{
    public List<ItemData> itemDatas; // 인스펙터 노출용
    private Dictionary<int, ItemData> _itemDict = new(); // 실제 검색용
    
    public void OnBeforeSerialize(){}
    // 유니티가 직렬화된 후 실행됨
    public void OnAfterDeserialize()
    {
        _itemDict.Clear();
        foreach (var data in itemDatas)
        {
            if(data != null && !_itemDict.ContainsKey(data.id))
                _itemDict.Add(data.id, data);
        }
    }

    public ItemData GetItemById(int id)
    {
        return _itemDict.GetValueOrDefault(id);
    }
}
