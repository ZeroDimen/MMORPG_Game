using TMPro;
using UnityEngine;

public class QuestEntry : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;

    public void Setup(QuestData quest)
    {
        titleText.text = quest.title;
        contentText.text = quest.IsCompleted ? "<color=green>(완료)</color>" : quest.CurrentTask.Status;
    }
}