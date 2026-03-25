using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PartyInformationListView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private Button information;

    public GameObject memberStatusPanel;
    private string member;
    private void Start()
    {
        information.onClick.AddListener(OnInformationButton);
    }

    public void MemberView(string memberName)
    {
        nickName.text = memberName;
        member = memberName;
    }

    private void OnInformationButton()
    {
        memberStatusPanel.SetActive(true);
        var info = memberStatusPanel.GetComponent<PartyMemberInformationView>();
        info.UpdateUI(member);
    }
}
