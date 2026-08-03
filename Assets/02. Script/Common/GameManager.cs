using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine.UI;
using static Constants;
using Random = UnityEngine.Random;

public class GameManager :  MonoBehaviourPun
{
    public static GameObject LocalPlayer;
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
    
    public List<PhotonView> playerList;

    private readonly List<EGameState> _stateStack = new();
    public EGameState GameState =>_stateStack.Count > 0 ? _stateStack[^1] : EGameState.Play;
    private EGameState _lastApplied = EGameState.Play;

    public static GameManager Instance => instance;
    
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
        // Set_Spawner("Boss");
    }

    public void Set_Spawner(string prefabName)
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string Name = PhotonNetwork.LocalPlayer.NickName;
        photonView.RPC(nameof(RPC_Spawner), RpcTarget.MasterClient,prefabName ,Name, actorNumber);
    }

    public void HitEnemy(PhotonView enemyView,PhotonView playerView, int damage)
    {
        photonView.RPC(nameof(RPC_RequestEnemyDamage), RpcTarget.All,enemyView.ViewID, playerView.ViewID, damage);
    }

    public void HitPlayer(PhotonView playerView, int damage)
    {
        photonView.RPC(nameof(RPC_RequestPlayerDamage), RpcTarget.All,playerView.ViewID, damage);
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
            
            if (!photonView.IsMine) return;
            // 보스 스폰 시 partyId(파티장 이름)가 반드시 세팅돼 있어야 클리어 처리가 동작한다.
            if(!string.IsNullOrEmpty(enemyController.partyId))
            {
                if (enemyController is BossController)
                    DungeonSystem.instance.OnBossDefeated(enemyController.partyId); // 보스 전용: 던전 클리어
                else
                    DungeonSystem.instance.KillMonster(enemyController.partyId);     // 일반 몹: 엘리베이터 카운트
            }
            else
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
                
                string name = $"(Maria)_{objName}";
                photonView.RPC(nameof(RPC_SetName), RpcTarget.AllBuffered, pv.ViewID, name);
                
                pv.TransferOwnership(actorNumber);
                playerList.Add(pv);
                break;
            case "Mutant":
                spownPos = GetRandomPosition(spawnPoints[1].point, spawnPoints[1].radius);
                PhotonNetwork.Instantiate("Mutant", spownPos, Quaternion.identity);
                break;
            case "Boss":
                spownPos = GetRandomPosition(spawnPoints[3].point, spawnPoints[3].radius);
                var boss = PhotonNetwork.Instantiate("Boss", spownPos, Quaternion.identity);
                boss.GetComponent<EnemyController>().partyId = objName; // 보스 처치 → 클리어 감지의 핵심
                break;
            default:
                Debug.Log("파일 없음");
                break;
        }
    }

    public void SpawnMonsterInDungeon(int index, string manager)
    {
        var spawnPos = GetRandomPosition(spawnPoints[index].point, spawnPoints[index].radius);
        GameObject monster = PhotonNetwork.Instantiate("Mutant", spawnPos, Quaternion.identity);
        monster.GetComponent<EnemyController>().partyId = manager;
    }
    
    public void SpawnBossInDungeon(string manager)
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string Name = PhotonNetwork.LocalPlayer.NickName;
        photonView.RPC(nameof(RPC_Spawner), RpcTarget.MasterClient,"Boss" ,Name, actorNumber);
    }

    Vector3 GetRandomPosition(Transform point, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        return point.position + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    public void PushState(EGameState state)
    {
        if (state == EGameState.Play) return;
        _stateStack.Add(state);
        RefreshState();
    }

    public void PopState(EGameState state)
    {
        int idx = _stateStack.LastIndexOf(state);
        if (idx < 0) return;
        _stateStack.RemoveAt(idx);
        RefreshState();
    }

    private void RefreshState()
    {
        var current = GameState;
        ApplyStateEffects(current);
        
        if(PhotonNetwork.LocalPlayer?.TagObject is PlayerController pc)
            pc.SetPlayerInputEnabled(current == EGameState.Play);
    }
    
    public void ResetToPlay()
    {
        _stateStack.Clear();
        RefreshState();
    }

    private void ApplyStateEffects(EGameState state)
    {
        bool isPlay = state == EGameState.Play;
        bool wasPlay = _lastApplied == EGameState.Play;
        bool isCutscene = state == EGameState.Cutscene;

        bool hideCursor = isPlay || isCutscene;
        Cursor.visible = !hideCursor;
        Cursor.lockState = hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
        
        AudioManager._instance.BgmVolume(isPlay ? 1f : 0.3f);
        
        if (isPlay && !wasPlay)
        {
            cinemachineOrbitalFollow.HorizontalAxis.Range = cinemachineObitalHRange;
            cinemachineOrbitalFollow.VerticalAxis.Range   = cinemachineObitalVRange;
        }
        else if (!isPlay && wasPlay)
        {
            float h = cinemachineOrbitalFollow.HorizontalAxis.Value;
            float v = cinemachineOrbitalFollow.VerticalAxis.Value;
            cinemachineOrbitalFollow.HorizontalAxis.Range = new Vector2(h, h);
            cinemachineOrbitalFollow.VerticalAxis.Range   = new Vector2(v, v);
        }

        _lastApplied = state;
    }
    
    public void SetChattingInputField()
    {
        bool willOpen = !chattingInputField.activeSelf;
        chattingInputField.SetActive(willOpen);
        
        if(willOpen) PushState(EGameState.TextInput);
        else PopState(EGameState.TextInput);
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

    [PunRPC]
    private void RPC_SetName(int viewID , string name)
    {
        PhotonView pv = PhotonView.Find(viewID);

        if (pv != null)
        {
            pv.gameObject.name = name;
        }
    }
    
}