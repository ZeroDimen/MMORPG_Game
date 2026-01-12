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
        None,Idle,Move,Jump
    }
    
    // Player 애니메이터 파라미터
    public static readonly int PlayerAniParamIdle = Animator.StringToHash("idle");
    public static readonly int PlayerAniParamMove = Animator.StringToHash("move");
    public static readonly int PlayerAniParamJump = Animator.StringToHash("jump");
    public static readonly int PlayerAniParamMoveSpeed = Animator.StringToHash("move_speed");
    public static readonly int PlayerAniParamGroundDistance = Animator.StringToHash("ground_distance");
}