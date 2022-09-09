using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Tweening;

public class PlayerStickerMovement : PlayerMovement
{
    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody;


    [Header("Movement Params")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _distanceToTurn = 1f;

    [Header("Visual")]
    [SerializeField] private Transform _spritePivot;
    [SerializeField] private Renderer _spriteRenderer;
    [SerializeField] private Texture2D _textureGrounded;
    [SerializeField] private Texture2D _textureJump;

    private const string DECAL_TEXTURE = "DecalTexture";

    private FlatWall _wall;
    private Vector3 _originPos;
    private Vector3 _targetPos;
    private bool _rotationMode;

    private bool _isGrounded;
    private MaterialPropertyBlock _propBlock;
    private float xInput;

    protected override void Awake()
    {
        base.Awake();
        _propBlock = new MaterialPropertyBlock();
    }

    public void SetOnWall(FlatWall wall, Vector3 origin, Vector3 target)
    {
        _wall = wall;
        transform.position = wall.StartPoint.position;
        transform.forward = -wall.StartPoint.forward;
        _originPos = origin;
        _targetPos = target;
        _rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(true);
    }

    public void ExitWall()
    {
        _wall = null;
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        CheckGrounded();

        if (!_active)
            return;

        if(!_rotationMode)
            Move();

        _velocity = _rigidbody.velocity;
    }

    public override void Move()
    {
        SetMovementDirection();

        if (_isGrounded)
            _coyoteTimeCounter = coyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;

        //Horizontal movement
        xInput = _inputDirection.x;
        Vector3 desiredPosition = Vector3.MoveTowards(transform.position, GetTargetMove(), Mathf.Abs(xInput) * Time.deltaTime * speed);
        _rigidbody.MovePosition(desiredPosition);

        //Checks if close to a corner and need to rotate
        Vector3 currentPosition = transform.position;
        currentPosition.y = _originPos.y;

        if (Vector3.Distance(currentPosition, _originPos) > Vector3.Distance(_originPos, _targetPos) - _distanceToTurn 
            || Vector3.Distance(currentPosition, _originPos) < _distanceToTurn)
        {
            StartRotation(Vector3.Distance(currentPosition, _targetPos) < Vector3.Distance(currentPosition, _originPos));
        }

        if (Jump())
        {
            _velocity = _rigidbody.velocity;
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _rigidbody.velocity = _velocity;
            
        }

        _rigidbody.velocity *= _velocityScale;

        SetAnimations();
    }

    private Vector3 GetTargetMove()
    {
        Vector3 target = xInput > 0f ? _targetPos : _originPos;
        target.y = transform.position.y; //Fix jumping issues
        return target;
    }

    public override bool Jump()
    {
        _jumpBufferCounter -= Time.deltaTime;

        if (_coyoteTimeCounter > 0f && _jumpBufferCounter > 0f)
        {
            SoundManager.Instance.PlaySFXAt(45, transform.position, 0.9f, pitch: 1.2f, pitchRange: 0.02f);
            _animator.SetTrigger(ANIM_JUMP);

            _jumpBufferCounter = 0f;
            _coyoteTimeCounter = 0f;

            return true;
        }

        return false;
    }

    protected override void SetAnimations()
    {
        base.SetAnimations();

        //Sets decal texture to grounded or jumping
        _spriteRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetTexture(DECAL_TEXTURE, _isGrounded ? _textureGrounded : _textureJump);
        _spriteRenderer.SetPropertyBlock(_propBlock);

        //Rotates sprite pivot to face right or left
        Vector3 scale = _spritePivot.localScale;
        scale.x = xInput > 0f ? 1f : xInput < 0 ? -1f : scale.x;
        _spritePivot.localScale = scale;
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, 0.15f, _groundMask);
    }

    public override bool IsGrounded()
    {
        return _isGrounded;
    }

    private void StartRotation(bool isRight)
    {
        _rotationMode = true;
        _rigidbody.isKinematic = true;

        //Get corner indexes based from the current corner
        int currentIndex = _wall.Corners.FindIndex((x) => x.position == (isRight ? _targetPos : _originPos));
        int nextIndex = _wall.Corners.NextIndex(currentIndex);
        int previousIndex = _wall.Corners.PreviousIndex(currentIndex);

        //Get next wall normal from a corner
        Vector3 desiredNormal = -_wall.Corners[isRight ? nextIndex : currentIndex].normal;

        //Get position character will be placed on the next wall
        Vector3 desiredPosition = _wall.Corners[currentIndex].position + _distanceToTurn * 1.2f * transform.forward;
        desiredPosition.y = transform.position.y;

        //Set next wall origin and target positions
        _originPos = _wall.Corners[isRight ? currentIndex : previousIndex].position;
        _targetPos = _wall.Corners[isRight ? nextIndex : currentIndex].position;

        //Move and rotate character to next wall
        TweenSequence seq = Tweener.Sequence();
        seq.Append(transform.DoRotate(Quaternion.LookRotation(desiredNormal).eulerAngles, 0.2f));
        seq.Join(transform.DoMove(desiredPosition, 0.2f));
        seq.AppendCallback(() => {
            _rigidbody.isKinematic = false;
            _rotationMode = false;
        });
        seq.Play();
    }

    public override void Turn() { }
}
