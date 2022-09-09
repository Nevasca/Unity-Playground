using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementCharacter : PlayerMovement
{
    //References
    private CharacterController _characterController;
    private float _defaultStepOffset;

    protected override void Awake()
    {
        base.Awake();

        _characterController = GetComponent<CharacterController>();
        _defaultStepOffset = _characterController.stepOffset;
    }

    private void Update()
    {
        if (!_active)
            return;

        Jump();
        Move();
        Turn();
    }

    public override void Move()
    {
        SetMovementDirection();
        bool isGrounded = _characterController.isGrounded;

        if (isGrounded)
            _coyoteTimeCounter = coyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;

        if (isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        _characterController.stepOffset = isGrounded ? _defaultStepOffset : 0.01f;

        _velocity.y += gravity * _gravityScale * Time.deltaTime;

        _characterController.Move(Time.deltaTime * _velocityScale * (_movementDirection * speed + _velocity));

        SetAnimations();

        if (!isGrounded && _velocity.y < 0f && !_startedDescending)
        {
            _startedDescending = true;
            //characterAudio.Play();
        }
    }

    public override bool Jump()
    {
        _jumpBufferCounter -= Time.deltaTime;

        if (_coyoteTimeCounter > 0f && _jumpBufferCounter > 0f)
        {
            SoundManager.Instance.PlaySFXAt(5, transform.position, pitchRange: 0.05f);
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _animator.SetTrigger(ANIM_JUMP);

            _jumpBufferCounter = 0f;
            _coyoteTimeCounter = 0f;

            return true;
        }

        return false;
    }

    public override void Turn()
    {
        _rotationDirection = _customTurn ? _rotationDirection : _movementDirection;

        if (_rotationDirection == Vector3.zero)
            return;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_rotationDirection), turnSpeed * Time.deltaTime);
    }

    public override bool IsGrounded()
    {
        return _characterController.isGrounded;
    }
}