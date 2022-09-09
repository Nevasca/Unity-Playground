using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindPlayerMovement : RewindObject
{
    private PlayerMovement _playerMovement;
    private PitChecker _playerPitChecker;
    private Animator _playerAnimator;

    protected override void Awake()
    {
        base.Awake();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerPitChecker = GetComponent<PitChecker>();
        _playerAnimator = GetComponent<Animator>();
    }

    protected override PointInTime CreateTickData()
    {
        return new PointInTimePlayerMovement(
            _playerAnimator.GetBool(PlayerMovement.ANIM_WALKING),
            _playerAnimator.GetBool(PlayerMovement.ANIM_GROUNDED),            
            _playerAnimator.GetBool(PitChecker.ANIM_BALANCE));
    }

    protected override void Rewind()
    {
        if (HasPointInTime())
        {
            PointInTimePlayerMovement point = (PointInTimePlayerMovement)PopPointInTime();
            _playerAnimator.SetBool(PlayerMovement.ANIM_WALKING, point.isRunning);
            _playerAnimator.SetBool(PlayerMovement.ANIM_GROUNDED, point.isGrounded);
            _playerAnimator.SetBool(PitChecker.ANIM_BALANCE, point.isLoosingBalance);
        }
    }

    protected override void StartRewind()
    {
        _playerMovement.Enable(false);
        _playerPitChecker.enabled = false;
        _playerAnimator.SetFloat(PlayerMovement.ANIMATION_SCALE, RewindManager.Instance.RewindTimeScale);
    }

    protected override void StopRewind()
    {
        _playerMovement.Enable(true);
        _playerPitChecker.enabled = true;
        _playerAnimator.SetFloat(PlayerMovement.ANIMATION_SCALE, RewindManager.Instance.RewindTimeScale);
    }
}