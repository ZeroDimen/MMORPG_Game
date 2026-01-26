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
    [SerializeField] private GameObject equipText;

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
        equipText.SetActive(item.isEquip);

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
        if (_iconHandle.IsValid())
        {
            Addressables.Release(_iconHandle);
            _iconHandle = default; // 핸들 초기화
        }
        
        if (item?.IconReference == null) return;

        // 2. 로컬 변수에 현재 로드하려는 아이템의 정보를 고정 (클로저 문제 방지)
        var currentReference = item.IconReference;

        // 3. 비동기 로드 시작
        var handle = Addressables.LoadAssetAsync<Sprite>(currentReference.RuntimeKey);
        _iconHandle = handle;

        handle.Completed += (h) => 
        {
            // 4. 완료 시점에 이 슬롯이 여전히 유효한지 + 이 핸들이 내가 마지막으로 요청한 핸들이 맞는지 확인
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                // 이 핸들이 현재 클래스의 최신 핸들(_iconHandle)과 일치할 때만 이미지를 교체
                // (로딩 중에 다른 아이템으로 데이터가 바뀌었을 경우를 대비)
                if (this != null && gameObject.activeInHierarchy && _iconHandle.Equals(h))
                {
                    iconImage.sprite = h.Result;
                }
                else
                {
                    // 만약 그 사이 다른 아이템을 보여줘야 해서 이 결과가 필요 없어졌다면 즉시 해제
                    Addressables.Release(h);
                }
            }
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
    
    
