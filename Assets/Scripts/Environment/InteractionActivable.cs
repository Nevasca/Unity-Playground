using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionActivable : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string actionNameActivate;
    [SerializeField] private string actionNameDeactivate;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform handTargetTransform;

    [Header("Feedback")]
    [SerializeField] private Animator animator;
    [SerializeField] private int startInteractionClipID = 12;
    [SerializeField] private int finishInteractionClipID = 13;

    [Header("Activable")]
    [SerializeField] private GameObject activableObject;

    private IActivable _activable;
    private Transform _playerInteracting;


    public bool IsInteracting { get ; set; }

    private void Awake()
    {
        _activable = activableObject.GetComponent<IActivable>();
    }

    public string GetActionName()
    {
        return _activable.IsActive() ? actionNameDeactivate : actionNameActivate;
    }

    public Transform GetTargetTransform()
    {
        if (targetTransform != null)
            return targetTransform;

        return transform;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Interact(Transform playerInteracting)
    {
        IsInteracting = true;
        _playerInteracting = playerInteracting;

        _playerInteracting.GetComponent<RigArm>().Hold(handTargetTransform, 1f, OnArmReached);
        _playerInteracting.GetComponent<LookAtNearby>().CancelLookAt();
        _playerInteracting.GetComponent<PlayerMovement>().Enable(false);

        InterfaceManager.Instance.HideInteractionDisplay();
    }

    private void OnArmReached()
    {
        //Se possuir animacao, ativar e aguardar evento
        if (animator != null)
            animator.SetTrigger(_activable.IsActive() ? "deactivate" : "activate");
        else
            ToggleActivable();

        SoundManager.Instance.PlaySFXAt(startInteractionClipID, transform.position, 0.7f);
    }

    //Animation event
    private void ToggleActivable()
    {
        _activable.ToggleActive();
        SoundManager.Instance.PlaySFXAt(finishInteractionClipID, transform.position, 0.7f);

        _playerInteracting.GetComponent<RigArm>().CancelHold();
        _playerInteracting.GetComponent<PlayerMovement>().Enable(true);

        InterfaceManager.Instance.ShowInteractionDisplay(GetActionName());
        IsInteracting = false;
    }
}