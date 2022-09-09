using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDisplay : MonoBehaviour
{
    [SerializeField] private Image _icon;

    [Header("Ammo Display")]
    [SerializeField] private GameObject _ammoDisplay;
    [SerializeField] private TextMeshProUGUI _textAmmoMagazine;
    [SerializeField] private TextMeshProUGUI _textAmmo;

    private Skill _currentSkill;

    public void SetSkill(Skill skill)
    {
        RemovePreviousSkill();

        _currentSkill = skill;
        _icon.sprite = skill.Icon;

        CheckIfUsesAmmo();
    }

    private void CheckIfUsesAmmo()
    {
        Machinegun machinegun = _currentSkill as Machinegun;

        if (machinegun == null)
        {
            _ammoDisplay.SetActive(false);
            return;
        }

        machinegun.OnAmmoChanged += OnAmmoChanged;
        machinegun.OnAmmoMagazinelChanged += OnAmmoMagazineChanged;
        OnAmmoChanged(machinegun.CurrentAmmo);
        OnAmmoMagazineChanged(machinegun.CurrentAmmoMagazine);
        _ammoDisplay.SetActive(true);
    }

    private void RemovePreviousSkill()
    {
        if (_currentSkill == null)
            return;

        Machinegun machinegun = _currentSkill as Machinegun;

        if (machinegun == null)
            return;

        machinegun.OnAmmoChanged -= OnAmmoChanged;
        machinegun.OnAmmoMagazinelChanged -= OnAmmoMagazineChanged;
    }

    private void OnAmmoChanged(int ammo)
    {
        _textAmmo.text = ammo.ToString();

    }

    private void OnAmmoMagazineChanged(int ammo)
    {
        _textAmmoMagazine.text = ammo.ToString();
    }
}