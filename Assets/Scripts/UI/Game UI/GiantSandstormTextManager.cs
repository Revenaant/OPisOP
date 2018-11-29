using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GiantSandstormTextManager : MonoBehaviour
{
    //private Tweener _tweener = null;
    private Sequence _sequenceBig = null;
    private Sequence _sequenceSmall = null;

    private Storms _storms = null;
    private Storms Storms
    {
        get
        {
            if (_storms == null)
            {
                if (GameManager.Instance != null &&
                    GameManager.Instance.Storms != null)
                    _storms = GameManager.Instance.Storms;
                else _storms = FindObjectOfType<Storms>();
            }

            return _storms;
        }
    }

    [SerializeField] private Text _bigText = null;
    [SerializeField] private Text _smallText = null;
    [SerializeField] private MinMaxPair _alphaRange = new MinMaxPair(0, 1);
    [SerializeField] private float _intermissionBigDuration = 0.5f;
    [SerializeField] private float _intermissionSmallDuration = 0.25f;
    [SerializeField] private TweenElement _fadeIn = new TweenElement(1, Ease.InQuad);
    [SerializeField] private TweenElement _fadeOut = new TweenElement(1, Ease.OutQuad);

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(_bigText != null, "Assign Small Text for Giant Sandstorm Indication! :(");
        Debug.Assert(_smallText != null, "Assign Big Text for Giant Sandstorm Indication! :(");
        //_dayText.CrossFadeAlpha(_alphaRange.Min, 0.01f, false);
        _bigText.DOFade(_alphaRange.Min, 0.01f);
        _smallText.DOFade(_alphaRange.Min, 0.01f);

        Tweener faderInSmall = _smallText.DOFade(_alphaRange.Max, _fadeIn.duration).SetEase(_fadeIn.ease).SetTarget(null).Pause();
        Tweener faderOutSmall = _smallText.DOFade(_alphaRange.Min, _fadeOut.duration).SetEase(_fadeOut.ease).SetTarget(null).Pause();

        Tweener faderInBig = _bigText.DOFade(_alphaRange.Max, _fadeIn.duration).SetEase(_fadeIn.ease).SetTarget(null).Pause();
        Tweener faderOutBig = _bigText.DOFade(_alphaRange.Min, _fadeOut.duration).SetEase(_fadeOut.ease).SetTarget(null).Pause();

        _sequenceSmall = DOTween.Sequence().Append(faderInSmall).AppendInterval(_intermissionSmallDuration).
            Append(faderOutSmall).SetAutoKill(false).Pause();
        _sequenceBig = DOTween.Sequence().Append(faderInBig).AppendInterval(_intermissionBigDuration).
            Append(faderOutBig).SetAutoKill(false).Pause();
    }

    private void OnEnable()
    {
        if (Storms != null)
        {
            Storms.onStormStart.AddListener(StormStartHandler);
            Storms.onStormEnd.AddListener(StormEndHandler);
        }
    }

    private void OnDisable()
    {
        if (Storms != null)
        {
            Storms.onStormStart.RemoveListener(StormStartHandler);
            Storms.onStormEnd.RemoveListener(StormEndHandler);
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.I))
    //        DayChangeHandler(_day + 1);
    //}

    //private void DayChangeHandler(int day)
    //{
    //    _day = day;

    //    if (_dayText == null || day == 1) return;

    //    _dayText.text = _textPrefix + day;

    //    _sequenceBig.Play().Restart();
    //}

    private void StormStartHandler()
    {
        _sequenceBig.Play().Restart();

        float lag = _fadeIn.duration + _intermissionBigDuration;
        //When the big text starts fading out - start fading in small one
        StartCoroutine(MyCoroutines.Wait(lag,
            () =>
            {
                _sequenceSmall.Play().Restart();
                //After small one finishes fading in Pause it until the storms is done. Then after it's done, finish.
                //lag += _fadeIn.duration;
                StartCoroutine(MyCoroutines.Wait(_fadeIn.duration, () => _sequenceSmall.Pause()));
            }));

    }

    private void StormEndHandler()
    {
        _sequenceSmall.Play();
    }
}
