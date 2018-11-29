using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI2 : MonoBehaviour
{
    private bool _waiting = false;

    private int _value = 1;
    private int _target = 1;

    private Sequence _sequence = null;

    //[SerializeField]
    //private int _speed = 1;
    [Header("Interpolates score value to pre-calculated capstone(s), then pulses (size increases")]
    [Header("and decreases quickly), " + "the finally explodes (pulses at higher size and spawns particles")]
    [SerializeField]
    private UIParticleSystem _particle = null;
    [SerializeField]
    private Text _scoreObject = null;

    [SerializeField] private float _scalePopUp = 1.5f;
    [SerializeField] private float _scaleExplosion = 2;

    [SerializeField] private float _popupBaseDuration = 5;
    [SerializeField] private float _endExplosionDuration = 1;

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

        float totalDuration = _popupBaseDuration;
        float explosionDuration = _endExplosionDuration;
        int fontSize = _scoreObject.fontSize;
        Vector2 rectTransformSizeDelta = _scoreObject.rectTransform.sizeDelta;


        _sequence = SequencePulse(totalDuration, _scalePopUp, rectTransformSizeDelta, fontSize);
        _sequence.Append(SequenceExplosion(explosionDuration, fontSize, _scaleExplosion, rectTransformSizeDelta,
            _particle));

        //Performance stuff
        _sequence.Pause().SetAutoKill(true).SetRecyclable();

        _sequence.Play();

        _sequence.OnComplete(() => StartCoroutine(MyCoroutines.WaitOneFrame(() => _sequence = null)));
    }

    private Sequence SequencePulse(float duration, float scale, Vector2 size, int fontSize, bool debug = false)
    {
        //Calculate Powers
        int powerTarget = GetStep10(_target);
        int powerValue = Mathf.Max(2, GetStep10(_value));

        duration += duration / 2 * (powerTarget - powerValue);

        //Calculate the value's 10 to its power and find out at what part of it is
        //E.g. value = 314 -> increment = 100 (10^2) ; step power = 3

        //Start
        int tenIncrementValue = (int)Mathf.Pow(10, powerValue);
        int currentStepPowerValue = _value / tenIncrementValue;

        //End
        int tenIncrementTarget = (int)Mathf.Pow(10, powerTarget);
        int currentStepPowerTarget = _target / tenIncrementTarget;

        //Do previous steps in order to calculate total amount of steps
        //to calculate how long each small step is
        int totalSteps = 0;

        if (powerValue != powerTarget)
        {
            //How many steps to reach 10 from the start
            //E.g. if 3 -> +7 steps
            totalSteps += 10 - currentStepPowerValue;

            //In between add 10 until we reach the target's threshold
            for (int i = powerValue + 1; i < powerTarget; i++)
                totalSteps += 10;

            //How many steps to reach from x * 10 until the end
            //E.g. if 6 -> +6 steps, cause e.g. 46 -> 40 + 6
            //and we calculate how to get to 10 and then add 10s until we reach
            //this threshold of 40
            totalSteps += currentStepPowerTarget;
        }
        //Special case where the 2 powers are equal
        //E.g. value (314) and taget (989) -> 9 - 3 = 6 steps
        else
            totalSteps = currentStepPowerTarget - currentStepPowerValue;

        float stepDuration = duration / totalSteps;

        Sequence sequence = DOTween.Sequence();

        int counter = 0;
        int increment = tenIncrementValue;


        //Slice up time into neat chunks 
        //Spend 75% of the time incrementing (or tweening)
        //value to step target (which is an increment of 10 ^ power
        //power = 10 - maxStepCounter (in the general case)
        //E.g. if value is 314 -> 400 -> 500 -> etc.

        //Spend other 25% creating an explosion (12.5% increasing size,
        //other half going back to original size)
        float durationNumber = stepDuration * 3 / 4f;
        float timeRemainder = stepDuration - durationNumber;
        float half = timeRemainder / 2;

        //Count until we finish all the steps
        while (counter < totalSteps)
        {
            //Step counter per power
            int maxPowerStepCounter = 10;
            if (totalSteps > 10)
            {
                //Increment increment (ironic)
                if (counter > currentStepPowerValue) increment *= 10;
                if (counter < currentStepPowerValue) maxPowerStepCounter = 10 - currentStepPowerValue;
                if (counter >= totalSteps - currentStepPowerTarget) maxPowerStepCounter = currentStepPowerTarget;

            }
            //Special case previous mentioned
            else maxPowerStepCounter = totalSteps;

            if (debug)
            {
                Debug.Log("Counter: " + counter);
                Debug.Log("Max Power Step Counter: " + maxPowerStepCounter);
            }

            //Count until we finish all the steps for this power of 10
            while (maxPowerStepCounter > 0)
            {
                //Increment counters
                maxPowerStepCounter--;
                counter++;

                int amount = 10 - maxPowerStepCounter/* + 1*/;
                if (counter >= totalSteps - currentStepPowerTarget) amount = currentStepPowerTarget - maxPowerStepCounter/* + 1*/;
                //Creating a sequence to do everything in
                //Btw, it's local time inside it
                //Eventually we append it back to keep the local times
                var increment1 = increment;
                Sequence pulse = DOTween.Sequence()
                        .AppendCallback(() =>
                        {
                            if (debug)
                            {
                                Debug.Log("Current Value: " + _value);
                                Debug.Log("New Target: " + amount * increment1);
                            }

                            int checkValue = amount * increment1 * 9 / 10;
                            if (_value < checkValue) _value = checkValue;
                            if (debug)
                            {
                                Debug.Log("New Value: " + _value);
                            }
                        })

                        .Append(DOTween.To(() => _value, x => _value = x, amount * increment,
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
        }

        float interval = Mathf.Abs(1 - (_target - currentStepPowerTarget * tenIncrementTarget) / (float)tenIncrementTarget);
        sequence.AppendInterval(interval);

        //Debug stuff

        if (debug)
        {
            Debug.Log("Target: " + _target);
            Debug.Log("Value: " + _value);
            Debug.Log("Power Target: " + powerTarget);
            Debug.Log("Power Value: " + powerValue);
            Debug.Log("Ten Increment Target: " + tenIncrementTarget);
            Debug.Log("Ten Increment Value: " + tenIncrementValue);
            Debug.Log("Current Step Power Target: " + currentStepPowerTarget);
            Debug.Log("Current Step Power Value: " + currentStepPowerValue);
            Debug.Log("Total Steps: " + totalSteps);
            Debug.Log("Last Value: " + (currentStepPowerTarget * tenIncrementTarget));
            Debug.Log("Interval: " + interval);
            Debug.Log("Duration: " + duration);
        }

        return sequence;
    }

    private Sequence SequenceExplosion(float duration, float fontSize, float scale, Vector2 size, UIParticleSystem particle)
    {
        //Local Sequence (Explode to End Value)
        //float duration = duration;
        float halfDuration = duration / 2;
        Sequence end = DOTween.Sequence()
            .Append(DOTween.To(() => _value, x => _value = x, _target, halfDuration))
            .Insert(0, _scoreObject.rectTransform.DOSizeDelta(size * scale, halfDuration, true))
            .Insert(0, DOTween.To(() => _scoreObject.fontSize, x => _scoreObject.fontSize = (int)x,
                fontSize * scale, halfDuration))
            .Insert(halfDuration, _scoreObject.rectTransform.DOSizeDelta(size, halfDuration, true))
            .Insert(halfDuration, DOTween.To(() => _scoreObject.fontSize, x => _scoreObject.fontSize = (int)x, fontSize, halfDuration));
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
