using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI contextText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    public event Action OnNextClick;

    private void Awake()
    {
        nextButton.onClick.AddListener(() => OnNextClick?.Invoke());
        acceptButton.onClick.AddListener(AcceptButton);
        declineButton.onClick.AddListener(DeclineButton);
    }

    public void UpdateUI(string speaker, string context)
    {
        speakerText.text = speaker;
        contextText.text = context;
    }

    public void ShowQuestButton(bool isMain)
    {
        if (isMain) return;
            
        nextButton.gameObject.SetActive(false);
        acceptButton.gameObject.SetActive(true);
        declineButton.gameObject.SetActive(true);
    }

    private void AcceptButton()
    {
        GameEvents.OnAcceptActionTriggered?.Invoke();
        
        nextButton.gameObject.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        OnNextClick?.Invoke();
    }

    private void DeclineButton()
    {
        nextButton.gameObject.SetActive(true);
        acceptButton.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        OnNextClick?.Invoke();
    }

    public void Show(bool isShow) => gameObject.SetActive(isShow);
}
