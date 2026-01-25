using UnityEngine;

public class PlayerStatus
{
    private int _hp;
    public int HP
    {
        get => _hp;
        set
        {
            _hp += value;
            if (_hp <= 0)
            {
                // TODO : 죽는 로직
                Debug.Log("죽었습니다.");
            }
        }
    }
    public int MP;
    public int EXP;
    public int ATK;     // 공격력
    public int DEF;     // 방어력
    public int DEX;     // 이동속도
    
    public PlayerStatus(int hp, int mp, int atk, int def, int dex)
    {
        _hp = hp;
        MP = mp;
        ATK = atk;
        DEF = def;
        DEX = dex;

        GameEvents.OnItemEquipped += AddStatus;
        GameEvents.OnItemUnEquipped += RemoveStatus;
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
                case StatType.MP :
                    MP += status.value;
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
                case StatType.MP :
                    MP -= status.value;
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
