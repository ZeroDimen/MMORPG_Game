using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";
    private string roomName = "MMORPG_Room";

    [SerializeField] private TMP_InputField nickNameField;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Image serverImage;
    
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false); // 해상도 설정
        PhotonNetwork.SendRate = 60; // 내 컴퓨터 게임 정보에 대한 전송률
        PhotonNetwork.SerializationRate = 30; // Photon View 관측 중인 대상에 대한 전송률
        PhotonNetwork.GameVersion = gameVersion; // 버전 설정
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Photon Master Server에 접속을 요청하는 함수
        nextButton.onClick.AddListener(ConnectRoom);
        serverButton.onClick.AddListener(CreateRoom);
        
        nextButton.interactable = false;
        serverButton.interactable = false;
        serverImage.color = Color.red;
    }

    private void ConnectRoom() 
    {
        if (nickNameField.text != "")
        {
            PhotonNetwork.NickName = nickNameField.text;
            PhotonNetwork.JoinRoom(roomName);
            Debug.Log("JoinRoom");
        }
    }

    private void CreateRoom() // 방(서버)생성을 요청 하는 함수
    {
        PhotonNetwork.NickName = "ServerClient";
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 10 });
    }
    public override void OnConnectedToMaster() // Photon Master Server 접속에 성공했을 경우 호출 되는 함수
    {
        Debug.Log("Master Server 접속");
        PhotonNetwork.JoinLobby(); // 방(서버)들의 목록을 관리하기 위해 로비에 접속하는 함수
        serverButton.interactable = true;
    }

    public override void OnJoinedLobby() // 로비 접속에 성공했을 경우 호출되는 함수
    {
        Debug.Log("Lobby 접속");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) // 로비에 방(서버)가 업데이트될 때마다 호출되는 함수
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.Name ==  roomName)
            {
                serverImage.color = Color.green;
                nextButton.interactable = true;
                serverButton.interactable = false;
            }
        }
    }

    public override void OnCreatedRoom() // 방(서버)생성을 요청 성공시 호출되는 함수
    {
        Debug.Log("MMORPG_Room 생성 완료");
        nextButton.interactable = false;
        serverImage.color = Color.green;
    }

    public override void OnCreateRoomFailed(short returnCode, string message) // 방(서버)생성을 요청 실패시 호출되는 함수
    {
        Debug.Log($"MMORPG_Room 생성 실패, {returnCode},  {message}");
    }
    
    
    public override void OnJoinedRoom() // 방(서버) 접속 성공시 호출되는 함수
    {
        PhotonNetwork.LoadLevel("Main");
        Debug.Log("Room 접속 성공 및 씬 전환");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) // 방(서버) 접속 실패시 호출되는 함수
    {
        Debug.Log($"MMORPG_Room 접속 실패, {returnCode},  {message}");
    }
}
