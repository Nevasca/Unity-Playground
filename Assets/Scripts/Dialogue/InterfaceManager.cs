using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;
using System.Globalization;
using System;
using Tweening;

public class InterfaceManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_Animated animatedText;
    [SerializeField] Image nameBox;
    [SerializeField] Image dialogueBox;
    [SerializeField] TextMeshProUGUI nameTMP;
    [SerializeField] Sprite npcDialogueSprite;
    [SerializeField] Sprite itemDialogueSprite;

    [Space]

    [Header("Cameras")]
    [SerializeField] GameObject gameCam;
    [SerializeField] GameObject dialogueCam;

    [Space]

    [Space]

    [Header("Dialogue Visual")]
    [SerializeField] private Volume dialogueDof;

    [Space]

    [Header("Interaction Display")]
    [SerializeField] private RectTransform interactionHUD;
    [SerializeField] private CanvasGroup interactionCanvasGroup;
    [SerializeField] private TextMeshProUGUI interactionNameText;

    private int _dialogueIndex;
    private bool _canExit;
    private bool _nextDialogue;
    private Transform _player;    

    public bool InDialogue { get; private set; }
    //public NPC CurrentNPC { get; private set; }
    public NPC CurrentNPC { get; private set; }
    public TMP_Animated AnimatedText { get { return animatedText; }}

    public static event Action OnDialogueStarted;
    public static event Action OnDialogueFinished;

    public static InterfaceManager Instance;

    private void Awake()
    {
        Instance = this;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        animatedText.onDialogueFinish.AddListener(FinishDialogue);
    }

    #region Dialogue
    public void OnTalk()
    {
        if(InDialogue)
        {
            if(_canExit)
            {
                CameraChange(false);
                FadeUI(false, 0.2f, 0f);
                TweenSequence s = Tweener.Sequence();
                s.AppendInterval(0.5f);
                s.AppendCallback(ResetState);
                s.AppendCallback(() => FadeInteractionUI(true, 0.15f, 0f));
                s.Play();
            }

            if(_nextDialogue)
            {
                animatedText.ReadText(CurrentNPC.dialogue.conversationBlock[_dialogueIndex]);
            }
        }
    }

    private void FadeUI(bool show, float time, float delay)
    {
        TweenSequence s = Tweener.Sequence();
        s.AppendInterval(delay);
        s.Append(canvasGroup.DoFade(show ? 1f : 0f, time));
        if(show)
        {
            _dialogueIndex = 0;

            canvasGroup.transform.localScale = Vector3.zero;
            s.Join(canvasGroup.transform.DoScale(1f, time * 2f).SetEase(Ease.OutBack));
            s.AppendCallback(() => animatedText.ReadText(CurrentNPC.dialogue.conversationBlock[0]));
        }
        s.Play();
    }

    private void SetCharNameAndColor()
    {
        nameTMP.text = CurrentNPC.data.npcName;
        nameTMP.color = CurrentNPC.data.npcNameColor;
        nameBox.color = CurrentNPC.data.npcColor;
    }

    public void StartDialogue(NPC npc)
    {
        CurrentNPC = npc;
        FindObjectOfType<PlayerMovement>().Enable(false);

        bool isACharacter = npc.data != null;
        if (isACharacter)
            SetCharNameAndColor();

        dialogueBox.sprite = isACharacter ? npcDialogueSprite : itemDialogueSprite;
        nameBox.gameObject.SetActive(isACharacter);

        InDialogue = true;
        CameraChange(true);
        animatedText.text = string.Empty;
        FadeUI(true, 0.2f, 0.65f);
        FadeInteractionUI(false, 0.15f, 0f);
        OnDialogueStarted?.Invoke();
    }

    private void CameraChange(bool dialogue)
    {
        gameCam.SetActive(!dialogue);
        dialogueCam.SetActive(dialogue);

        //Depth of field modifier
        float dofWeight = dialogueCam.activeSelf ? 1 : 0;
        dialogueDof.DoWeight(dofWeight, 0.8f);
    }

    private void ResetState()
    {
        CurrentNPC.Reset();
        FindObjectOfType<PlayerMovement>().Enable(true);
        InDialogue = false;
        _canExit = false;
        OnDialogueFinished?.Invoke();
    }

    private void FinishDialogue()
    {
        if(_dialogueIndex < CurrentNPC.dialogue.conversationBlock.Count - 1)
        {
            _dialogueIndex++;
            _nextDialogue = true;
        }
        else
        {
            _nextDialogue = false;
            _canExit = true;
        }
    }
    #endregion

    #region Interaction
    public void ShowInteractionDisplay(string actionName)
    {
        interactionNameText.text = actionName;
        LayoutRebuilder.ForceRebuildLayoutImmediate(interactionHUD);
        FadeInteractionUI(true, 0.15f, 0f);
    }

    public void HideInteractionDisplay()
    {
        FadeInteractionUI(false, 0.15f, 0f);
    }

    private void FadeInteractionUI(bool show, float time, float delay)
    {
        TweenSequence s = Tweener.Sequence();
        s.AppendInterval(delay);
        s.Append(interactionCanvasGroup.DoFade(show ? 1f : 0f, time));

        if (show)
        {
            _dialogueIndex = 0;
            interactionCanvasGroup.transform.localScale = Vector3.zero;
            s.Join(interactionCanvasGroup.transform.DoScale(1f, time * 2f).SetEase(Ease.OutBack));
        }

        s.Play();
    }
    #endregion
}