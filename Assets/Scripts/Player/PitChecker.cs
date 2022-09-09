using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PitChecker : MonoBehaviour
{
    [SerializeField] private Transform checkerTransform;
    [SerializeField] private float checkDistance = 1f;

    public const string ANIM_BALANCE = "loosingBalance";

    private PlayerMovement _playerMovement;
    private Animator _animator;
    private bool _nearPit;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        _nearPit = !Physics.Raycast(checkerTransform.position, checkerTransform.forward, checkDistance)
            && _playerMovement.IsGrounded();

        _animator.SetBool(ANIM_BALANCE, _nearPit);
    }   
}