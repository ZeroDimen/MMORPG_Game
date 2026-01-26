using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]
public class ItemDataBase : ScriptableObject
{
    public List<ItemData> itemDatas; // 인스펙터 노출용
    private Dictionary<int, ItemData> _itemDict = new(); // 실제 검색용
    
    private void OnValidate()
    {
        // 에디터에서 값을 수정하거나 리스트를 건드리면 즉시 실행됨
        BuildDictionary();
    }

    public void OnAfterDeserialize()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        _itemDict.Clear();
        if (itemDatas == null) return;

        foreach (var data in itemDatas)
        {
            if (data == null) continue;

            // ID가 중복되었는지 로그를 통해 확인
            if (_itemDict.ContainsKey(data.id))
            {
                Debug.LogWarning($"중복 ID 발견: {data.id} (아이템: {data.itemName}). ID를 수정하세요!");
                continue;
            }

            _itemDict.Add(data.id, data);
            // 등록되는 실제 ID를 로그로 찍어보세요
            Debug.Log($"딕셔너리 등록 성공: {data.itemName} (ID: {data.id})");
        }
    }

    public ItemData GetItemById(int id)
    {
        return _itemDict.GetValueOrDefault(id);
    }
}
