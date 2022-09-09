using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkills : MonoBehaviour
{
    [Header("Skills")]
    [SerializeField] private Transform skillOrigin;
    [SerializeField] private List<Skill> skills;

    [Header("UI")]
    [SerializeField] private SkillMenu skillMenu;
    [SerializeField] private SkillDisplay _skillDisplay;

    private PlayerMovement _playerMovement;

    private Skill _currentSkill;
    private int _currentSkillIndex = -1;

    public bool IsActive { get; private set; } = true;
    public List<Skill> Skills => skills;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();

        InitSkills();

        if(skills != null && skills.Count > 0)
        {
            _currentSkill = skills[0];
            SelectSkill(0);
        }
    }

    private void Start()
    {
        RewindManager.OnStartRewind += OnStartRewind;
        RewindManager.OnStopRewind += OnStopRewind;
    }

    private void InitSkills()
    {
        if (skills == null)
            return;

        for (int i = 0; i < skills.Count; i++)
        {
            skills[i] = Instantiate(skills[i], transform);
            skills[i].Init(skillOrigin);
        }
    }

    public void SelectSkill(int index)
    {
        if (_currentSkillIndex == index)
            return;

        if (index >= skills.Count)
            return;

        _currentSkill.gameObject.SetActive(false);
        _currentSkillIndex = index;
        _currentSkill = skills[_currentSkillIndex];
        _skillDisplay.SetSkill(_currentSkill);
        _currentSkill.gameObject.SetActive(true);
    }

    #region Input
    public void OnSkills(InputAction.CallbackContext input)
    {
        if (input.started && !skillMenu.IsOpen && IsActive)
            ToggleMenu(true);
        else if (input.canceled && skillMenu.IsOpen)
            ToggleMenu(false);
    }

    public void OnPrimarySkill(InputAction.CallbackContext input)
    {
        //Pensar como fazer nao usar outras skills alem de retroceder quando retrocedendo
        //porque se usar o IsActive nao consegue cancelar o retroceder
        if (_currentSkill == null)
            return;

        if (input.started && IsActive)
            _currentSkill.Use(true);
        else if (input.canceled)
            _currentSkill.Use(false);
    }

    public void OnSecondarySkill(InputAction.CallbackContext input)
    {
        if (_currentSkill == null)
            return;

        if (_currentSkill.UsingSecondary)
        {
            if (input.started && IsActive)
                _currentSkill.UseSecondary(true);
            else if (input.canceled)
                _currentSkill.UseSecondary(false);
        }
    }

    public void OnTertiarySkill(InputAction.CallbackContext input)
    {
        if (_currentSkill == null)
            return;

        if (input.started && IsActive)
            _currentSkill.UseTertiary(true);
        else if (input.canceled)
            _currentSkill.UseTertiary(false);

    }
    #endregion

    #region Animation Events
    public void SkillCast()
    {
        _currentSkill?.EventSkillCast();
    }

    public void SkillEnded()
    {
        _currentSkill?.EventSkillEnded();
    }
    #endregion

    private void ToggleMenu(bool open)
    {
        if(open)
            skillMenu.OpenMenu();
        else
            skillMenu.CloseMenu();

        _playerMovement.Enable(!open);
        IsActive = !open;
    }

    #region Rewind
    private void OnStartRewind()
    {
        IsActive = false;
        _currentSkill?.Enable(false);
    }

    private void OnStopRewind()
    {
        IsActive = true;
        _currentSkill?.Enable(true);
    }
    #endregion

    public void Enable(bool enable)
    {
        IsActive = enable;
    }

    private void OnDestroy()
    {
        RewindManager.OnStartRewind -= OnStartRewind;
        RewindManager.OnStopRewind -= OnStopRewind;
    }
}