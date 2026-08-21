using ExitGames.Client.Photon;
using Photon.Pun;
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
    public int SkillPoint; // 레벨업으로 획득하는 스킬 포인트도
    

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
        SetProperties();
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
            case "ATK" :
                ATK = value;
                break;
            case "SkillPoint" :
                SkillPoint = value;
                break;
        }

        SetProperties();
    }

    private void SetProperties()
    {
        var props = new Hashtable
        {
            { "Hp", HP },
            { "MaxHp", MAXHP },
            { "ATK", ATK },
            { "DEF", DEF },
            { "DEX", DEX },
            { "LV", LV }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void AddStatus(InstanceItem item)
    {
        foreach (var status in item._statBonusList)
        {
            switch (status.type)
            {
                case StatType.HP :
                    HP += status.value;
                    MAXHP += status.value;
                    GameEvents.OnPlayerHpChanged?.Invoke(this);
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
                    MAXHP -= status.value;
                    GameEvents.OnPlayerHpChanged?.Invoke(this);
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
