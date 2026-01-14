using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDataBase : ScriptableObject, ISerializationCallbackReceiver
{
    public List<ItemData> itemDatas; // 인스펙터 노출용
    private Dictionary<int, ItemData> _itemDict = new(); // 실제 검색용
    
    public void OnBeforeSerialize(){}
    public void OnAfterDeserialize()
    {
        _itemDict.Clear();
        foreach (var data in itemDatas)
        {
            if (data == null) {
                Debug.LogError("DB에 비어있는(Missing) 칸이 있습니다!");
                continue;
            }
            if (!_itemDict.ContainsKey(data.id)) {
                _itemDict.Add(data.id, data);
                Debug.Log($"아이템 등록 성공: ID {data.id} - {data.itemName}");
            }
        }
    }
    
    // // 유니티가 직렬화된 후 실행됨
    // public void OnAfterDeserialize()
    // {
    //     _itemDict.Clear();
    //     foreach (var data in itemDatas)
    //     {
    //         if(data != null && !_itemDict.ContainsKey(data.id))
    //             _itemDict.Add(data.id, data);
    //     }
    // }

    public ItemData GetItemById(int id)
    {
        return _itemDict.GetValueOrDefault(id);
    }
}
