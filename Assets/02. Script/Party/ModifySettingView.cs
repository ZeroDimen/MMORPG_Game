using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifySettingView : MonoBehaviour
{
    [SerializeField] private TMP_InputField title;
    [SerializeField] private Toggle instantToggle;
    [SerializeField] private Toggle requestToggle;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Party party;
    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmButton);
        cancelButton.onClick.AddListener(OnCancelButton);
        
        title.onSelect.AddListener(OnFieldSelect);
        title.onDeselect.AddListener(OnFieldDeselect);
    }

    private void OnEnable()
    {
        party = PartySystem.instance.MyParty;
        
        title.text = party._title;

        var isInstant = party._joinType == JoinType.Instant;
        
        instantToggle.SetIsOnWithoutNotify(isInstant);
        requestToggle.SetIsOnWithoutNotify(!isInstant);
    }

    private void OnConfirmButton()
    {
        if (string.IsNullOrEmpty(title.text)) return;
        var type = GetJoinType();
        party._title = title.text;
        party._joinType = type;
        PartySystem.instance.ModifySetting();
        OnCancelButton();
    }

    private void OnCancelButton()
    {
        title.text = "";
        
        if (EventSystem.current.currentSelectedGameObject == title.gameObject)
            EventSystem.current.SetSelectedGameObject(null);
        
        GameManager.Instance.PopState(Constants.EGameState.Interaction);
        
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        title.onSelect.RemoveListener(OnFieldSelect);
        title.onDeselect.RemoveListener(OnFieldDeselect);
    }

    private JoinType GetJoinType()
    {
        return instantToggle.isOn ? JoinType.Instant : JoinType.Request;
    }
    
    private void OnFieldSelect(string _) => GameManager.Instance.PushState(Constants.EGameState.TextInput);
    private void OnFieldDeselect(string _) => GameManager.Instance.PopState(Constants.EGameState.TextInput);
}
