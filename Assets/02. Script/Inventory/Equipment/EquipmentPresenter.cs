using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EquipmentPresenter : Singleton<EquipmentPresenter>
{
    [SerializeField] private EquipmentView view;
    private List<InstanceItem> equipmenItems = new List<InstanceItem>();

    public void OnEquipment(InstanceItem item, bool isLoaded)
    {
        item.isEquip = true;
        equipmenItems.Add(item);
        if(!isLoaded)
            GameEvents.OnItemEquipped?.Invoke(item);
        view.RenderEquipment(item, item.DetailType);
        AudioManager._instance.SfxPlay("Equipment");
    }

    public void UnEquipment(InstanceItem item)
    {
        item.isEquip = false;
        equipmenItems.Remove(item);
        GameEvents.OnItemUnEquipped?.Invoke(item);
        view.RenderEquipment(null, item.DetailType);
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
    }
}
