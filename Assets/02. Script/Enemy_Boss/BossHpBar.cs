using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private Ease ease = Ease.OutQuad;

    private Tween _tween;

    public void SetMaxHp()
    {
        image.fillAmount = 1f;
    }

    // result: 0~1 사이 비율, 또는 maxHp 기준 현재 hp 값 그대로 써도 됨
    public void SetHp(float targetValue)
    {
        _tween?.Kill();
        _tween = image.DOFillAmount(targetValue, duration).SetEase(ease);
    }
}
