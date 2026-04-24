using UnityEngine;
using static Constants;

public class BossController : EnemyController
{
    [SerializeField] private int a = 0;

    protected override void Awake()
    {
        
        base.Awake();
        var bossStateChase = new BossStateChase(this, _animator, _navMeshAgent);
        var enemyStateSkill1 = new EnemyStateSkill1(this, _animator, _navMeshAgent);
        
        if (_states.ContainsKey(EEnemyState.Chase))
        {
            _states[EEnemyState.Chase] = bossStateChase;
        }
        _states.Add(EEnemyState.Skill1 , enemyStateSkill1);
    }
}
