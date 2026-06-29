using Photon.Pun;
using UnityEngine;

public class MutantController : EnemyController
{
    [SerializeField] private GameObject attackObj;

    private Collider _attackCollider;

    protected override void Awake()
    {
        base.Awake();
        _attackCollider = attackObj.GetComponent<Collider>();
    }
    
    public void EnableAttackCollider()
    {
        if (_attackCollider != null)
        {
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
