using System;
using UnityEngine;

public class Constants
{
    public const float Gravity = -9.81f;






    public enum EGameState
    {
        Play,
        Interaction,
        TextInput,
    }
    // ----------------------------------------
    // Layer Mask
    public static LayerMask GroundLayerMask => LayerMask.GetMask("Ground", "Dungeon");
    
    // Player 상태
    public enum EPlayerState
    {
        None,
        Idle,
        Move,
        Jump,
        Spawn,
        Attack,
        Hit,
        Dead,
        Emotion1,
        Skill1,
        Skill2,
    }
    
    // Player 애니메이터 파라미터
    public static readonly int PlayerAniParamIdle = Animator.StringToHash("idle");
    public static readonly int PlayerAniParamMove = Animator.StringToHash("move");
    public static readonly int PlayerAniParamJump = Animator.StringToHash("jump");
    public static readonly int PlayerAniParamSpawn = Animator.StringToHash("spawn");
    public static readonly int PlayerAniParamAttack = Animator.StringToHash("attack");
    public static readonly int PlayerAniParamHit = Animator.StringToHash("hit");
    public static readonly int PlayerAniParamDead = Animator.StringToHash("dead");
    public static readonly int PlayerAniParamEmotion1 = Animator.StringToHash("emotion1");
    public static readonly int PlayerAniParamSkill1 = Animator.StringToHash("skill1");
    public static readonly int PlayerAniParamSkill2 = Animator.StringToHash("skill2");
    public static readonly int PlayerAniParamMoveSpeed = Animator.StringToHash("move_speed");
    public static readonly int PlayerAniParamGroundDistance = Animator.StringToHash("ground_distance");
    
    
    
    
    // ----------------------------------------
    // Enemy 상태
    public enum EEnemyState
    {
        None, Idle, Patrol, Chase, Attack, Hit, Dead,
        Skill1,
    }
    
    // ----------------------------------------
    // Enemy 애니메이터 파라미터
    public static readonly int EnemyAniParamIdle = Animator.StringToHash("idle");
    public static readonly int EnemyAniParamPatrol = Animator.StringToHash("patrol");
    public static readonly int EnemyAniParamChase = Animator.StringToHash("chase");
    public static readonly int EnemyAniParamAttack = Animator.StringToHash("attack");
    public static readonly int EnemyAniParamHit = Animator.StringToHash("hit");
    public static readonly int EnemyAniParamDead = Animator.StringToHash("dead");
    public static readonly int EnemyAniParamMoveSpeed = Animator.StringToHash("move_speed");
    
    // Boss 애니메이터 파라미터
    public static readonly int EnemyAniParamSkill1 = Animator.StringToHash("skill1");
    
    [Serializable]
    public class SpawnZone
    {
        public Transform point;
        public int radius;
    }

    [Serializable]
    public class EnemyStatus
    {
        public int maxHp;
        public int hp;
        public int damage;
        public int exp;
    }
}