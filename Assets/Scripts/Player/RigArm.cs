using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigArm : MonoBehaviour
{
    [SerializeField] private TwoBoneIKConstraint armConstraint;
    [SerializeField] private Transform armTarget;
    [SerializeField] private Transform armTransform;
    [SerializeField] private float baseLerpSpeed;

    //Constraint weight
    private float _desiredWeight;
    private float _weightVelocity;
    
    private float _transitionDuration = 0.15f;

    private Vector3 _targetPosition;
    private Vector3 _neutralOffset;
    private float _lerpSpeed;

    private Transform _holdTarget;


    private Action _callbackArmReached;
    private bool _callBackCalled;

    private void Awake()
    {
        _neutralOffset = armTarget.position - armTransform.position;
        _neutralOffset = transform.forward * _neutralOffset.z + transform.right * _neutralOffset.x; //Caso rotacao nao esteja zerada

        armConstraint.weight = 0f;
        baseLerpSpeed /= 10f;
        _lerpSpeed = baseLerpSpeed;
    }

    private void Update()
    {
        UpdateArmPosition();

        armConstraint.weight = Mathf.SmoothDamp(armConstraint.weight, _desiredWeight, ref _weightVelocity, _transitionDuration);
    }

    private void UpdateArmPosition()
    {
        if (_holdTarget != null)
            _targetPosition = _holdTarget.position;
        else
            _targetPosition = armTransform.position + _neutralOffset.z * transform.forward;


        armTarget.position = Vector3.Lerp(armTarget.position, _targetPosition, 
            Time.deltaTime * _lerpSpeed);

        //Callback quando o braco alcanca o destino
        if (_callBackCalled || _callbackArmReached == null)
            return;

        Vector3 dist = _targetPosition - armTarget.position;
        if (armConstraint.weight > 0.9f && dist.sqrMagnitude < 0.02f)
        {
            _callBackCalled = true;
            _callbackArmReached?.Invoke();
        }
    }

    public void Hold(Transform holdTarget, float desiredWeight = 1f, Action armReachedCallback = null)
    {
        _callBackCalled = false;
        _callbackArmReached = armReachedCallback;
        _holdTarget = holdTarget;
        _desiredWeight = desiredWeight;
        _lerpSpeed = baseLerpSpeed * 20f;
    }

    public void CancelHold()
    {
        _holdTarget = null;
        _desiredWeight = 0f;
        _lerpSpeed = baseLerpSpeed;
    }
}