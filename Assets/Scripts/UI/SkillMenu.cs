using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Cinemachine;
using Tweening;

public class SkillMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerSkills playerSkills;

    [Header("UI")]
    [SerializeField] private GameObject display;
    [SerializeField] private CanvasGroup displayCanvas;
    [SerializeField] private SkillMenuItem[] skillItems;

    [Header("Effects")]
    [SerializeField] private Volume menuEffect;
    [SerializeField] private CinemachineBrain cameraBrain;

    private Vector2 _point;
    private Vector2 _normalizedPosition;
    private float _currentAngle;
    private int _selection;
    private int _previousSelection;
    private bool _canSelect = false;
    private float _anglePerSection;

    private PlayerControls _playerControls;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _playerControls.Enable();
        display.transform.localScale = Vector3.zero;
        _anglePerSection = 360f / skillItems.Length;
    }

    private void Start()
    {
        SetSkills();
    }

    private void SetSkills()
    {
        int i = 0;

        foreach (var skill in playerSkills.Skills)
            skillItems[i++].SetSkill(skill);

        for (int j = i; i < skillItems.Length; j++)
            skillItems[j].SetSkill(null);
    }

    public void OpenMenu()
    {
        display.SetActive(true);
        cameraBrain.enabled = false;        
        TweenSequence seq = Tweener.Sequence();
        seq.Append(display.transform.DoScale(1f, 0.25f).SetEase(Ease.OutBack));
        seq.Join(displayCanvas.DoFade(1f, 0.25f));
        seq.Join(menuEffect.DoWeight(1f, 0.25f));
        seq.Join(TweenVirtual.DoFloat(1f, 0.1f, 0.25f, DoTimeScale));
        seq.AppendCallback(() => {
            Cursor.lockState = CursorLockMode.Confined;
            _canSelect = true;            
        });

        seq.Play();

        IsOpen = true;
    }

    public void CloseMenu()
    {
        _canSelect = false;
        cameraBrain.enabled = true;
        playerSkills.SelectSkill(_selection);

        TweenSequence seq = Tweener.Sequence();
        seq.Append(display.transform.DoScale(0f, 0.1f).SetEase(Ease.InBack));
        seq.Join(displayCanvas.DoFade(0f, 0.1f));
        seq.Join(menuEffect.DoWeight(0f, 0.1f));
        seq.Join(TweenVirtual.DoFloat(0f, 1f, 0.1f, DoTimeScale));
        seq.AppendCallback(() => {
            Cursor.lockState = CursorLockMode.Locked;
            display.SetActive(false);
            IsOpen = false;
        });

        seq.Play();
    }

    private void DoTimeScale(float x)
    {
        Time.timeScale = x;
    }

    private void Update()
    {
        if (!_canSelect)
            return;

        _point = _playerControls.UI.Point.ReadValue<Vector2>();

        _normalizedPosition = new Vector2(_point.x - Screen.width / 2, _point.y - Screen.height / 2);
        _currentAngle = Mathf.Atan2(_normalizedPosition.y, _normalizedPosition.x) * Mathf.Rad2Deg;
        _currentAngle = (_currentAngle + 360) % 360;

        _selection = (int) (_currentAngle / _anglePerSection); // 360 / total sessoes

        if(_selection != _previousSelection)
        {
            skillItems[_previousSelection].Deselect();
            _previousSelection = _selection;
            skillItems[_selection].Select();
        }
    }


    private void OnDestroy()
    {
        _playerControls.Disable();
    }
}