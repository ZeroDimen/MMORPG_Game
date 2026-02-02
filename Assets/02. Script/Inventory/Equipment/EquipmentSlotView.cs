using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class EquipmentSlotView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    private AsyncOperationHandle<Sprite> _iconHandle;
    public DetailItemType detailType;

    public void Setup(InstanceItem item)
    {
        if (item == null || item.IconReference == null)
        {
            ClearIcon();
            return;
        }
        
        ReleaseIcon();
        LoadIcon(item);
    }

    private void LoadIcon(InstanceItem item)
    {
        ReleaseIcon();
        
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
                if (this != null && iconImage != null && _iconHandle.Equals(h))
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
    
    public void ClearIcon()
    {
        // 현재 로딩 중이거나 이미 로드된 핸들 해제
        if (_iconHandle.IsValid())
        {
            Addressables.Release(_iconHandle);
            _iconHandle = default; // 핸들을 비워서 중복 해제 방지 [cite: 1]
        }

        if (iconImage != null)
            iconImage.sprite = null;
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
