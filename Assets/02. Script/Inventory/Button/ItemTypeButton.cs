using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemTypeButton : MonoBehaviour
{
    public ItemType type;
    private Button _button;

    public void Bind(Action<ItemType> onClickAction)
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => onClickAction?.Invoke(type));
    }
}
