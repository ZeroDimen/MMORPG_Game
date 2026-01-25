using Photon.Pun;
using UnityEngine;

public class PlayerHPBarController : MonoBehaviourPun
{
    [SerializeField] private GameObject hpBarPrefab;
    
    private Canvas _canvas;
    private PlayerHpBar _hpBar;

    private void Awake()
    {
        if ((photonView.IsMine))
        {
            _canvas = GameManager.Instance.Canvas;
            _hpBar = Instantiate(hpBarPrefab,_canvas.transform).GetComponent<PlayerHpBar>();
        }
    }
    
    public void SetHp(float hp)
    {
        _hpBar.SetHPGauge(hp);
    }

    public void SetHp(string text)
    {
        _hpBar.SetHpText(text);
    }
}
