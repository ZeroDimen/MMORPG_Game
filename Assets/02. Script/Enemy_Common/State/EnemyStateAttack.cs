using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateAttack: EnemyState, ICharacterState
{
    public EnemyStateAttack(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _enemyController.RpcSetTrigger(EnemyAniParamAttack); // 로컬에서 직접 수정하지 않고, RPC로 모든 클라이언트의 Animator에 동기화
        _enemyController.GiveSfxPlay("EnemyAttack");
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}