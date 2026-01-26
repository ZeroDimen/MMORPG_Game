using Photon.Pun;
using UnityEngine;

public class PlayerHPBarController : MonoBehaviourPun
{
    [SerializeField] private GameObject hpBarPrefab;

    private PlayerController _playerController;
    private Canvas _canvas;
    private PlayerHpBar _hpBar;

    private void Awake()
    {
        if ((photonView.IsMine))
        {
            _canvas = GameManager.Instance.Canvas;
            _hpBar = Instantiate(hpBarPrefab,_canvas.transform).GetComponent<PlayerHpBar>();
            _playerController = photonView.GetComponent<PlayerController>();
            
            SetHp($"{_playerController.playerStatus.hp} / {_playerController.playerStatus.maxHp}");
            SetExp($"LV : {_playerController.playerStatus.level} | {_playerController.playerStatus.exp} / {_playerController.playerStatus.maxExp}");
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

    public void SetExp(string text)
    {
        _hpBar.SetExpText(text);
    }
}
