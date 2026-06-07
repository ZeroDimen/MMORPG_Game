using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateSkill1: EnemyState, ICharacterState
{
    public EnemyStateSkill1(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

public void Enter()
    {
        _enemyController.RpcSetTrigger(EnemyAniParamSkill1);
        if (!Photon.Pun.PhotonNetwork.IsMasterClient) return;
        var target = _enemyController.TargetTransform;
        if (target != null)
        {
            // 준비 자세 1.0초 대기 후 체공 구간(1.0초) 동안 플레이어 위치로 이동
            Vector3 targetPos = target.position;
            _enemyController.StartCoroutine(JumpWithDelay(targetPos));
        }
    }

private System.Collections.IEnumerator JumpWithDelay(Vector3 targetPos)
    {
        yield return new WaitForSeconds(1.0f); // 준비 자세 대기
        yield return _enemyController.StartCoroutine(
            _enemyController.JumpToTarget(targetPos, 1.0f, 2.0f) // 체공 1.0초, 높이 2.0
        );
    }


    public void Update()
    {
    }

public void Exit() { }
}