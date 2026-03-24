using UnityEngine;

[CreateAssetMenu(fileName = "SkillData",menuName = "Skill/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string skillDescription;
    public int skillDamage;
    public int skillCooldown;
    public Sprite skillIcon;
    
}
