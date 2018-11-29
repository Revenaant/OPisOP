using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBarUI : MonoBehaviour
{
    private float _value = 1;
    private float _target = 1;

    [SerializeField] private float _speed = 0.01f;

    [SerializeField] private Image _fillBar = null;
    [SerializeField] private float _imageWidthFullPoint = 51.5f;

    // Use this for initialization
    private void OnEnable()
    {
        Pet.onHungerChanged += SetTarget1Minus100;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (Pet.onHungerChanged != null)
            Pet.onHungerChanged -= SetTarget1Minus100;
    }

    // Update is called once per frame
    private void UpdateFillBar(float value)
    {
        _fillBar.rectTransform.sizeDelta = new Vector2(_imageWidthFullPoint * value, _fillBar.rectTransform.sizeDelta.y);
    }

    public void SetTarget1Minus100(float value)
    {
        SetTarget(1 - value / 100);
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
            UpdateFillBar(_value);
            yield return null;
        }

        yield return null;
    }
}
