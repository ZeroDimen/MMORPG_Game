using TMPro;
using UnityEngine;

public class PlayerHpBar : HpBar
{
    [SerializeField] TMP_Text _hpText;
    [SerializeField] TMP_Text _expText;

    public void SetHpText(string text)
    {
        _hpText.text = text;
    }
    
    public void SetExpText(string text)
    {
        _expText.text = text;
    }
}