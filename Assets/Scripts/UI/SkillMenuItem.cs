using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tweening;

public class SkillMenuItem : MonoBehaviour
{
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _hoverColor;
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;

    private void Start()
    {
        _background.color = _baseColor;
    }

    public void SetSkill(Skill skill)
    {
        if(skill == null)
        {
            _icon.gameObject.SetActive(false);
            return;
        }

        _icon.sprite = skill.Icon;
    }

    public void Select()
    {
        _background.color = _hoverColor;
        SoundManager.Instance.PlaySFX(21, 0.4f);
        transform.DoScale(1.05f, 0.15f).SetEase(Ease.OutBack);
    }

    public void Deselect()
    {
        _background.color = _baseColor;
        transform.DoScale(1f, 0.15f).SetEase(Ease.InBack);
    }
}