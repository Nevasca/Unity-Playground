using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Tweening;
using System;

public abstract class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float turnSpeed = 10f;

    [Header("Jump")]
    [SerializeField] protected float jumpHeight = 2f;
    [SerializeField] protected float gravity = -15f;
    [SerializeField] protected float coyoteTime = 0.2f;
    
    public const string ANIM_WALKING = "isWalking";
    public const string ANIM_JUMP = "jump";
    public const string ANIM_GROUNDED = "isGrounded";
    public const string ANIM_GRAVITY = "gravityScale";
    public const string ANIMATION_SCALE = "animationScale";

    //References
    protected Animator _animator;
    protected AudioSource _characterAudio;
    protected Transform _cameraTransform;

    //Movement input
    protected Vector2 _inputDirection;
    protected Vector3 _movementDirection;
    protected bool _active = true;

    //Rotation
    protected bool _customTurn = false;
    protected Vector3 _rotationDirection;
    
    //Jump
    protected bool _startedDescending = false;

    //Movement params
    protected float _gravityScale = 1f;
    protected float _velocityScale = 1f;
    protected float _animationScale = 1f;
    protected Vector3 _velocity;

    //Coyote time and jump buffering
    protected float _coyoteTimeCounter;
    protected float _jumpBufferCounter;

    public Vector3 Velocity { get { return _velocity; } set { _velocity = value; } }
    //public float Speed { get { return speed; } set { speed = value; } }

    public abstract void Move();
    public abstract void Turn();
    public abstract bool Jump();
    public abstract bool IsGrounded();

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterAudio = GetComponent<AudioSource>();
        _cameraTransform = Camera.main.transform;
    }

    #region Input
    public void OnMovement(InputAction.CallbackContext inputAction)
    {
        _inputDirection = inputAction.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext inputAction)
    {
        if (!_active)
            return;

        if (inputAction.started)
            _jumpBufferCounter = coyoteTime;
        else if (inputAction.canceled)
            _coyoteTimeCounter = 0f;
    }
    #endregion

    #region Animation Events
    private void Land()
    {
        _startedDescending = false;
        _characterAudio.Stop();
    }
    #endregion

    protected void SetMovementDirection()
    {
        //Sets the movement direction relative to camera
        var forward = _cameraTransform.forward;
        var right = _cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        _movementDirection = forward * _inputDirection.y + right * _inputDirection.x;
    }

    protected virtual void SetAnimations()
    {
        _animator.SetBool(ANIM_WALKING, _movementDirection.sqrMagnitude > 0f);
        _animator.SetBool(ANIM_GROUNDED, IsGrounded());
        _animator.SetFloat(ANIM_GRAVITY, Mathf.Clamp(_gravityScale, 0.3f, 1f));
        _animator.SetFloat(ANIMATION_SCALE, _animationScale);
    }

    public virtual void SetGravityScale(float gravityScale)
    {
        _gravityScale = gravityScale;
    }

    public void SetVelocityScale(float velocityScale)
    {
        _velocityScale = velocityScale;
    }

    public void SetCustomTurn(bool customTurn, Vector3 direction)
    {
        _customTurn = customTurn;
        direction.y = 0f;
        _rotationDirection = direction;
    }

    public void SmoothVelocityScale(float endValue, float duration = 0.5f)
    {
        TweenVirtual.DoFloat(_gravityScale, endValue, duration, SetGravityScale);
        TweenVirtual.DoFloat(_velocityScale, endValue, duration, SetVelocityScale);
    }

    public void SetAnimationScale(float value)
    {
        _animationScale = value;
    }

    public void Enable(bool enable)
    {
        _active = enable;
        if(!enable)
            _animator.SetBool(ANIM_WALKING, false);
    }
}