using static Constants;

public class BossController : EnemyController
{
    protected override void Awake()
    {
        base.Awake();
        var bossStateChase = new BossStateChase(this, _animator, _navMeshAgent, enemyStatus);
        var enemyStateSkill1 = new EnemyStateSkill1(this, _animator, _navMeshAgent);
        
        if (_states.ContainsKey(EEnemyState.Chase)) // EEnemyState.Chase를 bossStateChase로 변경
        {
            _states[EEnemyState.Chase] = bossStateChase;
        }
        _states.Add(EEnemyState.Skill1 , enemyStateSkill1);
    }
}
