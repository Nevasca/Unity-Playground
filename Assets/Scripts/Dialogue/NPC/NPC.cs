using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCData data;
    public DialogueData dialogue;
    [SerializeField] private Transform targetTransform;

    private Animator _animator;

    public bool IsInteracting { get; set; }

    private void Start()
    {
        _animator = GetComponent<Animator>();

        if (_animator == null)
            return;

        TMP_Animated animatedText = InterfaceManager.Instance.AnimatedText;
        animatedText.onEmotionChange.AddListener(EmotionChanger);
    }

    private void EmotionChanger(Emotion emotion)
    {
        if (InterfaceManager.Instance.CurrentNPC != this)
            return;

        _animator.SetTrigger(emotion.ToString());
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

    public string GetActionName()
    {
        return "Conversar";
    }

    public void Interact(Transform playerInteracting)
    {
        InterfaceManager.Instance.StartDialogue(this);        
    }

    public void Reset()
    {
        if(_animator != null)
            _animator.SetTrigger("normal");
    }    
}