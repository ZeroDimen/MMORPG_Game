using System.Collections;
using UnityEngine;
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine.UI;
using static Constants;

public class GameManager :  MonoBehaviourPun
{
    private static GameManager instance;
    private bool _isCursorLock;
    [SerializeField] private SpawnZone[] spawnPoints;
    [SerializeField] private GameObject chattingInputField;
    [SerializeField] private GameObject playerCam;

    private CinemachineCamera cinemachineCamera;
    private CinemachineOrbitalFollow cinemachineOrbitalFollow;

    private Vector2 cinemachineObitalHRange;
    private Vector2 cinemachineObitalVRange;
    
    public Canvas Canvas => GetCanvas();
    
    public EGameState GameState { get; private set; }
    
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = new GameManager();
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        Application.targetFrameRate = 120;
        yield return null;
        
        cinemachineCamera = playerCam.GetComponent<CinemachineCamera>();
        cinemachineOrbitalFollow = playerCam.GetComponent<CinemachineOrbitalFollow>();

        cinemachineObitalHRange = cinemachineOrbitalFollow.HorizontalAxis.Range;
        cinemachineObitalVRange = cinemachineOrbitalFollow.VerticalAxis.Range;
        
        AudioManager._instance.BgmPlay("Forest");
        if (PhotonNetwork.IsMasterClient)
        {
            cinemachineCamera.enabled = false;
            yield break;
        }
        Set_Spawner("Maria");
    }

    public void Set_Spawner(string prefabName)
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string Name = PhotonNetwork.LocalPlayer.NickName;
        photonView.RPC("RPC_Spawner", RpcTarget.MasterClient,prefabName ,Name, actorNumber);
    }

    public void HitEnemy(PhotonView enemyView,PhotonView playerView, int damage)
    {
        photonView.RPC("RPC_RequestEnemyDamage", RpcTarget.All,enemyView.ViewID, playerView.ViewID, damage);
    }

    public void HitPlayer(PhotonView playerView, int damage)
    {
        photonView.RPC("RPC_RequestPlayerDamage", RpcTarget.All,playerView.ViewID, damage);
    }

   
    [PunRPC]
    private void RPC_RequestPlayerDamage(int playerView, int damage)
    {
        PhotonView playerPV = PhotonView.Find(playerView);
        PlayerController playerController = playerPV.GetComponent<PlayerController>();
        playerController.SetHit(damage);
    }
    
    [PunRPC]
    private void RPC_RequestEnemyDamage(int enemyView, int playerView,  int damage, PhotonMessageInfo info)
    {
        PhotonView enemyPV = PhotonView.Find(enemyView);
        
        EnemyController enemyController = enemyPV.GetComponent<EnemyController>();
        int exp = enemyController.SetHit(damage);

        if (exp > 0)
        {
            PhotonView playerPV = PhotonView.Find(playerView);
            PlayerController playerController = playerPV.GetComponent<PlayerController>();
            playerController.SetExp(exp);
            photonView.RPC("RPC_MonsterKillQuest", info.Sender);
        }
    }

    [PunRPC]
    private void RPC_MonsterKillQuest()
    {
        QuestManager.Instance.HandleProgressUpdate(QuestType.Kill, 0, 1);
    }
    
    [PunRPC]
    private void RPC_Spawner(string prefabName, string objName, int actorNumber)
    {
        Vector3 spownPos;
        switch (prefabName)
        {
            case "Maria":
                GameObject obj = null;
                
                spownPos = GetRandomPosition(spawnPoints[0].point, spawnPoints[0].radius);
                PhotonNetwork.NickName = objName;
                obj = PhotonNetwork.Instantiate("Maria", spownPos , Quaternion.identity);
                
                PhotonView pv =  obj.GetComponent<PhotonView>();
                pv.TransferOwnership(actorNumber);
                break;
            case "Mutant":
                spownPos = GetRandomPosition(spawnPoints[1].point, spawnPoints[1].radius);
                PhotonNetwork.Instantiate("Mutant", spownPos, Quaternion.identity);
                break;
            default:
                Debug.Log("파일 없음");
                break;
        }
    }

    Vector3 GetRandomPosition(Transform point, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return point.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    public void SetGameState(EGameState state)
    {
        if (state == EGameState.Interaction || state == EGameState.Alt)
        {
            Cursor.visible  = true;
            Cursor.lockState = CursorLockMode.None;

            float currentH = cinemachineOrbitalFollow.HorizontalAxis.Value; 
            float currentV = cinemachineOrbitalFollow.VerticalAxis.Value;

            cinemachineOrbitalFollow.HorizontalAxis.Range = new Vector2(currentH, currentH);
            cinemachineOrbitalFollow.VerticalAxis.Range = new Vector2(currentV, currentV);
            
            AudioManager._instance.BgmVolume(0.3f);
        }
        else if (state == EGameState.Play)
        {
            Cursor.visible  = false;
            Cursor.lockState = CursorLockMode.Locked;

            cinemachineOrbitalFollow.HorizontalAxis.Range = cinemachineObitalHRange;
            cinemachineOrbitalFollow.VerticalAxis.Range = cinemachineObitalVRange;
            
            AudioManager._instance.BgmVolume(1f);
        }

        GameState = state;
        if (PhotonNetwork.LocalPlayer?.TagObject is PlayerController pc)
        {
            pc.SetPlayerInputEnabled(state == EGameState.Play);
        }
    }
    
    public void SetChattingInputField()
    {
        chattingInputField.SetActive(!chattingInputField.activeSelf);
        if (chattingInputField.activeSelf)
        {
            SetGameState(EGameState.Interaction);
        }
        else
        {
            SetGameState(EGameState.Play);
        }
    }

    
    private Canvas GetCanvas()
    {
        var canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        Canvas result = null;

        if (!canvasObject)
        {
            canvasObject = new GameObject("Canvas");
            canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            
            result = canvasObject.GetComponent<Canvas>();
            result.renderMode = RenderMode.ScreenSpaceOverlay;
            result.tag = "Canvas";
        }
        else
        {
            result = canvasObject.GetComponent<Canvas>();
        }

        return result;
    }
    
}