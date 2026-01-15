using UnityEngine;

public class Constants
{
    public const float Gravity = -9.81f;
    
    // ----------------------------------------
    // Layer Mask
    public static LayerMask GroundLayerMask => LayerMask.GetMask("Ground");
    
    // Player 상태
    public enum EPlayerState
    {
        None,
        Idle,
        Move,
        Jump,
        Attack,
        Emotion1,
        Emotion2,
    }
    
    // Player 애니메이터 파라미터
    public static readonly int PlayerAniParamIdle = Animator.StringToHash("idle");
    public static readonly int PlayerAniParamMove = Animator.StringToHash("move");
    public static readonly int PlayerAniParamJump = Animator.StringToHash("jump");
    public static readonly int PlayerAniParamAttack = Animator.StringToHash("attack");
    public static readonly int PlayerAniParamEmotion1 = Animator.StringToHash("emotion1");
    public static readonly int PlayerAniParamEmotion2 = Animator.StringToHash("emotion2");
    public static readonly int PlayerAniParamMoveSpeed = Animator.StringToHash("move_speed");
    public static readonly int PlayerAniParamGroundDistance = Animator.StringToHash("ground_distance");
}