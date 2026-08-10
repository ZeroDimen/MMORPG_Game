using System.Collections;
using Photon.Pun;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private SkillData[] skillDatas;
    
    [SerializeField]
    private GameObject Skill_mold;

    private SkillMold[] skillmoldObj;
    private int[] skillRanks; // 스킬별 랭크 (0 = 미해금)
    
    private void SetGraphicVisible(Image graphic, bool visible)
    {
        graphic.color = visible ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1f); // 잠기해도 보이게 하되 회색톤으로 구분 (클릭은 계속 가능)
    }

    public bool IsSkillUnlocked(int index)
    {
        return skillRanks != null && index >= 0 && index < skillRanks.Length && skillRanks[index] > 0;
    }

    private void Start()
    {
        skillmoldObj = new SkillMold[skillDatas.Length];
        skillRanks = new int[skillDatas.Length];
        
        for (int i = 0; i < skillDatas.Length; i++) // 수정필요 skillData 수 많큼 출력
        {
            GameObject moldObj = Instantiate(Skill_mold, Skill_mold.transform.parent);
            skillmoldObj[i] = moldObj.GetComponent<SkillMold>();
            skillmoldObj[i].Init(this, i);
            
            moldObj.SetActive(true);
        }

        SetSkillData();
    }

    public void SetSkillData(int LV = 1)
    {
        for (int i = 0; i < skillDatas.Length; i++) // 수정필요 skillData 수 많큼 출력
        {
            bool unlocked = skillRanks[i] > 0;

            SetGraphicVisible(skillmoldObj[i].skillIcon, unlocked);

            skillmoldObj[i].skillIcon.sprite = skillDatas[i].skillIcon;
            skillmoldObj[i].skillName = skillDatas[i].skillName;
            skillmoldObj[i].skillCooltime = skillDatas[i].skillCooltime;
            skillmoldObj[i].skillDamage = skillDatas[i].skillBaseDamage + skillDatas[i].skillLVDamage * skillRanks[i];
            skillmoldObj[i].skillScript = skillDatas[i].skillDescription;
        }

        UpdateSkillPointBadges();
    }

    public void TrySpendSkillPoint(int index)
    {
        var localPlayer = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
        if (localPlayer == null || localPlayer.Status == null || localPlayer.Status.SkillPoint <= 0) return;

        skillRanks[index]++;
        localPlayer.Status.SetStatus("SkillPoint", localPlayer.Status.SkillPoint - 1);

        SetSkillData(localPlayer.Status.LV);
    }

    private void UpdateSkillPointBadges()
    {
        var localPlayer = PhotonNetwork.LocalPlayer.TagObject as PlayerController;
        bool hasPoint = localPlayer != null && localPlayer.Status != null && localPlayer.Status.SkillPoint > 0;

        foreach (var mold in skillmoldObj)
        {
            if (mold != null && mold.skillNewBadge != null)
            {
                mold.skillNewBadge.enabled = hasPoint;
            }
        }
    }

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