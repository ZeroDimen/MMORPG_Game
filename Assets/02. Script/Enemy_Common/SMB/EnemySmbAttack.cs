using UnityEngine;

public class EnemySmbAttack : StateMachineBehaviour
{
    private EnemyController _enemyController;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_enemyController) _enemyController = animator.GetComponent<EnemyController>();
        _damageApplied = false;
    }

    private bool _damageApplied;
    private const float DamageNormalizedTime = 0.5f; // 80f / 160f (Mutant Swiping 3, 60fps, 2.667s)

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_damageApplied) return;
        if (stateInfo.normalizedTime < DamageNormalizedTime) return;

        _damageApplied = true;
        if (!Photon.Pun.PhotonNetwork.IsMasterClient) return;
        _enemyController.RpcAttackDamage(10);
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _enemyController.SetState(Constants.EEnemyState.Chase);
    }
}