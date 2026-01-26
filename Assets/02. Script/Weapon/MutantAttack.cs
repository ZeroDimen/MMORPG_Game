using Photon.Pun;
using UnityEngine;

// 적이 플레이어에게 데미지를 주는 스크립트
public class MutantAttack : MonoBehaviour
{
    public int Damage = 0;
    private void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<PlayerController>();
        
        if (playerController)
        {
            playerController.SetHit(Damage, -transform.forward);
        }
    }
}