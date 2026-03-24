using UnityEngine;

public class PlayerStatus
{
    public int HP;
    public int MAXHP;
    public int LV;
    public int MAXEXP;
    public int EXP;
    public int ATK;     // 공격력
    public int DEF;     // 방어력
    public int DEX;     // 이동속도
    

    public PlayerStatus(int hp, int maxhp, int lv, int maxexp, int exp, int atk, int def, int dex)
    {
        HP = hp;
        MAXHP = maxhp;

        LV = lv;
        MAXEXP = maxexp;
        EXP = exp;

        ATK = atk;
        DEF = def;
        DEX = dex;

        GameEvents.OnItemEquipped += AddStatus;
        GameEvents.OnItemUnEquipped += RemoveStatus;
    }

    public void SetStatus(string status, int value)
    {
        switch (status)
        {
            case "EXP" :
                EXP = value;
                break;
            case "MAXEXP" :
                MAXEXP = value;
                break;
            case "LV" :
                LV = value;
                break;
            case "HP" :
                HP = value;
                break;
            case "MAXHP" :
                MAXHP = value;
                break;
        }
    }

    private void AddStatus(InstanceItem item)
    {
        foreach (var status in item._statBonusList)
        {
            switch (status.type)
            {
                case StatType.HP :
                    HP += status.value;
                    break;
                case StatType.ATK :
                    ATK += status.value;
                    break;
                case StatType.DEF :
                    DEF += status.value;
                    break;
                case StatType.DEX :
                    DEX += status.value;
                    break;
            }
        }
        GameEvents.OnStatusChanged?.Invoke(this);
    }
    
    private void RemoveStatus(InstanceItem item)
    {
        foreach (var status in item._statBonusList)
        {
            switch (status.type)
            {
                case StatType.HP :
                    HP -= status.value;
                    break;
                case StatType.ATK :
                    ATK -= status.value;
                    break;
                case StatType.DEF :
                    DEF -= status.value;
                    break;
                case StatType.DEX :
                    DEX -= status.value;
                    break;
            }
        }
        GameEvents.OnStatusChanged?.Invoke(this);
    }
}
