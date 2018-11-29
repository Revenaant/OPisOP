using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryBarUI : MonoBehaviour
{
    private float _energyValue = 0;
    private float _energyTarget = 1;

    private float _capValue = 0;
    private float _capTarget = 1;
    private float _capTotal = 0;

    private Coroutine _fillbarCoroutine = null;
    private Coroutine _capCoroutine = null;

    [Header("Fill Bar")]
    [SerializeField] private Image _fillBar = null;
    [SerializeField] private float _imageWidthFullPoint = 51.5f;
    [SerializeField] private float _speed = 0.01f;

    [Header("Cap")]
    [SerializeField] private RectTransform _capacityCap = null;

    [SerializeField] private float _minX = -10.2f;
    [SerializeField] private float _maxX = 39;

    [Header("Other")]
    [SerializeField] private int _maxCapacity = 20;
    [SerializeField, ReadOnly] private int _capacity = 20;
    [SerializeField, ReadOnly] private int _energy = 20;

    // Use this for initialization
    private void Start()
    {
        _capTotal = Mathf.Abs(_minX) + Mathf.Abs(_maxX);
    }

    private void OnEnable()
    {
        Storage.onMaxCapacityChange += SetMaxCapacity;
        Storage.onCapacityChange += CalculateTargetCap;
        Storage.onEnergyChange += CalculateTargetFill;
    }

    private void OnDisable()
    {
        if (Storage.onMaxCapacityChange != null) Storage.onMaxCapacityChange -= SetMaxCapacity;
        if (Storage.onCapacityChange != null) Storage.onCapacityChange -= CalculateTargetCap;
        if (Storage.onEnergyChange != null) Storage.onEnergyChange -= CalculateTargetFill;
    }

    private void SetMaxCapacity(int maxCapacity)
    {
        _maxCapacity = maxCapacity;
        _energy = Mathf.Clamp(_energy, 0, _maxCapacity);
        _capacity = Mathf.Clamp(_capacity, 0, _maxCapacity);
    }

    private void CalculateTargetCap(int capacity)
    {
        _capacity = capacity;
        SetTargetCap(_capacity / (float)_maxCapacity);
    }

    private void CalculateTargetFill(int energy)
    {
        _energy = energy;
        SetTargetFill(energy / (float)_maxCapacity);
    }

    // Update is called once per frame
    private void UpdateFillBar(float value)
    {
        _fillBar.rectTransform.sizeDelta = new Vector2(_imageWidthFullPoint * value, _fillBar.rectTransform.sizeDelta.y);
    }

    public void SetTargetFill(float value)
    {
        if (_fillbarCoroutine != null)
        {
            StopCoroutine(_fillbarCoroutine);
            _fillbarCoroutine = null;
        }

        _energyTarget = value;
        _fillbarCoroutine = StartCoroutine(LerpFillBar());
    }

    private IEnumerator LerpFillBar()
    {
        while (Mathf.Abs(_energyTarget - _energyValue) > Mathf.Epsilon)
        {
            //_energyValue = Mathf.Lerp()
            _energyValue = Mathf.MoveTowards(_energyValue, _energyTarget, _speed);
            UpdateFillBar(_energyValue);
            yield return null;
        }

        yield return null;
    }

    // Update is called once per frame
    private void UpdateCap(float value)
    {
        //Debug.Log("!");
        Vector3 localPos = _capacityCap.localPosition;
        localPos.y = value * _capTotal - Mathf.Abs(_minX);
        _capacityCap.localPosition = localPos;
    }

    public void SetTargetCap(float value)
    {
        //Debug.Log("Hello ");
        if (_capCoroutine != null)
        {
            StopCoroutine(_capCoroutine);
            _capCoroutine = null;
        }

        _capTarget = value;
        _capCoroutine = StartCoroutine(LerpCap());
    }

    private IEnumerator LerpCap()
    {
        //Debug.Log("World");
        while (Mathf.Abs(_capTarget - _capValue) > Mathf.Epsilon)
        {
            //_value = Mathf.Lerp()
            _capValue = Mathf.MoveTowards(_capValue, _capTarget, _speed);
            UpdateCap(_capValue);
            yield return null;
        }

        yield return null;
    }
}
