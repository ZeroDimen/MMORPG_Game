using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SubItemTypeButton : MonoBehaviour
{
    public SubItemType subType;
    private Button _button;

    public void Bind(Action<SubItemType> onClickAction)
    {
        _button = GetComponent<Button>();
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => onClickAction?.Invoke(subType));
    }
}
