using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour, IInventoryView
{
    public event Action<ItemType> OnTabChanged;
    public event Action<int> OnItemClick;
    
    [SerializeField] private List<InventorySlotView> _slotPool = new List<InventorySlotView>();
    [SerializeField] private Button EquipmentButton;
    [SerializeField] private Button ToolButton;
    [SerializeField] private Button GeneralButton;
    [SerializeField] private Button FashionButton;
    [SerializeField] private Button MountButton;

    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private GameObject infomationText;

    private void Awake()
    {
        foreach (var slot in _slotPool)
            slot.OnSlotClicked += (id) => OnItemClick?.Invoke(id);
    }

    private void Start()
    {
        EquipmentButton.onClick.AddListener(() => { OnTabChanged?.Invoke(ItemType.Equipment);});
        ToolButton.onClick.AddListener(() => { OnTabChanged?.Invoke(ItemType.Tool);});
        GeneralButton.onClick.AddListener(() => { OnTabChanged?.Invoke(ItemType.General);});
        FashionButton.onClick.AddListener(() => { OnTabChanged?.Invoke(ItemType.Costume);});
        MountButton.onClick.AddListener(() => { OnTabChanged?.Invoke(ItemType.Mount);});
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
}
