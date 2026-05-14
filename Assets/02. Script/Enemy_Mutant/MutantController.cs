using Photon.Pun;
using UnityEngine;

public class MutantController : EnemyController
{
    [SerializeField] private GameObject attackObj;

    private Collider _attackCollider;
    private EnemyAttack _enemyAttack;

    protected override void Awake()
    {
        base.Awake();
        _attackCollider = attackObj.GetComponent<Collider>();
        _enemyAttack = attackObj.GetComponent<EnemyAttack>();
    }
    
    public void EnableAttackCollider()
    {
        if (_attackCollider != null)
        {
            _enemyAttack.damage = enemyStatus.damage;
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
