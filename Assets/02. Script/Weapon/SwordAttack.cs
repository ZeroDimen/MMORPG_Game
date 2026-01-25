using Photon.Pun;
using UnityEngine;

// 플레이어가 적에게 데미지를 주는 스크립트
public class SwordAttack : MonoBehaviour
{
    [SerializeField]
    private int Damage = 30;
    private void OnTriggerEnter(Collider other)
    {
        var enemyController = other.GetComponent<EnemyController>();
        if (enemyController)
        {
            enemyController.SetHit(30, transform.forward);
        }
    }
}
