using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuestMenuEntry : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public Toggle pinToggle;

    private QuestData _quest;

    public void Setup(QuestData quest)
    {
        _quest = quest;
        titleText.text = quest.title;
        string status = quest.IsCompleted ? "완료됨" : $"{quest.currentAmount}/{quest.targetAmount}";
        contentText.text = $"{quest.description} : {status}";
        
        pinToggle.SetIsOnWithoutNotify(QuestManager.Instance.pinnedQuests.Contains(quest));
        
        pinToggle.onValueChanged.RemoveAllListeners();
        pinToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        GameEvents.OnQuestPinChanged?.Invoke(_quest, isOn);
    }
}
