using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    public event Action OnConfirmAction;
    
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI information;
    [SerializeField] private TextMeshProUGUI amount;
    [SerializeField] private Image icon;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unEquipButton;
    [SerializeField] private Button equipconfirmButton;

    private AsyncOperationHandle<Sprite> _iconHandle;

    public void ShowInfo(InstanceItem item)
    {
        gameObject.SetActive(true);
        if (item.Type == ItemType.Equipment)
        {
            if (!item.isEquip)
            {
                equipButton.gameObject.SetActive(true);
                unEquipButton.gameObject.SetActive(false);
                
                equipButton.onClick.RemoveAllListeners();
                equipButton.onClick.AddListener(() =>
                {
                    EquipmentPresenter.Instance.OnEquipment(item);
                    OnConfirmPanel();
                });
            }
            else
            {
                unEquipButton.gameObject.SetActive(true);
                equipButton.gameObject.SetActive(false);
                
                unEquipButton.onClick.RemoveAllListeners();
                unEquipButton.onClick.AddListener((() =>
                {
                    EquipmentPresenter.Instance.UnEquipment(item);
                    OnConfirmPanel();
                }));
            }
            
            confirmButton.gameObject.SetActive(false);
            equipconfirmButton.gameObject.SetActive(true);
            equipconfirmButton.onClick.RemoveAllListeners();
            equipconfirmButton.onClick.AddListener(OnConfirmPanel);
        }
        else
        {
            confirmButton.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(false);
            equipconfirmButton.gameObject.SetActive(false);
            unEquipButton.gameObject.SetActive(false);
            
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmPanel);   
        }
        
        itemName.text = item.Name;
        information.text = item.Information;
        amount.text = item.Amount.ToString();

        if (_iconHandle.IsValid())
        {
            Addressables.Release(_iconHandle);
            _iconHandle = default;
        }

        var checkHandle = item.IconReference.OperationHandle;
        if (checkHandle.IsValid() && checkHandle.Status == AsyncOperationStatus.Succeeded)
            icon.sprite = checkHandle.Convert<Sprite>().Result;
        else
        {
            _iconHandle = item.IconReference.LoadAssetAsync<Sprite>();
            _iconHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    icon.sprite = handle.Result;
            };
        }
    }

    public void OnConfirmPanel()
    {
        if(_iconHandle.IsValid())
            Addressables.Release(_iconHandle);

        itemName.text = "";
        information.text = "";
        amount.text = "";
        icon.sprite = null;
        
        OnConfirmAction?.Invoke();
        gameObject.SetActive(false);
    }
}
