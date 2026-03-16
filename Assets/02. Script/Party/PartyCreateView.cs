using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyCreateView : MonoBehaviour
{
    [SerializeField] private TMP_InputField title;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmButton);
        cancelButton.onClick.AddListener(OnCancelButton);
    }

    private void OnConfirmButton()
    {
        if (string.IsNullOrEmpty(title.text)) return;
        PartySystem.instance.RequestCreateParty(title.text, PhotonNetwork.LocalPlayer.NickName);
        OnCancelButton();
    }

    private void OnCancelButton()
    {
        title.text = "";
        gameObject.SetActive(false);
    }
}
