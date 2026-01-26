using System;
using JetBrains.Annotations;

public static class GameEvents
{
    public static Action<string> OnDialogueRequested;
    public static Action OnDialogueEnded;

    public static Action OnAcceptActionTriggered;

    public static Action<QuestType, int, int> OnQuestProgressUpdated;

    public static Action<QuestData> OnQuestAccepted;
    public static Action<QuestData> OnQuestCompleted;

    public static Action OnQuestListChanged;
    public static Action<QuestData, bool> OnQuestPinChanged;

    public static Action<InstanceItem> OnItemEquipped;
    public static Action<InstanceItem> OnItemUnEquipped;
    
    public static Action<PlayerStatus> OnStatusChanged;
}
