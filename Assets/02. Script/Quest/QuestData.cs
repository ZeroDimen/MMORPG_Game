using UnityEngine;

// 퀘스트의 종류 정의
public enum QuestType { Gather, Talk, Kill }

[CreateAssetMenu(fileName = "Quest_", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public int questID;
    public string title;
    public bool isMain;
    [TextArea] public string description;

    // 핵심: 이제 "목표" 자체를 타입으로 가집니다.
    public QuestType questType;
    public int targetID;     // 아이템 ID 혹은 NPC ID
    public int targetAmount;  // 목표 개수 (대화 퀘스트라면 1)

    [Header("Reward")]
    public ItemData rewardItem;
    public int rewardAmount;

    // 현재 진행도를 저장할 변수 (실제 서비스에선 별도 클래스 권장)
    public int currentAmount;
    public bool IsCompleted => currentAmount >= targetAmount;
}
