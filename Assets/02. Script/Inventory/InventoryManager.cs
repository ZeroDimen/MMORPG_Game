using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int n;
    
    [Header("Data Settings")] [SerializeField]
    private ItemDataBase itemDataBase;

    [Header("UI References")] [SerializeField]
    private InventoryView inventoryView;

    private InventoryModel _model;
    private InventoryPresenter _presenter;

    private void Awake()
    {
        itemDataBase.Initialize();
        _model = new InventoryModel(itemDataBase);
        _presenter = new InventoryPresenter(_model, inventoryView);
    }

    private void Start()
    {
        _presenter.Init();
        SaveManager.Instance.Init(_model);
    }
    
    [ContextMenu("Add Test Item")]
    public void AddTestItem()
    {
        // 인스펙터에서 우클릭하여 테스트로 아이템을 추가해볼 수 있습니다.
        // 예: 101번 아이템을 5개 추가
        _model.AddItem(n, 1);
    }
}
