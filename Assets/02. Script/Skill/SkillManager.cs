using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private SkillData[] skillDatas;
    
    [SerializeField]
    private GameObject Skill_mold;

    private SkillMold[] skillmoldObj;


    private void Start()
    {
        skillmoldObj = new SkillMold[skillDatas.Length];
        
        for (int i = 0; i < skillDatas.Length; i++) // 수정필요 skillData 수 많큼 출력
        {
            GameObject moldObj = Instantiate(Skill_mold, Skill_mold.transform.parent);
            skillmoldObj[i] = moldObj.GetComponent<SkillMold>();
            
            skillmoldObj[i].skillIcon.sprite = skillDatas[i].skillIcon;
            skillmoldObj[i].skillName = skillDatas[i].skillName;
            skillmoldObj[i].skillScript = skillDatas[i].skillDescription;
            skillmoldObj[i].skillCooltime = skillDatas[i].skillCooldown;
            
            moldObj.SetActive(true);
        }
    }

    public void StartCooltime(int num)
    {
        StartCoroutine(Cooltime(num));
    }

    public IEnumerator Cooltime(int num)
    {
        float time = 0f;
        while (time < skillmoldObj[num].skillCooltime)
        {
            skillmoldObj[num].skillIconCooltime.fillAmount = Mathf.Lerp(1f, 0f, time / skillmoldObj[num].skillCooltime);
            time += Time.deltaTime;
            yield return null;
        }
        
        skillmoldObj[num].skillIconCooltime.fillAmount = 0f;
    }
}