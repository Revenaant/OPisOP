using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureBarUI : MonoBehaviour
{
    private float _value = 1;
    private float _target = 1;
    private float _total = 0;

    [SerializeField]
    private RectTransform _gauge = null;

    [SerializeField] private float _speed = 0.01f;

    [Header("Local")]
    [SerializeField]
    private float _minPosX = -21.5f;
    [SerializeField]
    private float _maxPosX = 21.5f;

    // Use this for initialization
    private void Start()
    {
        _total = Mathf.Abs(_maxPosX) + Mathf.Abs(_minPosX);
    }

    private void OnEnable()
    {
        GameManager.onTemperatureChange += SetTarget100;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (GameManager.onTemperatureChange != null)
            GameManager.onTemperatureChange -= SetTarget100;
    }

    // Update is called once per frame
    private void UpdateGauge(float value)
    {
        Vector3 localPos = _gauge.localPosition;
        localPos.x = value * _total - Mathf.Abs(_minPosX);
        _gauge.localPosition = localPos;
    }

    public void SetTarget100(int value)
    {
        SetTarget(value / 100.0f);
    }

    public void SetTarget(float value)
    {
        StopAllCoroutines();

        _target = value;
        StartCoroutine(LerpBar());
    }

    private IEnumerator LerpBar()
    {
        while (Mathf.Abs(_target - _value) > Mathf.Epsilon)
        {
            //_value = Mathf.Lerp()
            _value = Mathf.MoveTowards(_value, _target, _speed);
            UpdateGauge(_value);
            yield return null;
        }

        yield return null;
    }
}
