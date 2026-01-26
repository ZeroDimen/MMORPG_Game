using System;
using Photon.Pun;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class MariaPlayerController :PlayerController
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private GameObject attackObj;
    [SerializeField] private Transform Maria_Head;
    
    private Collider _attackCollider;
    private SwordAttack _swordAttack;

    protected override void Start()
    {
        base.Start();
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

        GameObject.Find("PlayerCam").GetComponent<CinemachineCamera>().Follow = Maria_Head;
        PlayerStatusView.Instance.player = this;
        // GameObject.Find("PlayerCam").GetComponent<CinemachineCamera>().LookAt = Maria_Head;
    }

    public void EnableAttackCollider()
    {
        if (_attackCollider != null)
        {
            _swordAttack.Damage = Status.ATK;
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