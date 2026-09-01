using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class DungeonSystem : MonoBehaviourPunCallbacks
{
    public static DungeonSystem instance;
    private PhotonView pv;
    private List<Party> dungeonPartyList;
    [SerializeField] private DungeonLight dungeonLight;
    [SerializeField] private Image fadePanel;

    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private GameObject dungeonPanel;

    [SerializeField] private GameObject elevator;
    public const int MonsterNum = 3;
    private bool _hasPlayed = false;
    private Dictionary<string, int> partyKillCount;
    [SerializeField] private Transform fieldSpawnPos;
    [SerializeField] private GameObject exitDungeonButton;
    [SerializeField] private GameObject clearBanner; // "DUNGEON CLEAR!" 배너 UI (기본 비활성)
    [SerializeField] private CinemachineImpulseSource impulseSource; // 보스 등장 카메라 흔들림용 (미할당 시 흔들림만 생략)
    
    private HashSet<string> bossSpawnedParties = new HashSet<string>();
    
    public CinemachineBrain brain;
    public CinemachineCamera bossCam;
    public CinemachineCamera bossCam2;
    public CinemachineCamera bossCam3;

    [SerializeField] private Letterbox _letterbox;
    
    [Header("매복 컷씬")]
    [SerializeField] private Animator[] ambushDoors;      // 문 4개 (회전 애니메이션)
    [SerializeField] private Transform ambushCenterPoint; // 가운데 방 목적지
    [SerializeField] private float ambushDoorDelay = 0f;  // 문 순차 열림 간격
    [SerializeField] private float ambushChargeTime = 2.5f; // 몬스터 입장 대기시간
    [SerializeField] private AmbushRoom[] ambushRooms;    // 작은 방 4개 정보

    private HashSet<string> ambushedParties = new HashSet<string>();

    [System.Serializable]
    public class AmbushRoom
    {
        public Transform spawnPoint;   // 이 방 몬스터 스폰 위치
        public float spawnRadius = 1f; // 스폰 반경
        public int monsterCount = 2;   // 이 방에서 나올 몬스터 수
    }
    
    public BossController CurrentBoss { get; private set; }

    public Transform cheat;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        dungeonPartyList = new List<Party>();
        partyKillCount = new Dictionary<string, int>();
    }

    public void SendRpcToPartyMembers(Party party, string rpcName, params object[] parameters)
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            foreach (var member in party._member.Where(member => player.NickName == member))
                pv.RPC(rpcName, player, parameters);
        }
    }

    public void RegisterBoss(BossController boss)
    {
        CurrentBoss = boss;
    }

    public void UnregisterBoss(BossController boss)
    {
        if (CurrentBoss == boss) CurrentBoss = null;
    }
}
