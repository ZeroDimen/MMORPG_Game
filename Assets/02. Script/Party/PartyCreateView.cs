using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Constants;

public class PartyCreateView : MonoBehaviour
{
    [SerializeField] private TMP_InputField title;
    [SerializeField] private Toggle instantToggle;
    [SerializeField] private Toggle requestToggle;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmButton);
        cancelButton.onClick.AddListener(OnCancelButton);
        
        title.onSelect.AddListener(OnFieldSelect);
        title.onDeselect.AddListener(OnFieldDeselect);

        instantToggle.isOn = true;
        requestToggle.isOn = false;
    }

    private void OnConfirmButton()
    {
        if (string.IsNullOrEmpty(title.text)) return;
        var type = GetJoinType();
        PartySystem.instance.RequestCreateParty(title.text, PhotonNetwork.LocalPlayer.NickName, type);
        OnCancelButton();
    }

    private void OnCancelButton()
    {
        title.text = "";
        instantToggle.isOn = true;
        requestToggle.isOn = false;
        
        if (EventSystem.current.currentSelectedGameObject == title.gameObject)
            EventSystem.current.SetSelectedGameObject(null);
        
        GameManager.Instance.PopState(EGameState.Interaction);
        
        gameObject.SetActive(false);
    }

    private JoinType GetJoinType()
    {
        return instantToggle.isOn ? JoinType.Instant : JoinType.Request;
    }

    private void OnDestroy()
    {
        title.onSelect.RemoveListener(OnFieldSelect);
        title.onDeselect.RemoveListener(OnFieldDeselect);
    }

    private void OnFieldSelect(string _) => GameManager.Instance.PushState(Constants.EGameState.TextInput);
    private void OnFieldDeselect(string _) => GameManager.Instance.PopState(Constants.EGameState.TextInput);
}
