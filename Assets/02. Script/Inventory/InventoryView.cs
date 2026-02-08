using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryView : MonoBehaviour, IInventoryView
{
    public event Action OnRefreshInventory;
    public event Action<ItemType> OnItemTypeChanged;
    public event Action<int> OnItemClick;
    public event Action<SubItemType> OnSubItemTypeClick;
    
    [SerializeField] private List<InventorySlotView> _slotPool = new List<InventorySlotView>();
    
    [SerializeField] private ItemTypeButton[] TypeButtons;
    [SerializeField] private SubItemTypeButton[] SubTypeButtons;
    public SubItemTypeButtonCollection[] subTypeButtonCollection;
    
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private GameObject infomationText;

    [SerializeField] private ItemPanel _itemPanel;
    [SerializeField] private GameObject dim;

    private void Awake()
    {
        foreach (var button in SubTypeButtons)
            button.Bind((type) => OnSubItemTypeClick?.Invoke(type));

        foreach (var button in TypeButtons)
            button.Bind((type) => OnItemTypeChanged?.Invoke(type));
        
        foreach (var slot in _slotPool)
            slot.OnSlotClicked += (id) => OnItemClick?.Invoke(id);
        
        _itemPanel.OnConfirmAction += () =>
        {
            dim.SetActive(false);
        };

        _itemPanel.OnEquipAction += OnRefreshInventory;
    }

    public void ShowItemInformation(InstanceItem item)
    {
        _itemPanel.ShowInfo(item);
        dim.SetActive(true);
    }
    
    public void RenderItems(List<InstanceItem> items)
    {
        foreach (var slot in _slotPool)
            slot.gameObject.SetActive(false);

        for (int i = 0; i < items.Count; i++)
        {
            _slotPool[i].gameObject.SetActive(true);
            _slotPool[i].Setup(items[i]);
        }
    }

    public void UpdateCapacityText(int current, int max)
    {
        if (current == 0)
        {
            infomationText.SetActive(true);
            capacityText.gameObject.SetActive(false);
        }
        else
        {
            infomationText.SetActive(false);
            capacityText.gameObject.SetActive(true);
            capacityText.text = $"{current} / {max}";
        }
    }

    private void OnEnable()
    {
        OnRefreshInventory?.Invoke();
    }
}
