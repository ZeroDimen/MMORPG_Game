using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour
{
    [SerializeField] private Button slotButton;
    public Action<int> OnSlotClicked;
    [SerializeField] private int slotIndex;
    
    [Header("UI Components")] [SerializeField]
    private Image iconImage;

    [SerializeField] private Image frameImage; // 등급별 테두리
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI itemName;

    [Header("Settings")] [SerializeField] private Color[] gradeColors;

    private AsyncOperationHandle<Sprite> _iconHandle;

    public void Setup(InstanceItem item)
    {
        // 1. 이전 로드 작업 해제 (메모리 관리)
        ReleaseIcon();
        
        // 2. 수량 표시
        // 겹치기 가능한 아이템이고 1개보다 많을 때만 숫자 표시
        amountText.text = (item.IsStackable && item.Amount > 1) ? item.Amount.ToString() : "";
        itemName.text = item.Name;

        // 3. 등급별 테두리 설정 (아이템 데이터에 Grade 정보가 있다고 가정)
        // if (item.Grade < gradeColors.Length)
        //    frameImage.color = gradeColors[item.Grade];

        // 4. 아이콘 비동기 로드 (Addressables)
        LoadIcon(item);
        
        slotButton.onClick.RemoveAllListeners();
        slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(slotIndex));
    }

    private void LoadIcon(InstanceItem item)
    {
        if(_iconHandle.IsValid())
            Addressables.Release(_iconHandle);

        _iconHandle = Addressables.LoadAssetAsync<Sprite>(item.IconReference.RuntimeKey);
        _iconHandle.Completed += (handle) => {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                if(this != null && gameObject.activeInHierarchy)
                    iconImage.sprite = handle.Result;
        };
    }

    private void ReleaseIcon()
    {
        if (_iconHandle.IsValid())
        {
            Addressables.Release(_iconHandle);
        }
    }

    private void OnDisable()
    {
        ReleaseIcon();
    }
}
    
    
