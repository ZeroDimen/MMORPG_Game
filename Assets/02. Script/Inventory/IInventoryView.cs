using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryView
{
    void RenderItems(List<InstanceItem> items);
    void UpdateCapacityText(int current, int max);

    event Action<ItemType> OnItemTypeChanged;
    event Action<int> OnItemClick;
}
