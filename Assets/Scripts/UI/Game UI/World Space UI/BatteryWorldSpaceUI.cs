using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BatteryWorldSpaceUI : MonoBehaviour
{
    //private enum S { A, B, C}

    //private S s = S.C;

    private bool _moving = false;

    private int _energy = 1;
    private int _energyValue = 1;
    private int _capacityValue = 1;
    private int _capacity = 1;
    //private float _width = 0;

    private float _height = 0;

    private Tweener _capacityTweener = null;
    private Tweener _energyTweener = null;
    private Tweener _jiggleTweener = null;
    //private Tweener _widthTweener = null;

    //private Quaternion _target = Quaternion.identity;
    private Quaternion _startRotation = Quaternion.identity;
    private Quaternion _leftRotation = Quaternion.identity;
    private Quaternion _rightRotation = Quaternion.identity;

    //private Color _batteryColor = Color.black;

    [SerializeField] private Gradient _gradient = new Gradient();
//#pragma warning disable 0414
//    [SerializeField] private Color _overchargeColor = Color.red;
//#pragma warning restore 0414
    [SerializeField, Range(0, 1)] private float _overchargeThreshold = 0.9f;
    [SerializeField] private float _maxWidth = 50;
    [SerializeField] private float _jiggleAngle = 30;

    [Header("References")]
    [SerializeField] private Image _batteryIcon = null;
    [SerializeField] private Image _bar = null;
    [SerializeField] private Text _text = null;

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(_bar != null, "Assign Bar.");
        //Debug.Assert(_batteryIcon != null, "Assign Battery Icon.");
        Debug.Assert(_text != null, "Assign Text.");
        //_width = _bar.rectTransform.sizeDelta.x;
        _height = _bar.rectTransform.sizeDelta.y;
        //_batteryColor = _bar.color;

        if (_batteryIcon == null) return;
        _startRotation = _batteryIcon.transform.localRotation;
        _leftRotation = _startRotation * Quaternion.AngleAxis(-_jiggleAngle, transform.forward);
        _rightRotation = _startRotation * Quaternion.AngleAxis(_jiggleAngle, transform.forward);
    }

    private void OnEnable()
    {
        Storage.onEnergyChange += SetEnergy;
        Storage.onCapacityChange += SetCapacity;
    }

    private void OnDisable()
    {
        if (Storage.onEnergyChange != null)
            Storage.onEnergyChange -= SetEnergy;
        if (Storage.onCapacityChange != null)
            Storage.onCapacityChange -= SetCapacity;
    }

    private void LateUpdate()
    {
        UpdateText();
        UpdateBar();
        UpdateBatteryIcon();
    }

    private void SetEnergy(int energy)
    {
        _energy = energy;
        UpdateEnergy();
    }

    private void SetCapacity(int capacity)
    {
        _capacity = capacity;
        UpdateCapacity();
    }

    private void UpdateCapacity()
    {
        if (_capacityTweener != null)
        {
            _capacityTweener.Kill();
            _capacityTweener = null;
        }

        _capacityTweener =
            DOTween.To(() => _capacityValue, value => _capacityValue = value, _capacity, 0.5f)
                .SetAutoKill(true).SetRecyclable().OnComplete(() => _capacityTweener = null);
    }

    private void UpdateEnergy()
    {
        if (_energyTweener != null)
        {
            _energyTweener.Kill();
            _energyTweener = null;
        }

        _energyTweener =
            DOTween.To(() => _energyValue, value => _energyValue = value, _energy, 0.5f)
                .SetAutoKill(true).SetRecyclable().OnComplete(() => _energyTweener = null);
    }

    private void UpdateBar()
    {
        if (_bar == null) return;

        float value = _energyValue / (float)_capacityValue;

        //Old Color
        //if (value >= _overchargeThreshold)
        //{
        //    float weight = (1 - value) / (1 - _overchargeThreshold);
        //    Color finalColor = _batteryColor * weight + _overchargeColor * (1 - weight);
        //    _bar.color = finalColor;
        //}
        //else _bar.color = _batteryColor;

        _bar.color = _gradient.Evaluate(value);

        _bar.rectTransform.sizeDelta = new Vector2(value * _maxWidth, _height);
    }

    private void UpdateBatteryIcon()
    {
        if (_batteryIcon == null) return;

        float value = _energyValue / (float)_capacityValue;

        _batteryIcon.color = _gradient.Evaluate(value);

        if (value >= _overchargeThreshold)
        {
            Jiggle();
        }
        else if (_moving)
        {
            //Debug.Log("BITCONNECT");
            StopJiggle();
        }
    }

    private void Jiggle(float duration = 0.1f)
    {
        if (!_moving && _jiggleTweener != null)
        {
            _jiggleTweener.Kill(true);
        }

        if (_jiggleTweener == null)
        {
            _jiggleTweener = _batteryIcon.transform.DOLocalRotateQuaternion(_leftRotation, duration).SetAutoKill(false);
        }

        if (Quaternion.Angle(_batteryIcon.transform.localRotation, _leftRotation) <= Mathf.Epsilon)
            _jiggleTweener.ChangeEndValue(_rightRotation, true).Restart();
        else if (Quaternion.Angle(_batteryIcon.transform.localRotation, _rightRotation) <= Mathf.Epsilon)
            _jiggleTweener.ChangeEndValue(_leftRotation, true).Restart();

        _moving = true;
    }

    private void StopJiggle()
    {
        _moving = false;
        //_jiggleTweener.Kill(true);
        _jiggleTweener.ChangeEndValue(_startRotation, true)
        //_jiggleTweener = _bar.transform.DOLocalRotateQuaternion(_startRotation, 0.5f)
            .OnComplete(() =>
            {
                //Debug.Log("WASUWASUWASU");
                _jiggleTweener = null;
            });
        _jiggleTweener.Restart();
    }

    private void UpdateText()
    {
        if (_text == null) return;
        int final = (int)(_energyValue / (float)_capacityValue * 100);
        _text.text = final + "%";
    }
}
