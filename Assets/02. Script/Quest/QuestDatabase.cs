using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quest/Database")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestData> allQuests;

    public QuestData GetQuestByID(int id)
    {
        if (allQuests == null) return null;
        return allQuests.Find(q => q.questID == id);
    }
}
