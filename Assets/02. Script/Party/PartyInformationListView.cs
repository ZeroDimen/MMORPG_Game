using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyInformationListView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickName;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private Button status;

    public void MemberView(string memberName)
    {
        nickName.text = memberName;
    }
}
