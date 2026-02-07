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
        // if (item == null || item.IconReference == null)
        // {
        //     ClearIcon();
        //     return;
        // }
        //
        // ReleaseIcon();
        // LoadIcon(item);

        if (item == null || item.img == null)
        {
            iconImage.sprite = null;
            return;
        }

        iconImage.sprite = item.img;
    }

    // private void LoadIcon(InstanceItem item)
    // {
    //     if (item?.IconReference == null) return;
    //
    //     // [핵심] 기존 핸들을 Release하지 않고 일단 둡니다. (누수가 생겨도 이미지는 나옵니다)
    //     Addressables.LoadAssetAsync<Sprite>(item.IconReference.RuntimeKey).Completed += (handle) => 
    //     {
    //         if (handle.Status == AsyncOperationStatus.Succeeded)
    //         {
    //             if (this == null || iconImage == null) return;
    //
    //             // 1. 이미지 할당
    //             iconImage.sprite = handle.Result;
    //
    //             // 2. [필수] 투명도 및 활성화 강제 리셋
    //             iconImage.enabled = true;
    //             Color c = iconImage.color;
    //             c.a = 1f;
    //             iconImage.color = c;
    //
    //             // 3. [강력 추천] UI 강제 갱신 알림
    //             iconImage.SetAllDirty(); 
    //         
    //             Debug.Log($"[Force Success] {item.Name} 이미지가 강제로 적용됨");
    //         }
    //         else
    //         {
    //             Debug.LogError($"[Force Fail] {item.Name} 로드 실패");
    //         }
    //     };
    // }
    //
    // public void ClearIcon()
    // {
    //     // 현재 로딩 중이거나 이미 로드된 핸들 해제
    //     if (_iconHandle.IsValid())
    //     {
    //         Addressables.Release(_iconHandle);
    //         _iconHandle = default; // 핸들을 비워서 중복 해제 방지 [cite: 1]
    //     }
    //
    //     if (iconImage != null)
    //         iconImage.sprite = null;
    // }
    //
    // private void ReleaseIcon()
    // {
    //     if (_iconHandle.IsValid())
    //     {
    //         Addressables.Release(_iconHandle);
    //     }
    // }
    //
    // private void OnDisable()
    // {
    //     ReleaseIcon();
    // }
}
