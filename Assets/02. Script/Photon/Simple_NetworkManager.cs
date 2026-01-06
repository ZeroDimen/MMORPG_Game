using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Simple_NetworkManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";

    [SerializeField] private TMP_InputField nickNameField;
    [SerializeField] private Button connectButton;
    
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false); // 해상도 설정
        PhotonNetwork.SendRate = 60; // 내 컴퓨터 게임 정보에 대한 전송률
        PhotonNetwork.SerializationRate = 30; // Photon View 관측 중인 대상에 대한 전송률
        PhotonNetwork.GameVersion = gameVersion; // 버전 설정
    }

    private void Start()
    {
        connectButton.onClick.AddListener(Connect);
    }

    private void Connect() // Photon Master Server에 접속을 요청하는 함수
    {
        PhotonNetwork.NickName = nickNameField.text;
        
        PhotonNetwork.ConnectUsingSettings(); 
        Debug.Log("Master Server 접속");
    }

    public override void OnConnectedToMaster() // 방 생성 및 생성 되었다면 접속하는 함수 (Master Server에 접속시 호출됨)
    {
        PhotonNetwork.JoinOrCreateRoom("MMORPG_Room", new RoomOptions { MaxPlayers = 5 }, null);
        Debug.Log("MMORPG_Room 접속 완료");
    }

    public override void OnJoinedRoom() // 캐릭터를 생성하는 함수 (방 접속 완료시 호출됨)
    {
        PhotonNetwork.LoadLevel("Main");
        Debug.Log("씬 전환");
    }
}
