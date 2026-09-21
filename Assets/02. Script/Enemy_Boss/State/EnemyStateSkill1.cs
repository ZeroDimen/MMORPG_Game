using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateSkill1: EnemyState, ICharacterState
{
    public EnemyStateSkill1(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

public void Enter()
    {
        _enemyController.StopCoroutine(nameof(JumpCombo));

        _navMeshAgent.isStopped = true;
        _enemyController.RpcSetFloat(EnemyAniParamMoveSpeed, 0f); // Chase 블렌드트리 잔여 속도 제거

        if (!Photon.Pun.PhotonNetwork.IsMasterClient) return;
        if (_enemyController.TargetTransform != null)
        {
            _enemyController.StartCoroutine(JumpCombo());
        }
    }

private IEnumerator JumpCombo()
    {
        // 체력 50% 이하면 풀 콤보(2연속 점프+암전+그로기), 초과면 견제용 단발 점프
        bool isFullCombo = (float)_enemyController.enemyStatus.hp / _enemyController.enemyStatus.maxHp <= 0.5f;
        int jumpCount = isFullCombo ? 2 : 1;

        if (isFullCombo)
            DungeonSystem.instance.SetHangingCageDark(); // 즉시 암전 (지속시간 타이머 없음 - 콤보가 끝날 때까지 유지됨)

        for (int i = 0; i < jumpCount; i++)
        {
            var target = _enemyController.TargetTransform;
            if (target == null)
            {
                // 콤보 도중 타겟을 놓치면 정상 AI 루프로 복귀 (암전 중이었다면 불도 다시 켜줌)
                if (isFullCombo)
                    DungeonSystem.instance.RestoreHangingCageLight();
                _enemyController.SetState(EEnemyState.Chase);
                yield break;
            }

            yield return _enemyController.StartCoroutine(SingleJump(target.position));
        }

        if (isFullCombo)
        {
            // 콤보(2번째 착지) 완료 → 제자리 정지 후 불을 켜고, 켜지기 시작하면 그로기
            _navMeshAgent.isStopped = true;
            _enemyController.RpcSetFloat(EnemyAniParamMoveSpeed, 0f);
            DungeonSystem.instance.RestoreHangingCageLight(() => _enemyController.SetState(EEnemyState.Groggy));
        }
        else
        {
            // 견제용 단발 점프 — 그로기 없이 바로 전투 복귀
            _enemyController.SetState(EEnemyState.Chase);
        }
    }

    private IEnumerator SingleJump(Vector3 targetPos)
    {
        _enemyController.RpcSetTrigger(EnemyAniParamSkill1);

        yield return new WaitForSeconds(1f); // 준비 자세 대기

        // 체공 시작 → AoE 인디케이터 표시
        _enemyController.ShowJumpIndicator(targetPos, 6.0f, 6.0f);

        // 플레이어 위치로 포물선 이동
        yield return _enemyController.StartCoroutine(
            _enemyController.JumpToTarget(targetPos, 1.0f, 2.0f)
        );

        // 착지 → 인디케이터 제거 + 데미지 판정
        _enemyController.HideJumpIndicator();

        // halfExtents = BoxCollider 월드 크기(6x6)의 절반, 데미지 = 30
        Vector3 landPos     = _enemyController.transform.position;
        Vector3 halfExtents = new Vector3(3.0f, 0.5f, 3.0f);
        _enemyController.RpcJumpLandingDamage(landPos, halfExtents, 30);
        _enemyController.GiveSfxPlay("Boss_Jump");
        DungeonSystem.instance.TriggerCameraShake(); // 점프 착지마다 카메라 흔들림

        // 착지 애니메이션(리커버리)이 완전히 끝날 때까지 대기 후 다음 동작으로 진행
        yield return new WaitUntil(() =>
        {
            var info = _animator.GetCurrentAnimatorStateInfo(0);
            return !info.IsName("Sargent Jump") || info.normalizedTime >= 1f;
        });
    }


    public void Update()
    {
    }

public void Exit()
    {
        _enemyController.StopCoroutine(nameof(JumpCombo));
    }
}