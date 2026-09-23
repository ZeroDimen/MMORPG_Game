using System;
using System.Collections;
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
    [SerializeField] private Light hangingCageLight; // Dungeon/Room4/Room4 Obj/Hanging_Cage/Point Light
    [SerializeField] private float fadeInDuration = 2f; // 조명 복귀 시 서서히 밝아지는 시간
    private float _originalIntensity;
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
    private Dictionary<string, int> ambushKillCount = new Dictionary<string, int>();
    private const int AmbushMonsterNum = 4;

    public InteractableDoor bossFrontDoor;
    public InteractableDoor bossDoor;
    
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

        if (hangingCageLight != null)
            _originalIntensity = hangingCageLight.intensity;
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

public void SetHangingCageDark()
    {
        pv.RPC(nameof(RPC_SetHangingCageDark), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_SetHangingCageDark()
    {
        if (hangingCageLight != null)
            hangingCageLight.enabled = false;
    }

    public void RestoreHangingCageLight(Action onComplete = null)
    {
        pv.RPC(nameof(RPC_RestoreHangingCageLight), RpcTarget.All);
        onComplete?.Invoke(); // 페이드인이 끝나는 시점이 아니라, 불이 켜지기 '시작'하는 시점에 즉시 호출
    }

    [PunRPC]
    private void RPC_RestoreHangingCageLight()
    {
        StartCoroutine(HangingCageFadeInRoutine());
    }

    private IEnumerator HangingCageFadeInRoutine()
    {
        if (hangingCageLight == null) yield break;

        hangingCageLight.enabled = true;
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            hangingCageLight.intensity = Mathf.Lerp(0f, _originalIntensity, elapsed / fadeInDuration);
            yield return null;
        }
        hangingCageLight.intensity = _originalIntensity;
    }

    private IEnumerator InvokeAfter(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    public void TriggerCameraShake()
    {
        pv.RPC(nameof(RPC_TriggerCameraShake), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_TriggerCameraShake()
    {
        if (impulseSource != null)
            impulseSource.GenerateImpulse(0.5f); // 흔들림 세기 절반
    }




}
