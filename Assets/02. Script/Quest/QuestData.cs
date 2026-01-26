using System;
using System.Collections.Generic;
using UnityEngine;

// 퀘스트의 종류 정의
public enum QuestType { Gather, Talk, Kill }

[Serializable]
public class QuestTask
{
    public string teskDescription;
    public QuestType questType;
    public int targetID;
    public int taskID;
    public int targetAmount;
    public int currentAmount;
    public string questGiveTalkKey;
    public string questCompleteTalkKey;
    public bool IsTaskCompleted => currentAmount >= targetAmount;
    public string Status
    {
        get
        {
            if (questType == QuestType.Talk)
                return teskDescription;
            return teskDescription + $"{currentAmount} / {targetAmount}";
        }
    }
}

[CreateAssetMenu(fileName = "Quest_", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public int questID;
    public string title;
    public bool isMain;
    public bool isSequential;
    [TextArea] public string description;

    public List<QuestTask> questTasks = new List<QuestTask>();

    [Header("Reward")]
    public ItemData rewardItem;
    public int rewardAmount;

    public bool IsCompleted
    {
        get
        {
            foreach (var task in questTasks)
            {
                if (!task.IsTaskCompleted) return false;
            }
            return true;
        }
    }

    public QuestTask CurrentTask
    {
        get
        {
            foreach (var task in questTasks)
            {
                if(task.IsTaskCompleted) continue;
                return task;
            }
            return null;
        }
    }

    public int CurrentTaskID => CurrentTask.taskID;
}
