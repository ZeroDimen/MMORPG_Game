using System.IO;
using Photon.Pun;
using UnityEngine;

public class SaveManager : MonoBehaviourPunCallbacks
{
    public static SaveManager Instance { get; private set; }
    private InventoryModel _inventoryModel;
    // [SerializeField] private SimplePlayer simplePlayer;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void Init(InventoryModel model)
    {
        _inventoryModel = model;
    }

    // 플레이어별 고유 저장 경로 생성 (방장 컴퓨터의 경로)
    private string GetSavePath(string playerName)
    {
        return Path.Combine(Application.persistentDataPath, $"{playerName}_save.json");
    }
    
    #region Save Logic (Client -> Master)
    // [중앙 통제실] 클라이언트가 자신의 데이터를 모아 방장에게 저장을 요청함
    public void SaveGameToMaster()
    {
        if (!PhotonNetwork.InRoom) return;

        // 1. 저장할 데이터 바구니 생성 및 정보 수집
        CharacterSaveData data = new CharacterSaveData();
        data.playerName = PhotonNetwork.LocalPlayer.NickName;
        // data.level = simplePlayer.level;
        // data.gold = simplePlayer.gold;
        // data.pos = simplePlayer.transform.position;

        
        if (_inventoryModel != null)
            data.inventoryItems = _inventoryModel.GetSaveData();

        // 2. JSON 직렬화
        string json = JsonUtility.ToJson(data, true);

        // 3. 방장(MasterClient)에게만 RPC 전송
        photonView.RPC("RPC_SaveOnMaster", RpcTarget.MasterClient, data.playerName, json);
        Debug.Log($"[Client] 방장에게 저장 요청을 보냈습니다: {data.playerName}");
    }

    [PunRPC]
    private void RPC_SaveOnMaster(string playerName, string json)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        // 방장 컴퓨터의 로컬 폴더에 플레이어 이름별로 저장
        string path = GetSavePath(playerName);
        File.WriteAllText(path, json);
        Debug.Log($"[Master] {playerName}의 데이터를 저장 완료: {path}");
    }
    #endregion
    
    #region Load Logic (Master -> Client)
    
    // [클라이언트가 호출] 게임 시작 시 혹은 복구 시 방장에게 데이터를 달라고 요청함
    public void LoadGameFromMaster()
    {
        if (!PhotonNetwork.InRoom) return;

        string myName = PhotonNetwork.LocalPlayer.NickName;
        photonView.RPC("RPC_RequestLoadData", RpcTarget.MasterClient, myName);
        Debug.Log($"[Client] 방장에게 데이터 로드 요청 중: {myName}");
    }

    [PunRPC]
    private void RPC_RequestLoadData(string playerName)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        string path = GetSavePath(playerName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            // 데이터를 요청한 클라이언트에게만 응답을 보냄 (RpcTarget.Others로 쏘고 닉네임 필터링 혹은 타겟 지정)
            photonView.RPC("RPC_ReceiveLoadData", RpcTarget.Others, playerName, json);
        }
        else
        {
            Debug.LogWarning($"[Master] {playerName}의 저장 파일을 찾을 수 없음.");
        }
    }

    [PunRPC]
    private void RPC_ReceiveLoadData(string targetName, string json)
    {
        // 내 닉네임과 일치하는 응답만 처리
        if (PhotonNetwork.LocalPlayer.NickName != targetName) return;

        // 1. 데이터 역직렬화
        CharacterSaveData data = JsonUtility.FromJson<CharacterSaveData>(json);

        // 2. 실제 게임 시스템에 데이터 적용 (기존 LoadGame의 역할)
        if (data != null)
        {
            // 예: 플레이어 위치 복구
            // simplePlayer.transform.position = data.pos;
            
            // 인벤토리 복구
            if (_inventoryModel != null)
            {
                _inventoryModel.LoadData(data.inventoryItems);
            }

            Debug.Log($"[Client] 방장으로부터 데이터를 받아 복구 완료: {targetName}");
        }
    }
    #endregion
}
