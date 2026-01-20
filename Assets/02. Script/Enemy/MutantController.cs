using UnityEngine;

public class MutantController : EnemyController
{
    [SerializeField] private Collider attackCollider;
    
    public void EnableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = true;
        }
    }
    
    public void DisableAttackCollider()
    {
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }
}
