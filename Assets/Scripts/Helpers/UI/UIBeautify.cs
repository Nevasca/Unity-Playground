using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Tweening;

public class UIBeautify : MonoBehaviour
{
    [SerializeField] private float _effectDuration = 0.1f;
    [SerializeField] private bool _deactivateParent = false;
    [SerializeField] private Image _overlay;
    [SerializeField] private float _overlayOpacity = 0.85f;
    [SerializeField] private UnityEvent _onShow;
    [SerializeField] private UnityEvent _onHide;

    private bool _inProgress; //Flag to check if gameobject can be disabled

    public void Show()
    {
        _onShow?.Invoke();
    }

    public void Hide()
    {
        _onHide?.Invoke();
    }

    #region In Effects
    public void SlideInX(float x)
    {
        Enable();
        GetComponent<RectTransform>().DoAnchorPosX(x, _effectDuration).OnComplete(() => {
            _inProgress = false;
        });
    }

    public void SlidePunchInX(float x)
    {
        Enable();
        RectTransform rectTransform = GetComponent<RectTransform>();

        float defaultScale = rectTransform.localScale.x;

        TweenSequence s = Tweener.Sequence();
        s.Append(rectTransform.DoAnchorPosX(x, _effectDuration));
        s.Append(rectTransform.DoScale(defaultScale + 0.05f, _effectDuration / 2f));
        s.Append(rectTransform.DoScale(defaultScale, _effectDuration / 2f));
        s.AppendCallback(() =>
        {
            _inProgress = false;
        });

        s.Play();

        //rectTransform.DoAnchorPosX(x, _effectDuration).OnComplete(() => {
        //    //rectTransform.DOPunchScale(rectTransform.localScale * 0.05f, _effectDuration);
        //    _inProgress = false;
        //});
    }

    public void SlideBounceInX(float x)
    {
        Enable();
        GetComponent<RectTransform>().DoAnchorPosX(x, _effectDuration).SetEase(Ease.OutBack).OnComplete(() => {
            _inProgress = false;
        });
    }

    public void SlideInY(float y)
    {
        Enable();
        GetComponent<RectTransform>().DoAnchorPosY(y, _effectDuration).OnComplete(() => {
            _inProgress = false;
        });
    }

    public void SlidePunchInY(float y)
    {
        Enable();
        RectTransform rectTransform = GetComponent<RectTransform>();

        //rectTransform.DoAnchorPosY(y, _effectDuration).OnComplete(() => {
        //    //rectTransform.DOPunchScale(rectTransform.localScale * 0.05f, _effectDuration);
        //    _inProgress = false;
        //});

        float defaultScale = rectTransform.localScale.x;

        TweenSequence s = Tweener.Sequence();
        s.Append(rectTransform.DoAnchorPosY(y, _effectDuration));
        s.Append(rectTransform.DoScale(defaultScale + 0.05f, _effectDuration / 2f));
        s.Append(rectTransform.DoScale(defaultScale, _effectDuration / 2f));
        s.AppendCallback(() =>
        {
            _inProgress = false;
        });

        s.Play();
    }

    public void SlideBounceInY(float y)
    {
        Enable();
        GetComponent<RectTransform>().DoAnchorPosY(y, _effectDuration).SetEase(Ease.OutBack).OnComplete(() => {
            _inProgress = false;
        });
    }

    public void PopIn(float value)
    {
        Enable();
        GetComponent<CanvasGroup>()?.DoFade(1f, _effectDuration);
        GetComponent<RectTransform>().DoScale(value, _effectDuration).SetEase(Ease.OutBack).OnComplete(()=> {
            _inProgress = false;
        });
    }
    #endregion

    #region Out Effects
    public void SlideOutX(float x)
    {
        GetComponent<RectTransform>().DoAnchorPosX(x, _effectDuration).OnComplete(Disable);
        _overlay?.DoFade(0f, _effectDuration);
    }

    public void SlideOutY(float y)
    {
        GetComponent<RectTransform>().DoAnchorPosY(y, _effectDuration).OnComplete(Disable);
        _overlay?.DoFade(0f, _effectDuration);
    }

    public void PopOut()
    {
        GetComponent<CanvasGroup>()?.DoFade(0f, _effectDuration);
        GetComponent<RectTransform>().DoScale(0f, _effectDuration).SetEase(Ease.InBack).OnComplete(Disable);
        _overlay?.DoFade(0f, _effectDuration);
    }
    #endregion

    private void Enable()
    {
        gameObject.SetActive(true);

        if (_deactivateParent)
            gameObject.transform.parent.gameObject.SetActive(true);

        _inProgress = true;

        if (_overlay == null)
            return;

        _overlay.gameObject.SetActive(true);
        _overlay.DoFade(_overlayOpacity, _effectDuration);
    }

    private void Disable()
    {
        //Make sure no other effect was called
        if (_inProgress)
            return;

        gameObject.SetActive(false);

        if (_deactivateParent)
            gameObject.transform.parent.gameObject.SetActive(false);

        if (_overlay == null)
            return;

        _overlay.gameObject.SetActive(false);
    }
}