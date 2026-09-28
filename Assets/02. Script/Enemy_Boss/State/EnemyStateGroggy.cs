using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateGroggy: EnemyState, ICharacterState
{
    public EnemyStateGroggy(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

public void Enter()
    {
        _enemyController.RpcSetTrigger(EnemyAniParamGroggy);
    }


    public void Update()
    {
    }

public void Exit()
    {
        _enemyController.LastGroggyEndTime = Time.time;
        _enemyController.RerollTarget(); // 그로기 종료 시 타겟(어그로) 재추첨
    }
}