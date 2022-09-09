using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class InteractionTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup targetGroup;

    private IInteractable _interactableInRange;
    private PlayerInput _playerInput;
    private LookAtNearby _playerLook;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerLook = GetComponent<LookAtNearby>();
        InterfaceManager.OnDialogueStarted += OnDialogueStarted;
        InterfaceManager.OnDialogueFinished += OnDialogueFinished;
    }

    public void OnInteract(InputAction.CallbackContext inputAction)
    {
        if (inputAction.started)
            Interact();
    }

    public void OnTalk(InputAction.CallbackContext inputAction)
    {
        if(inputAction.started && InterfaceManager.Instance.InDialogue)
            InterfaceManager.Instance.OnTalk();
    }

    private void Interact()
    {
        if (_interactableInRange == null)
            return;

        if(!_interactableInRange.IsInteracting)
        {
            Transform lookTransform = _interactableInRange.GetTransform();
            PointOfInterest poi = lookTransform.GetComponent<PointOfInterest>();
            if (poi != null)
                lookTransform = poi.GetLookTarget();
            _playerLook.LookAt(lookTransform.position);

            _interactableInRange.Interact(transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Interactable"))
        {
            _interactableInRange = other.transform.GetComponent<IInteractable>();
            targetGroup.m_Targets[1].target = _interactableInRange.GetTargetTransform();
            InterfaceManager.Instance.ShowInteractionDisplay(_interactableInRange.GetActionName());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Interactable"))
        {
            _interactableInRange = null;
            InterfaceManager.Instance.HideInteractionDisplay();
        }
    }

    private void OnDialogueStarted()
    {
        _playerInput.SwitchCurrentActionMap("Dialogue");
    }

    private void OnDialogueFinished()
    {
        _playerInput.SwitchCurrentActionMap("Player");
        _playerLook.CancelLookAt();
    }

    private void OnDestroy()
    {
        InterfaceManager.OnDialogueStarted -= OnDialogueStarted;
        InterfaceManager.OnDialogueFinished -= OnDialogueFinished;
    }
}