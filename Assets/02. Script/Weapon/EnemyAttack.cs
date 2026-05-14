using System;
using Photon.Pun;
using UnityEngine;

// 적이 플레이어에게 데미지를 주는 스크립트
public class EnemyAttack : MonoBehaviour
{
    public int damage = 1;
    private PhotonView enemyPV;

    private void Start()
    {
        enemyPV = gameObject.transform.parent.GetComponent<PhotonView>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!enemyPV.IsMine) return;
        var playerController = other.GetComponent<PlayerController>();
        if (playerController)
        {
            PhotonView playerView = playerController.photonView;
            GameManager.Instance.HitPlayer(playerView, damage);
        }
    }
}