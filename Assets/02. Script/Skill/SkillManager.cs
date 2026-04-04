using System.Collections;
using UnityEngine;

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
            
            moldObj.SetActive(true);
        }
    }

    public void SetSkillData(int LV = 1)
    {
        for (int i = 0; i < skillDatas.Length; i++) // 수정필요 skillData 수 많큼 출력
        {
            skillmoldObj[i].skillIcon.sprite = skillDatas[i].skillIcon;
            skillmoldObj[i].skillName = skillDatas[i].skillName;
            skillmoldObj[i].skillCooltime = skillDatas[i].skillCooltime;
            skillmoldObj[i].skillDamage = skillDatas[i].skillBaseDamage + skillDatas[i].skillLVDamage * LV;
            skillmoldObj[i].skillScript = skillDatas[i].skillDescription;
        }
    }

    // 이름을 사용하여 스킬 데이터를 반환 하는 함수
    public SkillMold GetSkillData(string skillName)
    {
        foreach (var skill in skillmoldObj)
        {
            if (skill.skillName == skillName)
            {
                return skill;
            }
        }
        return null;
    }

    public void StartCooltime(int num)
    {
        StartCoroutine(Cooltime(num));
    }

    public IEnumerator Cooltime(int num)
    {
        float time = 0f;
        skillmoldObj[num].skillUse = false;
        while (time < skillmoldObj[num].skillCooltime)
        {
            skillmoldObj[num].skillIconCooltime.fillAmount = Mathf.Lerp(1f, 0f, time / skillmoldObj[num].skillCooltime);
            time += Time.deltaTime;
            yield return null;
        }
        
        skillmoldObj[num].skillIconCooltime.fillAmount = 0f;
        skillmoldObj[num].skillUse = true;
    }

    public bool GetSkillUse(int num)
    {
        return skillmoldObj[num].skillUse;
    }
}