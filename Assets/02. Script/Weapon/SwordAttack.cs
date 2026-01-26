using UnityEngine;

// 플레이어가 적에게 데미지를 주는 스크립트
public class SwordAttack : MonoBehaviour
{
    public int Damage = 70;
    private PlayerController playerController;

    private void Start()
    {
        playerController = gameObject.transform.parent.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemyController = other.GetComponent<EnemyController>();
        if (enemyController)
        {
            int exp = enemyController.SetHit(30, transform.forward);
            playerController.SetExp(exp);
        }
    }
}
