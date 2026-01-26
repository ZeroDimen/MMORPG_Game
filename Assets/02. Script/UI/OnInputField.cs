using TMPro;
using UnityEngine;
using UnityEngine.EventSystems; 

// 채팅 입력창 활성화 시 마우스로 선택할 필요 없이 바로 작성할 수 있도록 제어하는 스크립트
public class OnInputField : MonoBehaviour
{
    [SerializeField] ChatManager chatManager;
    private TMP_InputField inputField;
    private void Awake()
    {
        inputField =  GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        // EventSystem가 inputField 오브젝트 선택
        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        // 키보드 입력 대기 상태 진입
        inputField.ActivateInputField();
    }

    private void OnDisable()
    {
        // 메시지 전송
        chatManager.SendMsg();
    }
}
