using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlataform : MonoBehaviour, IActivable
{
    public enum PlataformState { Moving, Waiting, Disabled}

    [SerializeField] private Transform[] points;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private PlataformState currentState;    
    [SerializeField] private bool disableAfterPoints = false;
    [SerializeField] private float rotationSpeed = 20f;

    private Rigidbody rb;
    private AudioSource _audioSource;
    private int indexPoint = 0;
    private float timerWait = 0f;
    private Vector3 distanceToDestination;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up));

        switch (currentState)
        {
            case PlataformState.Moving:
                Move();
                break;
            case PlataformState.Waiting:
                Wait();
                break;
        }
    }

    private void Move()
    {
        float step = speed * Time.deltaTime;
        rb.MovePosition(Vector3.MoveTowards(transform.position, points[indexPoint].position, step));
        

        distanceToDestination = points[indexPoint].position - transform.position;
        if(distanceToDestination.sqrMagnitude < 0.001f)
        {
            _audioSource.SmoothStop();

            if(disableAfterPoints && indexPoint == points.Length - 1)
            {
                currentState = PlataformState.Disabled;
            }
            else
            {
                currentState = PlataformState.Waiting;
                indexPoint = (indexPoint + 1) % points.Length;
            }
        }
    }

    private void Wait()
    {
        timerWait += Time.deltaTime;

        if(timerWait >= waitTime)
        {
            _audioSource.Play();
            currentState = PlataformState.Moving;
            timerWait = 0f;
        }
    }

    public void ToggleActive()
    {
        currentState = currentState == PlataformState.Disabled ? PlataformState.Moving : PlataformState.Disabled;

        //transform.DOShakePosition(0.15f, 0.1f, 20);
        SoundManager.Instance.PlaySFXAt(currentState == PlataformState.Disabled ? 15 : 14, transform.position);
    }

    public bool IsActive()
    {
        return currentState != PlataformState.Disabled;
    }
}