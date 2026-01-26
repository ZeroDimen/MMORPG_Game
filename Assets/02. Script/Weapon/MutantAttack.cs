using System;
using Photon.Pun;
using UnityEngine;

// 적이 플레이어에게 데미지를 주는 스크립트
public class MutantAttack : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<PlayerController>();
        
        if (playerController)
        {
            playerController.SetHit(damage, -transform.forward);
        }
    }
}