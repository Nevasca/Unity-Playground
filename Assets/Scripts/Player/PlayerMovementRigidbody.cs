using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementRigidbody : PlayerMovement
{
    [SerializeField] private float _groundCheckRadius = 0.15f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Header("Step Climb")]
    [SerializeField] private Transform stepRayUpper;
    [SerializeField] private Transform stepRayLower;
    [SerializeField] private float stepHeight = 0.3f;
    [SerializeField] private float stepSmooth = 0.1f;

    private Rigidbody _rigidbody;
    private bool _isGrounded;
    private Vector3[] _stepDirectionsCheck;
    
    public float GroundCheckRadius { 
        get { return _groundCheckRadius; } 
        set { _groundCheckRadius = value; } 
    }

    public Rigidbody Rigidbody => _rigidbody != null ? _rigidbody : GetComponent<Rigidbody>();

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
        //Physics.gravity = new Vector3(0f, gravity, 0f);
        stepRayUpper.position = new Vector3(stepRayUpper.position.x, stepHeight, stepRayUpper.position.z);
        _stepDirectionsCheck = new Vector3[] { new Vector3(0f, 0f, 1f), new Vector3(1.2f, 0f, 1f), new Vector3(-1.2f, 0f, 1f) };        
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        if (!_active)
            return;

        Move();
        Turn();
        StepClimb();

        _velocity = _rigidbody.velocity;
    }

    public override void Move()
    {
        SetMovementDirection();

        if (_isGrounded)
            _coyoteTimeCounter = coyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;

        _movementDirection *= speed * Time.deltaTime;
        _rigidbody.MovePosition(_rigidbody.position + _movementDirection);

        if(Jump())
        {
            _velocity = _rigidbody.velocity;
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _rigidbody.velocity = _velocity;
        }

        _rigidbody.velocity *= _velocityScale;
        SetAnimations();

        if (!_isGrounded && _rigidbody.velocity.y < 0f && !_startedDescending)
        {
            _startedDescending = true;
            //characterAudio.Play();
        }
    }

    public override void Turn()
    {
        _rotationDirection = _customTurn ? _rotationDirection : _movementDirection;

        if (_rotationDirection == Vector3.zero)
            return;

        _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_rotationDirection), turnSpeed * Time.deltaTime));
    }

    public override bool Jump()
    {
        _jumpBufferCounter -= Time.deltaTime;

        if (_coyoteTimeCounter > 0f && _jumpBufferCounter > 0f)
        {
            SoundManager.Instance.PlaySFXAt(5, transform.position, pitchRange: 0.05f);
            //_velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _animator.SetTrigger(ANIM_JUMP);

            _jumpBufferCounter = 0f;
            _coyoteTimeCounter = 0f;

            return true;
        }

        return false;
    }

    public override void SetGravityScale(float gravityScale)
    {
        base.SetGravityScale(gravityScale);
        _rigidbody.useGravity = gravityScale != 0f;
    }

    private void StepClimb()
    {
        if (_movementDirection.sqrMagnitude == 0f)
            return;

        Vector3 directionTransformed;

        foreach(Vector3 stepDirection in _stepDirectionsCheck)
        {
            directionTransformed = transform.TransformDirection(stepDirection);
            if (Physics.Raycast(stepRayLower.position, directionTransformed, 0.2f))
            {
                if (!Physics.Raycast(stepRayUpper.position, directionTransformed, 0.4f))
                {
                    _rigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
                    break;
                }
            }
        }
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, _groundCheckRadius, groundMask);
    }

    public void SetGroundCheckRadius()
    {

    }

    public override bool IsGrounded()
    {
        return _isGrounded;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(groundCheck.position, 0.1f);
    }
}