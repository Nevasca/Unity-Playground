using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class LookAtNearby : MonoBehaviour
{
    [SerializeField] private MultiAimConstraint aimConstraint;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform aimTargetTransform;
    [SerializeField] private float visionRadius;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float rotationRange = -0.2f;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private float baseInterestSpeed = 1.5f;

    private PointOfInterest nearPointOfInterest;
    private bool lookingAtAsked;
    private Vector3 origin;
    private Vector3 neutralOffset;
    private Collider[] cols;
    private Vector3 targetPosition;
    private Vector3 focusDir;

    //Rig weight
    private float interestSpeed;
    private float desiredWeight;
    private float weightVelocity;

    private bool focusEnable = true;

    private bool _active = true;
    public bool Active 
    { 
        get
        {
            return _active;
        }
        set 
        {
            _active = value;
            desiredWeight = _active ? desiredWeight : 0f;
        } 
    }

    private void Start()
    {
        neutralOffset = aimTargetTransform.position - headTransform.position;
        neutralOffset = transform.forward * neutralOffset.z + transform.right * neutralOffset.x; //Caso rotacao nao esteja zerada

        aimConstraint.weight = 0f;
        lerpSpeed /= 10f;
        interestSpeed = baseInterestSpeed;
    }
    private void Update()
    {
        if (!_active)
            return;

        if(focusEnable)
            CheckNearby();

        Look();

        aimConstraint.weight = Mathf.SmoothDamp(aimConstraint.weight, desiredWeight, ref weightVelocity, interestSpeed);
    }

    private void CheckNearby()
    {
        if (lookingAtAsked)
            return;

        nearPointOfInterest = null;
        cols = Physics.OverlapSphere(headTransform.position + transform.forward, visionRadius);

        foreach (Collider col in cols)
        {
            if (col.transform == transform)
                continue;

            nearPointOfInterest = col.GetComponent<PointOfInterest>();
            if (nearPointOfInterest != null)
                break;
        }

        targetPosition = origin;

        if (nearPointOfInterest != null)
        {
            focusDir = nearPointOfInterest.GetLookTarget().position - headTransform.position;
            focusDir.Normalize();

            if (Vector3.Dot(focusDir, transform.forward) > rotationRange)
            {
                SetAimWeight(1f);
                targetPosition = nearPointOfInterest.GetLookTarget().position;
            }
            else
            {
                nearPointOfInterest = null;
            }
        }
    }

    private void Look()
    {
        if (nearPointOfInterest == null && !lookingAtAsked)
        {
            targetPosition = headTransform.position + neutralOffset.z * transform.forward;
            SetAimWeight(0f);
        }

        aimTargetTransform.position = Vector3.Lerp(aimTargetTransform.position, targetPosition, Time.deltaTime * lerpSpeed);
    }

    private void SetAimWeight(float endValue)
    {
        desiredWeight = endValue;
        interestSpeed = endValue == 1f ? baseInterestSpeed : baseInterestSpeed * 2f;
    }

    public void OnFocus(InputAction.CallbackContext inputAction)
    {
        if (!inputAction.started)
            return;

        if (focusEnable)
            DisableFocus();
        else
            EnableFocus();
    }
    public void EnableFocus()
    {
        focusEnable = true;
        interestSpeed = baseInterestSpeed;        
    }

    public void DisableFocus()
    {
        focusEnable = false;
        interestSpeed = baseInterestSpeed * 0.8f;
        SetAimWeight(0f);
    }

    public void LookAt(Vector3 position)
    {
        lookingAtAsked = true;
        SetAimWeight(1f);
        targetPosition = position;
    }

    public void CancelLookAt()
    {
        lookingAtAsked = false;
        SetAimWeight(0f);
    }

    public void LookForward()
    {
        SetAimWeight(1f);
    }

    public void CancelLookForward()
    {
        SetAimWeight(0f);
    }

    public Transform GetAimTarget()
    {
        return aimTargetTransform;
    }

    public float GetAimWeight()
    {
        return aimConstraint.weight;
    }

    public void SetWeight(float weight)
    {
        aimConstraint.weight = weight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue * 0.5f;
        Gizmos.DrawWireSphere(headTransform.position + transform.forward, visionRadius);
    }

    private void Reset()
    {
        aimConstraint = GetComponentInChildren<MultiAimConstraint>();
        if(aimConstraint != null)
            aimTargetTransform = aimConstraint.transform.GetChild(0);
    }
}