using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateSkill1: EnemyState, ICharacterState
{
    public EnemyStateSkill1(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _animator.SetTrigger(EnemyAniParamSkill1);
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}