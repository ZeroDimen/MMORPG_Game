using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

public class PlayerStateSkill2: PlayerState, ICharacterState
{
    public PlayerStateSkill2(PlayerController playerController, Animator animator, PlayerInput playerInput,
        SkillManager skillManager)
        : base(playerController, animator, playerInput)
    {
        _skillManager = skillManager;
    }

    public void Enter()
    {
        if (!_skillManager.IsSkillUnlocked(1) || !_skillManager.GetSkillUse(1))
        {
            Debug.Log("스킬 2을 사용할 수 없습니다.");
            _playerController.SetState(EPlayerState.Idle);
            return;
        }
        // Skill2 애니메이션 실행
        _animator.SetTrigger(PlayerAniParamSkill2);
        _playerController.GiveSfxPlay("Skill_Water");
        _skillManager.StartCooltime(1);
    }

public void Update()
    {
        // Skill2 애니메이션 진행률이 Skill2CancelThreshold를 넘으면 이동 입력으로 캔슬 가능
        if (_playerInput.actions["Move"].IsPressed())
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Skill Water Spin") && stateInfo.normalizedTime >= _playerController.Skill2CancelThreshold)
                _playerController.SetState(EPlayerState.Move);
        }
    }

    public void Exit()
    {
    }
}