using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class MariaPlayerController :PlayerController
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private GameObject attackObj;
    
    private Collider _attackCollider;
    private SwordAttack _swordAttack;

    private void Start()
    {
        if (photonView.IsMine)
        {
            playerName.text = PhotonNetwork.NickName;
            playerName.color = Color.green;
        }
        else
        {
            playerName.text = photonView.Owner.NickName;
            playerName.color = Color.red;
        }
        _attackCollider = attackObj.GetComponent<Collider>();
        _swordAttack = attackObj.GetComponent<SwordAttack>();
    }

    public void EnableAttackCollider()
    {
        if (_attackCollider != null)
        {
            _swordAttack.Damage = playerStatus.damage;
            _attackCollider.enabled = true;
        }
    }
    
    public void DisableAttackCollider()
    {
        if (_attackCollider != null)
        {
            _attackCollider.enabled = false;
        }
    }
}