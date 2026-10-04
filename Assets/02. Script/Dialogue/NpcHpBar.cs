using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using WebSocketSharp;

public class NpcHpBar : MonoBehaviour
{
    private NPC npc;
    [SerializeField] private RectTransform nameRectTransform;
    [SerializeField] private TextMeshProUGUI nameText;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
        npc = GetComponent<NPC>();
        if (!npc.npcName.IsNullOrEmpty())
            SetName(npc.npcName);
    }

    private void LateUpdate()
    {
        if (_cam == null)
        {
            _cam = Camera.main;
            if (_cam == null) return;
        }

        nameText.transform.forward = _cam.transform.forward;
    }
    
    public void SetName(string npcName)
    {
        nameText.text = npcName;
    }
}
