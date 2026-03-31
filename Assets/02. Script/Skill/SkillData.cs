using UnityEngine;

[CreateAssetMenu(fileName = "SkillData",menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public int skillBaseDamage;
    public int skillLVDamage;
    public int skillCooltime;
    public Sprite skillIcon;
    
}
