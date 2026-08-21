using System;
using JetBrains.Annotations;

public static class GameEvents
{
    public static Action<string> OnDialogueRequested;
    public static Action OnDialogueEnded;

    public static Action OnAcceptActionTriggered;

    // Quest
    public static Action<QuestType, int, int> OnQuestProgressUpdated;

    public static Action<QuestData> OnQuestAccepted;
    public static Action<QuestData> OnQuestCompleted;

    public static Action OnQuestListChanged;
    public static Action<QuestData, bool> OnQuestPinChanged;

    // Inventory
    public static Action<InstanceItem> OnItemEquipped;
    public static Action<InstanceItem> OnItemUnEquipped;
    
    // Status
    public static Action<PlayerStatus> OnStatusChanged;
    public static Action<PlayerStatus> OnPlayerHpChanged;

    // Camera
    public static Action<int> OnCameraChanged;
    public static Action OnCurrentCameraChanged;

    // Player
    public static Action OnPlayerLevelUpEvent;
}