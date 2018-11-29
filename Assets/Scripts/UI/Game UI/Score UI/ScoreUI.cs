using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    private bool _waiting = false;

    private int _value = 1;
    private int _target = 1;

    private Sequence _sequence = null;

    //[SerializeField]
    //private int _speed = 1;

    [Header("Interpolates score value to new value while increasing size, then goes back ")]
    [Header("to OG size. Particles spawn at max size.")]

    [SerializeField]
    private UIParticleSystem _particle = null;
    [SerializeField]
    private Text _scoreObject = null;
    
    [SerializeField] private float _scale = 1.5f;

    [SerializeField] private float _scaleUpDuration = 2;
    [SerializeField] private float _scaleDownDuration = 1;

    // Use this for initialization
    private void Start()
    {
        //DOTween.SetTweensCapacity(10000, 500);
    }

    private void OnEnable()
    {
        GameRules.onScoreChange += SetTarget;
    }

    private void OnDisable()
    {
        if (GameRules.onScoreChange != null)
            GameRules.onScoreChange -= SetTarget;
    }

    private void Update()
    {
        if (_waiting) return;
        UpdateText(_value);
    }

    // Update is called once per frame
    private void UpdateText(int score)
    {
        if (_scoreObject != null)
            _scoreObject.text = score.ToString();
    }

    public void SetTarget(int value)
    {
        //StopAllCoroutines();

        _target = value;
        //_target = 46356;
        //_value = 314;
        float totalDuration = _scaleUpDuration;
        float explosionDuration = _scaleDownDuration;
        float scale = _scale;
        int fontSize = _scoreObject.fontSize;
        Vector2 rectTransformSizeDelta = _scoreObject.rectTransform.sizeDelta;

        if (_sequence != null) return;

        _sequence = SequenceGradual(totalDuration, explosionDuration, scale, rectTransformSizeDelta, fontSize);

        //Performance stuff
        _sequence.Pause().SetAutoKill(true).SetRecyclable();

        _sequence.Play();

        _sequence.OnComplete(() => StartCoroutine(MyCoroutines.WaitOneFrame(() => _sequence = null)));
    }

    private Sequence SequenceGradual(float duration, float explosionDuration, float scale, Vector2 size, int fontSize)
    {
        Sequence sequence = DOTween.Sequence()
                .Append(DOTween.To(() => _value, x => _value = x, _target, duration))
                .Insert(0, _scoreObject.rectTransform.DOSizeDelta(size * scale, duration, true))
                .Insert(0,
                    DOTween.To(() => _scoreObject.fontSize, x => _scoreObject.fontSize = (int) x, fontSize * scale,
                        duration))
                .Insert(duration, _scoreObject.rectTransform.DOSizeDelta(size, explosionDuration, true))
                .Insert(duration,
                    DOTween.To(() => _scoreObject.fontSize, x => _scoreObject.fontSize = x, fontSize,
                        explosionDuration))
            ;

        if (_particle != null)
            sequence.InsertCallback(duration - 0.5f, () => _particle.Play());

        return sequence;
    }
}
