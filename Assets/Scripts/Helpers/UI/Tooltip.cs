using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : Singleton<Tooltip>
{
    [Header("Params")]
    [SerializeField] private Vector2 _offset;

    [Header("UI")]
    [SerializeField] private RectTransform _tooltipTransform;
    [SerializeField] private TextMeshProUGUI _textContent;

    private Vector2 _heightOffset = Vector2.zero;
    private Vector2 _margin = new Vector2(40f, 40f);

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }

    public void ShowAtPosition(Vector3 position, string content)
    {
        _textContent.text = content;
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipTransform);

        _heightOffset.y = _tooltipTransform.rect.height / 2f;
        //_tooltipTransform.position = position + _offset + _heightOffset;        
        _tooltipTransform.position = position;
        _tooltipTransform.anchoredPosition += _offset + _heightOffset;

        CheckIfCrossingBorder();
    }

    private void CheckIfCrossingBorder()
    {
        //Offset if crossing right screen
        float crossingX = Screen.width - _tooltipTransform.TransformPoint(_tooltipTransform.rect.max).x;

        if (crossingX < 0f)
            _tooltipTransform.anchoredPosition += new Vector2(crossingX - _margin.x, 0f);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}