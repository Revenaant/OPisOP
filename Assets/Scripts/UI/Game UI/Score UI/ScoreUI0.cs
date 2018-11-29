using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI0 : MonoBehaviour
{
    private bool _waiting = false;

    private int _value = 1;
    private int _target = 1;

    //[SerializeField]
    private int _speed = 1;

    [Header("Interpolates score value to new value.")]
    
    [SerializeField]
    private Text _scoreObject = null;
    
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
        StopAllCoroutines();

        _target = value;
        _speed = Mathf.Max((_target - _value) / 100, 1);

        StartCoroutine(LerpValue());
    }

    private IEnumerator LerpValue()
    {
        while (_target - _value > 0)
        {
            //_value = Mathf.Lerp()
            _value = MoveTowards(_value, _target, _speed);
            UpdateText(_value);
            yield return null;
        }

        yield return null;
    }

    private int MoveTowards(int value, int target, int delta)
    {
        if (target - value < delta) delta = target - value;
        value += delta;
        return value;
    }
}
