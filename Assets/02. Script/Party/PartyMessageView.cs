using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMessageView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI context;
    [SerializeField] private Button confirm;

    private void Start()
    {
        confirm.onClick.AddListener(() => Destroy(gameObject));
    }

    public void ViewText(string text)
    {
        context.text = text;
    }
}
