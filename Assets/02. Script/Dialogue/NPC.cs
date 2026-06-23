using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public CinemachineCamera cam; 
    public int npcID;
    public string npcName;
    public string talkGroupKey;
    
    public List<QuestData> questsToGive = new List<QuestData>();

    private bool isPlayerNearby = false;
    private Animator _anime;
    private static readonly int IsTalking = Animator.StringToHash("isTalking");
    
    private PhotonView PV;

    private QuestData _pendingQuest;

    private void Awake()
    {
        _anime = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
            Talk();
    }

    // 아직 받지도 완료하지도 않은, 줄 수 있는 첫 번째 퀘스트를 선택
    private QuestData GetNextQuestToGive()
    {
        foreach (var quest in questsToGive)
        {
            if (quest == null) continue;
            if (QuestManager.Instance.IsProgressingQuestByQuestID(quest.questID)) continue;
            if (QuestManager.Instance.completedQuestIDs.Contains(quest.questID)) continue;
            return quest;
        }
        return null;
    }

    private void Talk()
    {
        GameManager.Instance.SetGameState(Constants.EGameState.Interaction);
        _anime.SetBool(IsTalking, true);

        // 대화 종료 시 마무리(카메라 복귀/상태 복귀/메인 퀘스트 부여) 구독
        GameEvents.OnDialogueEnded += TalkEnd;

        // 퀘스트 완료
        if (QuestManager.Instance.IsCompleteQuestByNPCID(QuestType.Talk, npcID, out var key))
        {
            GameEvents.OnDialogueRequested?.Invoke(key);
            GameEvents.OnQuestProgressUpdated?.Invoke(QuestType.Talk, npcID, 1);
            return;
        }

        // 퀘스트 주기 (진행도에 따라 줄 수 있는 다음 퀘스트 선택)
        _pendingQuest = GetNextQuestToGive();
        if (_pendingQuest != null)
        {
            // 비메인 퀘스트만 수락 버튼 이벤트로 처리.
            // 메인 퀘스트는 대화 종료 시 TalkEnd 에서 직접 GiveQuest 를 호출(순서 보장).
            if (!_pendingQuest.isMain)
                GameEvents.OnAcceptActionTriggered += GiveQuest;

            GameEvents.OnDialogueRequested?.Invoke(_pendingQuest.CurrentTask.questGiveTalkKey);
            return;
        }
        
        // 일반 대화
        GameEvents.OnDialogueRequested?.Invoke(talkGroupKey);
    }

    private void GiveQuest()
    {
        if (isPlayerNearby && _pendingQuest != null)
            GameEvents.OnQuestAccepted?.Invoke(_pendingQuest);

        // 한 번 처리했으면 스스로 구독 해제하고 정리
        GameEvents.OnDialogueEnded -= GiveQuest;
        GameEvents.OnAcceptActionTriggered -= GiveQuest;
        _pendingQuest = null;
    }

    private void TalkEnd()
    {
        GameEvents.OnCurrentCameraChanged?.Invoke();
        GameManager.Instance.SetGameState(Constants.EGameState.Play);

        // 메인 퀘스트는 대화가 끝나면 즉시 부여 (GiveQuest 가 내부에서 _pendingQuest 정리)
        if (_pendingQuest != null && _pendingQuest.isMain)
        {
            GiveQuest();
        }
        else
        {
            // 비메인 퀘스트를 수락하지 않고 대화를 끝낸 경우 등: 누수 방지 정리
            GameEvents.OnAcceptActionTriggered -= GiveQuest;
            _pendingQuest = null;
        }

        _anime.SetBool(IsTalking, false);
        GameEvents.OnDialogueEnded -= TalkEnd;
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView targetPV = other.GetComponent<PhotonView>();
        if (targetPV != null && targetPV.IsMine && other.CompareTag("Player"))
            isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView targetPV = other.GetComponent<PhotonView>();
        if (targetPV != null && targetPV.IsMine && other.CompareTag("Player"))
            isPlayerNearby = false;
    }
}
