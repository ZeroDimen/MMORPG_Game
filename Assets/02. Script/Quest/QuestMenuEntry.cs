using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuestMenuEntry : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI goalText;
    public Toggle pinToggle;

    private QuestData _quest;

    public void Setup(QuestData quest)
    {
        _quest = quest;
        titleText.text = quest.title;
        descriptionText.text = quest.description;
        goalText.text = quest.IsCompleted ? "<color=green>(완료)</color>" : quest.CurrentTask.Status;

        
        pinToggle.SetIsOnWithoutNotify(QuestManager.Instance.pinnedQuests.Contains(quest));
        
        pinToggle.onValueChanged.RemoveAllListeners();
        pinToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        GameEvents.OnQuestPinChanged?.Invoke(_quest, isOn);
    }
}
