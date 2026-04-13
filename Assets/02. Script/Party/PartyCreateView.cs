using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        gameObject.SetActive(false);
    }

    private JoinType GetJoinType()
    {
        return instantToggle.isOn ? JoinType.Instant : JoinType.Request;
    }
}
