using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI contextText;
    [SerializeField] private Button nextButton;

    public event Action OnNextClick;

    private void Awake()
    {
        nextButton.onClick.AddListener(() => OnNextClick?.Invoke());
    }

    public void UpdateUI(string speaker, string context)
    {
        speakerText.text = speaker;
        contextText.text = context;
    }

    public void Show(bool isShow) => gameObject.SetActive(isShow);
}
