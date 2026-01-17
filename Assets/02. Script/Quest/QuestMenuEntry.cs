using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenuEntry : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statusText;
    public Toggle pinToggle;

    private QuestData _quest;

    public void Setup(QuestData quest)
    {
        _quest = quest;
        titleText.text = quest.title;
        statusText.text = quest.IsCompleted ? "완료됨" : $"{quest.currentAmount}/{quest.targetAmount}";
        
        pinToggle.SetIsOnWithoutNotify(QuestManager.Instance.pinnedQuests.Contains(quest));
        
        pinToggle.onValueChanged.RemoveAllListeners();
        pinToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        GameEvents.OnQuestPinChanged?.Invoke(_quest, isOn);
    }
}
