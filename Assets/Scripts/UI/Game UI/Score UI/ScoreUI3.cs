using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI3 : MonoBehaviour
{
    private int _value = 1;
    private int _target = 1;

    private Sequence _sequence = null;

    //[SerializeField]
    //private int _speed = 1;
    [Header("Interpolates score value to user-defined capstone(s), then pulses (size increases)")]
    [Header("and decreases quickly), " + "the finally explodes (pulses at higher size and spawns particles")]
    [SerializeField]
    private UIParticleSystem _particle = null;
    [SerializeField]
    private Text _scoreObject = null;

    [SerializeField] private float _scalePopUp = 1.5f;
    [SerializeField] private float _scaleExplosion = 2;

    [SerializeField] private float _popupBaseDuration = 5;
    [SerializeField] private float _endExplosionDuration = 1;

    [SerializeField] private List<int> _scoreCapstones = new List<int>();

    // Use this for initialization
    private void Start()
    {
        DOTween.SetTweensCapacity(10000, 500);
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

        //value = 46356;

        if (_sequence != null)
        {
            if (value == _target) return;

            if (isActiveAndEnabled)
                StartCoroutine(MyCoroutines.DoUntil(() => _sequence == null, null,
                    () => { StartCoroutine(MyCoroutines.WaitOneFrame(() => SetTarget(value))); }));
        }

        _target = value;
        //_target = 46356;
        //_value = 314;
        //int duration = Mathf.Max(_target / _value, 1);
        float totalDuration = _popupBaseDuration;
        float explosionDuration = _endExplosionDuration;
        int fontSize = _scoreObject.fontSize;
        Vector2 rectTransformSizeDelta = _scoreObject.rectTransform.sizeDelta;

        //var sequence = SequenceGradual(duration, size, fontSize);

        if (_sequence != null) return;

        _sequence = SequencePulse(totalDuration, _scalePopUp, rectTransformSizeDelta, fontSize);
        _sequence.Append(SequenceExplosion(explosionDuration, fontSize, _scaleExplosion, rectTransformSizeDelta,
            _particle));

        //Performance stuff
        _sequence.Pause().SetAutoKill(true).SetRecyclable();

        _sequence.Play();

        _sequence.OnComplete(() =>
        {
            if (isActiveAndEnabled)
                StartCoroutine(MyCoroutines.WaitOneFrame(() => _sequence = null));
        });
    }

    private Sequence SequencePulse(float duration, float scale, Vector2 size, int fontSize)
    {
        //Calculate Powers
        int powerTarget = GetStep10(_target);
        int powerValue = Mathf.Max(2, GetStep10(_value));

        duration += duration / 2 * (powerTarget - powerValue);

        List<int> targets = _scoreCapstones.FindAll(i => _value < i && i < _target);

        int totalSteps = targets.Count;

        Sequence sequence = DOTween.Sequence();
        if (totalSteps <= 0) return sequence;

        float stepDuration = duration / totalSteps;

        //Slice up time into neat chunks 
        //Spend 75% of the time incrementing (or tweening)
        //value to step target (which is an user defined capstone
        //power = 10 - maxStepCounter (in the general case)
        //E.g. if value is 314 -> 400 -> 500 -> etc.

        //Spend other 25% creating an explosion (12.5% increasing size,
        //other half going back to original size)
        float durationNumber = stepDuration * 3 / 4f;
        float timeRemainder = stepDuration - durationNumber;
        float half = timeRemainder / 2;

        //Count until we finish all the steps
        for (int i = 0; i < totalSteps; i++)
        {
            //Creating a sequence to do everything in
            //Btw, it's local time inside it
            //Eventually we append it back to keep the local times
            Sequence pulse = DOTween.Sequence()
                    .Append(DOTween.To(() => _value, x => _value = x, targets[i],
                        durationNumber))
                    .AppendInterval(timeRemainder)
                    .Insert(durationNumber, _scoreObject.rectTransform.DOSizeDelta(size * scale, half, true))
                    .Insert(durationNumber, DOTween.To(() => _scoreObject.fontSize,
                        x => _scoreObject.fontSize = (int)x, fontSize * scale, half))
                    .Insert(durationNumber + half, _scoreObject.rectTransform.DOSizeDelta(size, half, true))
                    .Insert(durationNumber + half,
                        DOTween.To(() => _scoreObject.fontSize, x => _scoreObject.fontSize = x, fontSize, half))
                ;

            sequence.Append(pulse);
        }

        float interval = stepDuration / 2;
        sequence.AppendInterval(interval);

        return sequence;
    }

    private Sequence SequenceExplosion(float duration, float fontSize, float scale, Vector2 size, UIParticleSystem particle)
    {
        //Local Sequence (Explode to End Value)
        //float duration = duration;
        float halfDuration = duration / 2;
        Sequence end = DOTween.Sequence()
            .Append(DOTween.To(() => _value, x => _value = x, _target, halfDuration))
            .Insert(halfDuration, _scoreObject.rectTransform.DOSizeDelta(size * scale, halfDuration, true))
            .Insert(halfDuration, DOTween.To(() => _scoreObject.fontSize, x => _scoreObject.fontSize = (int)x,
                fontSize * scale, halfDuration))
            .Insert(duration, _scoreObject.rectTransform.DOSizeDelta(size, halfDuration, true))
            .Insert(duration, DOTween.To(() => _scoreObject.fontSize, x => _scoreObject.fontSize = (int)x, fontSize, halfDuration));
        //sequence.Append(end);

        //Particles
        if (particle != null)
            end.InsertCallback(0, particle.Play);

        return end;
    }

    private int GetStep10(int number)
    {
        int count = 0;
        while (number > 9)
        {
            number /= 10;
            count++;
        }
        return count;
    }
}
