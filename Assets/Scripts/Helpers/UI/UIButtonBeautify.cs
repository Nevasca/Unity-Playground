using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Tweening;
using UnityEngine.UI;

#if UNITY_STANDALONE || UNITY_WEBGL
public class UIButtonBeautify : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
#elif UNITY_ANDROID
public class UIButtonBeautify : MonoBehaviour, ISelectHandler, IDeselectHandler
#endif
{
    public enum ClickEffect { None, Punch};

    [SerializeField] private Transform _target;
    [SerializeField] private ClickEffect _clickEffect = ClickEffect.None;
    [SerializeField] private float _punchScale = 0.15f;

    [Header("On Focused")]
    [SerializeField] private Vector3 _defaultScale = Vector3.one;
    [SerializeField] private float _scaleFactor = 1.1f;
    [SerializeField] private float _transitionDuration = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioClip _clipFocused;
    [SerializeField] private float _focusedVolume = 0.3f;
    [SerializeField] private AudioClip _clipIdClick;
    [SerializeField] private float _pitch = 1;

    private TweenSequence _seq;
    private Coroutine _seqCor;
    private bool _hovering;

    private void Awake()
    {
        if (TryGetComponent(out Button button))
            button.onClick.AddListener(OnClick);
        else if (TryGetComponent(out Toggle toggle))
            toggle.onValueChanged.AddListener(OnValueChanged);
    }

    //OnPointerEnter e OnSelect
    private void OnFocused()
    {
        if (!GetComponent<Selectable>().interactable || _hovering)
            return;

        SoundManager.Instance.PlaySFX(_clipFocused, _focusedVolume, pitch: _pitch);

        if (_seqCor != null)
            StopCoroutine(_seqCor);

        _seqCor = StartCoroutine(StartFocusedLoopCoroutine());
    }

    private IEnumerator StartFocusedLoopCoroutine()
    {
        yield return null;

        _target.DoScale(_defaultScale * _scaleFactor, _transitionDuration);

        //_seq?.Kill();

        //_target.DoScale(_defaultScale * _scaleFactor, _transitionDuration).OnComplete(() =>
        //{
        //    _seq = Tweener.Sequence();
        //    _seq.Append(_target.DoScale(_defaultScale, 0.7f));
        //    _seq.Append(_target.DoScale(_defaultScale * _scaleFactor, 0.7f));
        //    _seq.SetLoops(-1);
        //    _seq.Play();
        //});
    }

    private void OnNotFocused()
    {
        _seq?.Kill();
        if (_seqCor != null)
            StopCoroutine(_seqCor);

        _target.DoScale(_defaultScale, _transitionDuration);
    }

    public void OnClick()
    {
        SoundManager.Instance.PlaySFX(_clipIdClick, 0.8f, pitch: _pitch);
        DoClickEffect();
    }

    public void OnValueChanged(bool value)
    {
        if (!value)
            return;

        OnClick();
    }

    public void DoClickEffect()
    {
        if (_seqCor != null)
            StopCoroutine(_seqCor);

        if (_clickEffect == ClickEffect.Punch)
            _target.DoPunchScale(_target.localScale * _punchScale, _transitionDuration);
    }

    #region Event triggers
#if UNITY_STANDALONE || UNITY_WEBGL
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnFocused();
        _hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnNotFocused();
        _hovering = false;
    }
#endif

    public void OnSelect(BaseEventData eventData)
    {
        OnFocused();
#if UNITY_ANDROID
        hovering = true;
#endif
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnNotFocused();
#if UNITY_ANDROID
        hovering = false;
#endif
    }
    #endregion

    private void OnDisable()
    {
        _seq?.Kill();
        if (_seqCor != null)
            StopCoroutine(_seqCor);

        _hovering = false;
    }

    private void Reset()
    {
        _target = transform;
    }
}