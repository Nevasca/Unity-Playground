using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string GetActionName();
    void Interact(Transform playerInteracting);
    Transform GetTransform();
    Transform GetTargetTransform();
    bool IsInteracting { get; set; }
}