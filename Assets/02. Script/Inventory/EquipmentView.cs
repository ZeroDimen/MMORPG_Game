using System.Collections.Generic;
using UnityEngine;

public class EquipmentView : MonoBehaviour
{
    [SerializeField] private List<EquipmentSlotView> slots = new List<EquipmentSlotView>();

    public void RenderEquipment(InstanceItem item, DetailItemType type)
    {
        slots.Find(i => i.detailType == type).Setup(item);
    }
    
    
}
