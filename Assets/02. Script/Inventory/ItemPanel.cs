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

    private AsyncOperationHandle<Sprite> _iconHandle;

    public void ShowInfo(InstanceItem item)
    {
        gameObject.SetActive(true);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(OnConfirmPanel);
        
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
