using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_InputField inputField; // 메시지 입력 필드
    [SerializeField]
    private TMP_Text chattingList; // 접속 중인 플레이어 목록을 표시할 택스트 UI
    [SerializeField]
    private TMP_Text chatLog; // 채팅 로그를 표시할 택스트
    [SerializeField]
    private ScrollRect scrollRect; // 채팅 로그의 스크롤을 제어하는 ScrollRect
    
    private string chatters; // 접속 중인 플레이어 목록 문자열
    
    [Header("자동 페이드")]
    [SerializeField] private CanvasGroup chatCanvasGroup;
    [SerializeField] private float visibleDuration = 4f;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine _fadeCoroutine;
    
    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true; // Photon의 메시지 큐가 작동하도록 설정
        ShowChat();
    }
    
    public void SendMsg() // 메시지 전송 함수
    {
        ChatterUpdate();
        // 메시지 입력 필드가 비어 있는지 확인
        if (string.IsNullOrEmpty(inputField.text))
        {
            Debug.Log("메시지가 비어있습니다.");
            return;
        }
        
        // 메시지 형식을 지정하여 문자열로 만듦
        string msg = string.Format("[{0}]: {1}", PhotonNetwork.LocalPlayer.NickName, inputField.text);
        // 다른 클라이언트들에게 메시지 전송
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        // 자신 메시지 수신 처리
        ReceiveMsg(msg);

        // 입력 필드 초기화
        inputField.text = "";
    }
    
    void Update()
    {
        // 플레이어 목록 업데이트
        ChatterUpdate();
    }

    // 접속 중인 플레이어 목록을 업데이트
    private void ChatterUpdate()
    {
        chatters = "*Player List*\n";

        // 현재 접속 중인 모든 플레이어 닉네임을 리스트에 추가
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient) continue; // 마스터 클라이언트 제외
            chatters += player.NickName + "\n";
        }

        chattingList.text = chatters;
    }
    // 플레이어 접속시 호출되는 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string msg = string.Format("<color=#00ff00>[{0}] is joined.</color>", newPlayer.NickName);
        ReceiveMsg(msg);
    }

    // 플레이어 퇴장시 호출되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = string.Format("<color=#ff0000>[{0}] is left.</color>", otherPlayer.NickName);
        ReceiveMsg(msg);
    }

    // RPC 메서드: 메시지를 수신하여 채팅 로그에 추가하는 메서드
    // 네트워크 상의 다른 클라이언트에서 호출하는 메서드
    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        // 메시지 추가
        chatLog.text += "\n" + msg;

        // 스크롤을 가장 아래로 내림
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0.0f;
        }

        ShowChat();
    }
    
    private void ShowChat()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(ShowAndFade());
    }

    private IEnumerator ShowAndFade()
    {
        // 즉시 완전히 보이게
        chatCanvasGroup.alpha = 1f;

        // 유지 시간 대기
        yield return new WaitForSeconds(visibleDuration);

        // 서서히 사라지기
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            chatCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        chatCanvasGroup.alpha = 0f;
    }
    
    // 입력창을 열 때
    public void OnChatInputOpen()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        chatCanvasGroup.alpha = 1f; // 입력 중엔 계속 보이게
    }

    // 입력창을 닫을 때 (전송 후 등)
    public void OnChatInputClose()
    {
        ShowChat(); // 이제 타이머 시작해서 페이드
    }
}
