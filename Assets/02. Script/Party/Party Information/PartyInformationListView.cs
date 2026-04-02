using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PartyInformationListView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private Button information;
    [SerializeField] private Button delegateButton;

    public GameObject memberStatusPanel;
    private string member;
    private void Start()
    {
        information.onClick.AddListener(OnInformationButton);
    }

    private void OnEnable()
    {
        if (PartySystem.instance.MyParty != null && PartySystem.instance.MyParty._manager == PhotonNetwork.NickName)
        {
            delegateButton.gameObject.SetActive(true);
            delegateButton.onClick.RemoveAllListeners();
            delegateButton.onClick.AddListener(DelegateManager);
        }
    }

    public void MemberView(string memberName)
    {
        nickName.text = memberName;
        member = memberName;

        if (PartySystem.instance.MyParty._manager == PhotonNetwork.NickName && memberName == PhotonNetwork.NickName)
            delegateButton.gameObject.SetActive(false);
    }

    private void OnInformationButton()
    {
        memberStatusPanel.SetActive(true);
        var info = memberStatusPanel.GetComponent<PartyMemberInformationView>();
        info.UpdateUI(member);
    }

    private void DelegateManager()
    {
        PartySystem.instance.DelegateManager(member);
    }
}
