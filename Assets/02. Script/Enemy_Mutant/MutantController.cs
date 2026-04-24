using Photon.Pun;
using UnityEngine;

public class MutantController : EnemyController
{
    [SerializeField] private GameObject attackObj;

    private Collider _attackCollider;
    private MutantAttack _mutantAttack;

    protected override void Awake()
    {
        base.Awake();
        _attackCollider = attackObj.GetComponent<Collider>();
        _mutantAttack = attackObj.GetComponent<MutantAttack>();
    }
    
    public void EnableAttackCollider()
    {
        if (_attackCollider != null)
        {
            _mutantAttack.damage = enemyStatus.damage;
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
