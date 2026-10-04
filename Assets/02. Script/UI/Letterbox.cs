using System.Collections;
using UnityEngine;

public class Letterbox : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform top;
    [SerializeField] private RectTransform bottom;
     private float height = 120f;
     private float duration = 2f;

    public IEnumerator Show() => Animate(0f, height);
    public IEnumerator Hide() => Animate(height, 0f);

    private IEnumerator Animate(float from, float to)
    {
        _canvasGroup.alpha = _canvasGroup.alpha >= 1 ? 0 : 1;
        
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration);
            float h = Mathf.Lerp(from, to, k);
            top.sizeDelta = new Vector2(0, h);
            bottom.sizeDelta = new Vector2(0, h);
            yield return null;
        }
    }
}
