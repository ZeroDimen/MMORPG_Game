using TMPro;
using UnityEngine;

public class PlayerHpBar : HpBar
{
    [SerializeField] TMP_Text _hpText;

    public void SetHpText(string text)
    {
        _hpText.text = text;
    }
}