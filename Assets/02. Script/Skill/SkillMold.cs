using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class SkillMold : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject skillInfoObj;
    
    public TMP_Text skillInfoSkillName;
    public TMP_Text skillInfoSkillScript;
    public int skillCooltime;
    
    public Image skillIcon;
    public Image skillIconCooltime;

    private Vector3 skillInfoPos;

    public string skillName;
    public string skillScript;
    
    private void Start()
    {
        skillInfoPos = skillInfoObj.transform.localPosition;
    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        skillInfoObj.transform.localPosition = new Vector3(skillInfoPos.x + gameObject.transform.localPosition.x, skillInfoPos.y, skillInfoPos.z);

        skillInfoSkillName.text = skillName;
        skillInfoSkillScript.text = skillScript;
        
        skillInfoObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillInfoObj.SetActive(false);
    }
}
