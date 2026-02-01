using Photon.Pun;
using UnityEngine;

// 플레이어가 적에게 데미지를 주는 스크립트
public class SwordAttack : MonoBehaviour
{
    public int Damage = 1;
    private PlayerController playerController;
    private PhotonView playerPV;

    private void Start()
    {
        playerController = gameObject.transform.parent.GetComponent<PlayerController>();
        playerPV = gameObject.transform.parent.GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerPV.IsMine) return;
        var enemyController = other.GetComponent<EnemyController>();
        if (enemyController)
        {
            int exp = enemyController.SetHit(Damage, transform.forward);
            playerController.SetExp(exp);
        }
    }
}
