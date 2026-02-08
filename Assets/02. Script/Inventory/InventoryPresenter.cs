using System.Collections.Generic;

public class InventoryPresenter
{
    private readonly InventoryModel _model;
    private readonly InventoryView _view;

    private ItemType _currentType = ItemType.Equipment;
    private SubItemType _currentSubType = SubItemType.None;
    private List<InstanceItem> _currentItems;

    public InventoryPresenter(InventoryModel model, InventoryView view)
    {
        _model = model;
        _view = view;

        _model.OnInventoryChanged += RefreshCurrentItemType;

        _view.OnRefreshInventory += RefreshCurrentItemType;
        _view.OnItemTypeChanged += OnItemTypeSelected;
        _view.OnItemClick += OnItemClick;
        _view.OnSubItemTypeClick += OnSubItemTypeTypeSelected;
    }

    public void Init()
    {
        RenderItemType(_currentType);
    }

    private void OnItemTypeSelected(ItemType type)
    {
        // 서브 아이템 타입 모음집 교체
        foreach (var colletion in _view.subTypeButtonCollection)
        {
            if(colletion.type == _currentType)
                colletion.gameObject.SetActive(false);
            if(colletion.type == type)
                colletion.gameObject.SetActive(true);
        }
        _currentType = type;
        RenderItemType(type);
    }

    private void OnSubItemTypeTypeSelected(SubItemType subType)
    {
        _currentSubType = subType;
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
        if(_currentSubType == SubItemType.None)
            _view.ShowItemInformation(_currentItems[index]);
        else
        {
            var selectedItems = _currentItems.FindAll(i => i.SubType == _currentSubType);
            _view.ShowItemInformation(selectedItems[index]);
        }
    }

    private void RefreshCurrentItemType()
    {
        RenderItemType(_currentType);
    }

    private void RenderItemType(ItemType type)
    {
        _currentItems = _model.GetItemsByType(type);

        _view.RenderItems(_currentItems);
        
        _view.UpdateCapacityText(_currentItems.Count, _model.MaxCapacity);
    }
}
