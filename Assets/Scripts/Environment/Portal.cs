using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweening;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [field: SerializeField]
    public Portal OtherPortal { get; private set; }

    [SerializeField] private LayerMask placementMask;
    [SerializeField] private Transform testTransform;

    private Collider _wallCollider;
    private BoxCollider _collider;

    private List<PortalableObject> _portalObjects = new List<PortalableObject>();

    public Renderer Renderer { get; set; }
    public bool IsPlaced { get; set; } = false;

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        _collider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Renderer.enabled = OtherPortal.IsPlaced;

        for (int i = 0; i < _portalObjects.Count; ++i)
        {
            Vector3 objPos = transform.InverseTransformPoint(_portalObjects[i].transform.position);

            if (objPos.z > 0f)
                _portalObjects[i].Warp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PortalableObject obj))
        {
            _portalObjects.Add(obj);
            obj.SetIsInPortal(this, OtherPortal, _wallCollider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out PortalableObject obj))
        {
            _portalObjects.Remove(obj);
            obj.ExitPortal(_wallCollider);
        }
    }

    public bool PlacePortal(Collider wallCollider, Vector3 pos, Quaternion rot)
    {
        testTransform.position = pos;
        testTransform.rotation = rot;
        testTransform.position -= testTransform.forward * 0.001f;

        FixOverhangs();
        FixIntersects();

        if (CheckOverlap())
        {
            this._wallCollider = wallCollider;
            transform.position = testTransform.position;
            transform.rotation = testTransform.rotation;

            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            transform.DoScale(1f, 0.2f);
            SoundManager.Instance.PlaySFXAt(35, transform.position, minDistance: 3f);

            IsPlaced = true;
            return true;
        }

        return false;
    }

    //Ensures portal cannot extends past edge of a surface
    private void FixOverhangs()
    {
        var testPoints = new List<Vector3>
        {
            new Vector3(-0.6f, 0f, 0.1f),
            new Vector3(0.6f, 0f, 0.1f),
            new Vector3(0f, -1f, 0.1f),
            new Vector3(0f, 1f, 0.1f)
        };

        var testDirs = new List<Vector3>
        {
            Vector3.right,
            -Vector3.right,
            Vector3.up,
            -Vector3.up
        };

        for (int i = 0; i < 4; ++i)
        {
            RaycastHit hit;
            Vector3 raycastPos = testTransform.TransformPoint(testPoints[i]);
            Vector3 raycastDir = testTransform.TransformDirection(testDirs[i]);

            if (Physics.CheckSphere(raycastPos, 0.05f, placementMask))
            {
                break;
            }
            else if(Physics.Raycast(raycastPos, raycastDir, out hit, 1f, placementMask))
            {
                var offset = hit.point - raycastPos;
                testTransform.Translate(offset, Space.World);
            }
        }
    }

    //Ensures portal is not intersecting walls
    private void FixIntersects()
    {
        var testDirs = new List<Vector3>
        {
            Vector3.right,
            -Vector3.right,
            Vector3.up,
            -Vector3.up
        };

        var testDists = new List<float> { 0.6f, 0.6f, 1f, 1f};
        for (int i = 0; i < 4; ++i)
        {
            RaycastHit hit;
            Vector3 raycastPos = testTransform.TransformPoint(0f, 0f, -0.1f);
            Vector3 raycastDir = testTransform.TransformDirection(testDirs[i]);

            if(Physics.Raycast(raycastPos, raycastDir, out hit, testDists[i], placementMask))
            {
                var offset = (hit.point - raycastPos);
                var newOffset = -raycastDir * (testDists[i] - offset.magnitude);
                testTransform.Translate(newOffset, Space.World);
            }
        }
    }

    //After positioning, ensure portal is not intersecting anything
    private bool CheckOverlap()
    {
        var checkExtents = new Vector3(0.4f, 0.8f, 0.05f);

        var checkPositions = new Vector3[]
        {
            testTransform.position + testTransform.TransformVector(new Vector3(0f, 0f, -0.1f)),

            testTransform.position + testTransform.TransformVector(new Vector3(-0.5f, -0.9f, -0.1f)),
            testTransform.position + testTransform.TransformVector(new Vector3(-0.5f, 0.9f, -0.1f)),
            testTransform.position + testTransform.TransformVector(new Vector3(0.5f, -0.9f, -0.1f)),
            testTransform.position + testTransform.TransformVector(new Vector3(0.5f, 0.9f, -0.1f)),

            testTransform.TransformVector(new Vector3(0f, 0f, 0.2f))
        };

        var intersections = Physics.OverlapBox(checkPositions[0], checkExtents, testTransform.rotation, placementMask);

        if(intersections.Length > 1)
        {
            return false;
        }
        else if(intersections.Length == 1)
        {
            //Can intersect with old portal if that's the case
            if (intersections[0] != _collider)
                return false;
        }

        //Ensure the portal corners overlap a surface
        bool isOverlapping = true;

        for (int i = 1; i < checkPositions.Length - 1; ++i)
        {
            isOverlapping &= Physics.Linecast(checkPositions[i], 
                checkPositions[i] + checkPositions[checkPositions.Length - 1], placementMask);
        }

        return isOverlapping;
    }
}