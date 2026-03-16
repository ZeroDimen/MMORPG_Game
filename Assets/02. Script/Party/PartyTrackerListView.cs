using TMPro;
using UnityEngine;

public class PartyTrackerListView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nickName;

    public void ViewMember(string member)
    {
        nickName.text = member;
    }
}
