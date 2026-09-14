using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateSkill1: EnemyState, ICharacterState
{
    public EnemyStateSkill1(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    private int attacknum = 0;
    private int attacknum2 = 0;

public void Enter()
    {
        _enemyController.StopCoroutine(nameof(JumpWithDelay));
        _enemyController.RpcSetTrigger(EnemyAniParamSkill1);

        _navMeshAgent.isStopped = true;
        _enemyController.RpcSetFloat(EnemyAniParamMoveSpeed, 0f); // Chase 블렌드트리 잔여 속도 제거

        if (!Photon.Pun.PhotonNetwork.IsMasterClient) return;
        var target = _enemyController.TargetTransform;
        if (target != null)
        {
            // 준비 자세 1.0초 대기 후 체공 구간(1.0초) 동안 플레이어 위치로 이동
            Vector3 targetPos = target.position;
            _enemyController.StartCoroutine(JumpWithDelay(targetPos));
        }
    }

private IEnumerator JumpWithDelay(Vector3 targetPos)
    {
        yield return new WaitForSeconds(1f); // 준비 자세 대기

        // 체공 시작 → AoE 인디케이터 표시
        _enemyController.ShowJumpIndicator(targetPos, 6.0f, 6.0f);

        // 플레이어 위치로 포물선 이동
        yield return _enemyController.StartCoroutine(
            _enemyController.JumpToTarget(targetPos, 1.0f, 2.0f)
        );

        // 착지 → 인디케이터 제거 + 데미지 판정
        _enemyController.HideJumpIndicator();
;

        // halfExtents = BoxCollider 월드 크기(6x6)의 절반, 데미지 = 10
        Vector3 landPos     = _enemyController.transform.position;
        Vector3 halfExtents = new Vector3(3.0f, 0.5f, 3.0f);
        _enemyController.RpcJumpLandingDamage(landPos, halfExtents, 30);
        _enemyController.GiveSfxPlay("Boss_Jump");
        attacknum++;
        attacknum2++;

        if (attacknum >= 2)
        {
            _enemyController.SetState(EEnemyState.Groggy);
            attacknum = 0;
        }
        else if (attacknum2 >= 3)
        {
            attacknum2 = 0;
            DungeonSystem.instance.TriggerHangingCageDarkness();
            _enemyController.SetState(EEnemyState.Chase);
        }
        else
        {
            // 그로기로 넘어가지 않으면 정상 AI 루프(Phase1)로 복귀
            _enemyController.SetState(EEnemyState.Chase);
        }
    }


    public void Update()
    {
    }

    public void Exit()
    {
        _enemyController.StopCoroutine(nameof(JumpWithDelay));
    }
}