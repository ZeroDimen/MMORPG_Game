using System;
using static Constants;

public class BossController : EnemyController
{
    private int _attackPoiseDamage = 0; // Attack(일반 전투) 중 누적 피격 데미지 - 임계값 넘으면 Hit으로 끊김

    public override void ResetAttackPoise() => _attackPoiseDamage = 0;

    protected override bool ShouldInterruptOnHit(int damage)
    {
        // - 그로기: Hit 전환/넉백 없이 그로기 유지
        // - Skill1: 하이퍼아머로 완전 면역 (애니메이션 안 끊김, 데미지는 정상 적용)
        // - Attack(일반 전투): 포이즈 누적, 임계값(45) 넘을 때만 Hit으로 끊김
        if (State == EEnemyState.Groggy || State == EEnemyState.Skill1)
            return false;

        if (State == EEnemyState.Attack)
        {
            _attackPoiseDamage += damage;
            return _attackPoiseDamage >= 45;
        }

        return true;
    }

    protected override void Awake()
    {
        base.Awake();
        var bossStateChase = new BossStateChase(this, _animator, _navMeshAgent, enemyStatus);
        var enemyStateSkill1 = new EnemyStateSkill1(this, _animator, _navMeshAgent);
        var enemyStateGroggy = new EnemyStateGroggy(this, _animator, _navMeshAgent);
        
        if (_states.ContainsKey(EEnemyState.Chase)) // EEnemyState.Chase를 bossStateChase로 변경
        {
            _states[EEnemyState.Chase] = bossStateChase;
        }

        _states.Add(EEnemyState.Skill1, enemyStateSkill1);
        _states.Add(EEnemyState.Groggy, enemyStateGroggy);
    }

    private void Start()
    {
        SetState(EEnemyState.None);
        DungeonSystem.instance.RegisterBoss(this);
    }

    private void OnDestroy()
    {
        if(DungeonSystem.instance != null)
            DungeonSystem.instance.UnregisterBoss(this);
    }
}
