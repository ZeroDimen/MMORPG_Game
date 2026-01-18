using System;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    public int npcID;
    public string npcName;
    public string talkGroupKey;
    public QuestData questToGive;
    
    private bool isPlayerNearby = false;
    private Animator _anime;
    private static readonly int IsTalking = Animator.StringToHash("isTalking");

    private void Awake()
    {
        _anime = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
            Talk();
    }

    private void Talk()
    {
        cam.SetActive(true);
        GameEvents.OnDialogueEnded += TalkEnd;
        _anime.SetBool(IsTalking, true);
        GameEvents.OnDialogueRequested?.Invoke(talkGroupKey);

        if (questToGive != null)
        {
            if (questToGive.isMain)
                GameEvents.OnDialogueEnded += GiveQuest;
            else
                GameEvents.OnAcceptActionTriggered += GiveQuest;
        }
        GameEvents.OnQuestProgressUpdated?.Invoke(QuestType.Talk, npcID, 1);
    }

    private void GiveQuest()
    {
        GameEvents.OnQuestAccepted?.Invoke(questToGive);
    }

    private void TalkEnd()
    {
        cam.SetActive(false);
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
        if (other.CompareTag("Player"))
            isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNearby = false;
    }
}
