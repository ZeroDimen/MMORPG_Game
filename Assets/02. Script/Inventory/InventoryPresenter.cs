using System.Collections.Generic;
using UnityEngine;

public class InventoryPresenter
{
    private readonly InventoryModel _model;
    private readonly InventoryView _view;

    private ItemType _currentType = ItemType.Equipment;
    private List<InstanceItem> _currentItems;

    public InventoryPresenter(InventoryModel model, InventoryView view)
    {
        _model = model;
        _view = view;

        _model.OnInventoryChanged += RefreshCurrentCategory;

        _view.OnTabChanged += OnCategorySelected;
        _view.OnItemClick += OnItemClick;
    }

    public void Init()
    {
        RenderCategory(_currentType);
    }

    private void OnCategorySelected(ItemType type)
    {
        _currentType = type;
        RenderCategory(type);
    }

    private void OnSubCategorySelected(SubItemType subType)
    {
        if (subType == SubItemType.None)
        {
            _view.RenderItems(_currentItems);
            return;
        }

        var items = _currentItems.FindAll(i => i.SubType == subType);
        _view.RenderItems(items);
    }

    private void OnItemClick(int index)
    {
        Debug.Log($"Item {index} clicked!");
    }

    private void RefreshCurrentCategory()
    {
        RenderCategory(_currentType);
    }

    private void RenderCategory(ItemType type)
    {
        _currentItems = _model.GetItemsByType(type);

        _view.RenderItems(_currentItems);
        
        _view.UpdateCapacityText(_currentItems.Count, _model.MaxCapacity);
    }
}
