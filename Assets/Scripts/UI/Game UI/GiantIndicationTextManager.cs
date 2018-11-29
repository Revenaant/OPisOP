using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GiantIndicationTextManager : MonoBehaviour
{
    //private Tweener _tweener = null;
    private Sequence _sequence = null;
    //private int _day = 0;

    [SerializeField] private Text _dayText = null;
    [SerializeField] private string _textPrefix = "Dag ";
    [SerializeField] private MinMaxPair _alphaRange = new MinMaxPair(0, 1);
    [SerializeField] private float _intermissionDuration = 0.5f;
    [SerializeField] private TweenElement _fadeIn = new TweenElement(2, Ease.InQuad);
    [SerializeField] private TweenElement _fadeOut = new TweenElement(2, Ease.OutQuad);

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(_dayText != null, "Assign Text for Giant Indication! :(");
        //_dayText.CrossFadeAlpha(_alphaRange.Min, 0.01f, false);
        _dayText.DOFade(_alphaRange.Min, 0.01f);

        Tweener faderIn = _dayText.DOFade(_alphaRange.Max, _fadeIn.duration).SetEase(_fadeIn.ease).SetTarget(null).Pause();
        Tweener faderOut = _dayText.DOFade(_alphaRange.Min, _fadeOut.duration).SetEase(_fadeOut.ease).SetTarget(null).Pause();

        _sequence = DOTween.Sequence().Append(faderIn).AppendInterval(_intermissionDuration).Append(faderOut).SetAutoKill(false).Pause();
    }

    private void OnEnable()
    {
        GameRules.onDayChange += DayChangeHandler;
    }

    private void OnDisable()
    {
        if (GameRules.onDayChange != null)
            GameRules.onDayChange -= DayChangeHandler;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.I))
    //        DayChangeHandler(_day + 1);
    //}

    private void DayChangeHandler(int day)
    {
        //_day = day;

        if (_dayText == null || day == 1) return;

        _dayText.text = _textPrefix + day;

        _sequence.Play().Restart();
    }
}
