using System;
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
    public QuestData questToGive;
    
    private bool isPlayerNearby = false;
    private Animator _anime;
    private static readonly int IsTalking = Animator.StringToHash("isTalking");
    
    private PhotonView PV;

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

    private void Talk()
    {
        GameManager.Instance.SetGameState(Constants.EGameState.Interaction);
        cam.Priority = 11;
        GameEvents.OnDialogueEnded += TalkEnd;
        _anime.SetBool(IsTalking, true);

        // 퀘스트 완료
        if (QuestManager.Instance.IsCompleteQuestByNPCID(QuestType.Talk, npcID, out var key))
        {
            GameEvents.OnDialogueRequested?.Invoke(key);
            GameEvents.OnQuestProgressUpdated?.Invoke(QuestType.Talk, npcID, 1);
            return;
        }

        // 퀘스트 주기
        if (questToGive != null && !QuestManager.Instance.IsProgressingQuestByQuestID(questToGive.questID))
        {
            if (!QuestManager.Instance.completedQuestIDs.Contains(questToGive.questID))
            {
                if (questToGive.isMain)
                    GameEvents.OnDialogueEnded += GiveQuest;
                else
                    GameEvents.OnAcceptActionTriggered += GiveQuest;
            
                GameEvents.OnDialogueRequested?.Invoke(questToGive.CurrentTask.questGiveTalkKey);
                return;
            }
        }
        
        // 일반 대화
        GameEvents.OnDialogueRequested?.Invoke(talkGroupKey);
    }

    private void GiveQuest()
    {
        GameEvents.OnQuestAccepted?.Invoke(questToGive);
    }

    private void TalkEnd()
    {
        GameManager.Instance.SetGameState(Constants.EGameState.Play);
        cam.Priority = 0;
        if (questToGive)
        {
            GameEvents.OnDialogueEnded -= GiveQuest;
            GameEvents.OnAcceptActionTriggered -= GiveQuest;
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
