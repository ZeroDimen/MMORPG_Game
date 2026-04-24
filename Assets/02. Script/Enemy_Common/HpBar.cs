using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Image gauge;

    public void SetHPGauge(float hp)
    {
        gauge.fillAmount = hp;
    }
}