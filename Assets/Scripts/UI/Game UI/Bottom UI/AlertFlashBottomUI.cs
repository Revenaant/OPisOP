using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AlertFlashBottomUI : MonoBehaviour
{
    [System.Serializable]
    private class FlashyUI
    {
        private float _alpha = 1;

        public float Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                UpdateAlpha();
            }
        }

        [HideInInspector] public bool flashing = false;
        [HideInInspector] public List<Graphic> list = new List<Graphic>();
        public Graphic graphic = null;
        public bool includeChildGraphics = true;

        public FlashyUI()
        {
            flashing = false;
            list = new List<Graphic>();
        }

        public void UpdateAlpha()
        {
            foreach (Graphic g in list)
            {
                g.CrossFadeAlpha(Alpha, 0.001f, false);
            }
        }

        public override string ToString()
        {
            return GetType().ToString();
        }
    }

    //[SerializeField, ReadOnly]
    private bool _flashing = false;
    //[SerializeField, ReadOnly]
    private int _conditionsHappening = 0;
    //[SerializeField, ReadOnly]
    private float _currentAlpha = 1;
    //[SerializeField, ReadOnly]
    private float _targetAlpha = 1;
    //[SerializeField, ReadOnly]
    private List<FlashyUI> _flashingUIs = new List<FlashyUI>();
    private Tweener _alphaTweener = null;

    private int ConditionsHappening
    {
        get { return _conditionsHappening; }
        set
        {
            _conditionsHappening = value;
            //Start or End _currentAlpha Lerp
            if (value > 0 && !_flashing)
            {
                _flashing = true;
                StartFlashingTween();
                return;
            }

            if (value == 0 && _flashing)
            {
                _flashing = false;
                StopFlashingTween();
            }
        }
    }

    [SerializeField] private NotificationsConditionManager _conditionManager = null;

    [SerializeField] private float _duration = 2;
    [SerializeField] private MinMaxPair _alphaRange = new MinMaxPair(0, 1);

    [SerializeField] private FlashyUI _hunger = new FlashyUI();
    [SerializeField] private FlashyUI _battery = new FlashyUI();
    [SerializeField] private FlashyUI _temperature = new FlashyUI();

    //private List<Graphic> _listHungerGraphics = new List<Graphic>();
    //private List<Graphic> _listBatterGraphics = new List<Graphic>();
    //private List<Graphic> _listTemperatureGraphics = new List<Graphic>();
    //[SerializeField] private Graphic _hungerGraphic = null;
    //[SerializeField] private Graphic _batteryGraphic = null;
    //[SerializeField] private Graphic _temperatureGraphic = null;

    private void OnEnable()
    {
        if (_conditionManager == null) _conditionManager = FindObjectOfType<NotificationsConditionManager>();
        if (_conditionManager == null) return;

        _conditionManager.BatteryOverchargeNotification.onTrigger += StartFlashingBattery;
        _conditionManager.GasBatteryUnderchargeNotification.onTrigger += StartFlashingBattery;
        _conditionManager.HungerNotification.onTrigger += StartFlashingHunger;
        _conditionManager.TemperatureTooHotNotification.onTrigger += StartFlashingTemperature;
        _conditionManager.TemperatureTooColdNotification.onTrigger += StartFlashingTemperature;

        _conditionManager.BatteryOverchargeNotification.onEnd += EndFlashingBattery;
        _conditionManager.GasBatteryUnderchargeNotification.onEnd += EndFlashingBattery;
        _conditionManager.HungerNotification.onEnd += EndFlashingHunger;
        _conditionManager.TemperatureTooHotNotification.onEnd += EndFlashingTemperature;
        _conditionManager.TemperatureTooColdNotification.onEnd += EndFlashingTemperature;
    }

    // Use this for initialization
    private void Start()
    {
        Init();
        _targetAlpha = _alphaRange.Max;
    }

    private void OnDisable()
    {
        if (_conditionManager == null) return;

        if (_conditionManager.BatteryOverchargeNotification.onTrigger != null)
            _conditionManager.BatteryOverchargeNotification.onTrigger -= StartFlashingBattery;
        if (_conditionManager.GasBatteryUnderchargeNotification.onTrigger != null)
            _conditionManager.GasBatteryUnderchargeNotification.onTrigger -= StartFlashingBattery;
        if (_conditionManager.HungerNotification.onTrigger != null)
            _conditionManager.HungerNotification.onTrigger -= StartFlashingHunger;
        if (_conditionManager.TemperatureTooHotNotification.onTrigger != null)
            _conditionManager.TemperatureTooHotNotification.onTrigger -= StartFlashingTemperature;
        if (_conditionManager.TemperatureTooColdNotification.onTrigger != null)
            _conditionManager.TemperatureTooColdNotification.onTrigger -= StartFlashingTemperature;

        if (_conditionManager.BatteryOverchargeNotification.onEnd != null)
            _conditionManager.BatteryOverchargeNotification.onEnd -= EndFlashingBattery;
        if (_conditionManager.GasBatteryUnderchargeNotification.onEnd != null)
            _conditionManager.GasBatteryUnderchargeNotification.onEnd -= EndFlashingBattery;
        if (_conditionManager.HungerNotification.onEnd != null)
            _conditionManager.HungerNotification.onEnd -= EndFlashingHunger;
        if (_conditionManager.TemperatureTooHotNotification.onEnd != null)
            _conditionManager.TemperatureTooHotNotification.onEnd -= EndFlashingTemperature;
        if (_conditionManager.TemperatureTooColdNotification.onEnd != null)
            _conditionManager.TemperatureTooColdNotification.onEnd -= EndFlashingTemperature;
    }

    private void Init()
    {
        //Debug.Assert(_hunger.graphic != null, "Assign Base Hunger Graphic.");
        //Debug.Assert(_battery.graphic != null, "Assign Base Battery Graphic.");
        //Debug.Assert(_temperature.graphic != null, "Assign Base Temperature Graphic.");

        _battery.list = _battery.includeChildGraphics ?
            _battery.graphic.GetComponentsInChildren<Graphic>().ToList()
            : new List<Graphic> { _battery.graphic };

        _hunger.list = _hunger.includeChildGraphics ?
            _hunger.graphic.GetComponentsInChildren<Graphic>().ToList()
            : new List<Graphic> { _hunger.graphic };

        _temperature.list = _temperature.includeChildGraphics ?
            _temperature.graphic.GetComponentsInChildren<Graphic>().ToList()
            : new List<Graphic> { _temperature.graphic };
    }

    private void StartFlashingTween()
    {
        SwapTarget();
        if (_alphaTweener == null)
        {
            _alphaTweener = DOTween.To(() => _currentAlpha, alpha => _currentAlpha = alpha, _targetAlpha, _duration)
                .SetAutoKill(false).OnComplete(OnCompleteTweener);

            return;
        }

        _alphaTweener.ChangeEndValue(_targetAlpha, _duration, true).OnComplete(OnCompleteTweener).Restart();
        //_alphaTweener.Play();
    }

    private void StopFlashingTween()
    {
        ////Debug.Log("Stop Flashing Tween");
        if (_alphaTweener == null)
        {
            _flashingUIs.Clear();
            return;
        }
        //Debug.Log("Stop Flashing Tween Tweener not null.");

        _alphaTweener.ChangeEndValue(_alphaRange.Max, Mathf.Abs(_targetAlpha - _currentAlpha) * _duration, true).OnComplete(() =>
        {
            //Debug.Log("End Flashing.");
            _flashingUIs.Clear();
            OnCompleteTweener();
        }).Restart();
        //_alphaTweener.Play();
    }

    private void OnCompleteTweener()
    {
        if (_alphaTweener == null) return;

        ////Debug.Log("Complete One Tween");

        if (_flashing)
        {
            ////Debug.Log("Swap Flashing");
            SwapTarget();
            _alphaTweener.ChangeEndValue(_targetAlpha, _duration, true).Restart();
        }
        else
        {
            //Debug.Log("Kill");
            _alphaTweener.Kill();
            _alphaTweener = null;
        }
    }

    private void SwapTarget()
    {
        if (Math.Abs(_targetAlpha - _alphaRange.Max) < Mathf.Epsilon)
        {
            _targetAlpha = _alphaRange.Min;
            return;
        }

        if (Math.Abs(_targetAlpha - _alphaRange.Min) < Mathf.Epsilon)
        {
            _targetAlpha = _alphaRange.Max;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateAlphaFlashing();
    }

    private void UpdateAlphaFlashing()
    {
        foreach (FlashyUI flash in _flashingUIs)
            flash.Alpha = _currentAlpha;
    }

    public void StartFlashingBattery()
    {
        StartFlashing(_battery);
    }

    public void StartFlashingHunger()
    {
        StartFlashing(_hunger);
    }

    public void StartFlashingTemperature()
    {
        StartFlashing(_temperature);
    }

    public void EndFlashingBattery()
    {
        EndFlashing(_battery);
    }

    public void EndFlashingHunger()
    {
        EndFlashing(_hunger);
    }

    public void EndFlashingTemperature()
    {
        EndFlashing(_temperature);
    }

    private void StartFlashing(FlashyUI flashyUI)
    {
        ////Debug.Log("Attempt Start Flashing");

        if (flashyUI.flashing) return;

        //Debug.Log("Start Not Flashing" + flashyUI);

        ConditionsHappening++;
        //If this is the first one
        if (ConditionsHappening == 1)
        {
            //Debug.Log("First");
            flashyUI.flashing = true;
            _flashingUIs.Add(flashyUI);
            return;
        }

        //Debug.Log("NExt");
        //Catch up
        //FlashyUI copy = flashyUI;
        DOTween.To(() => flashyUI.Alpha, alpha => flashyUI.Alpha = alpha, _targetAlpha,
            Mathf.Abs(_targetAlpha - _currentAlpha) * _duration).SetAutoKill(true).SetRecyclable()
            .OnComplete(() => { _flashingUIs.Add(flashyUI); });

        flashyUI.flashing = true;
    }

    private void EndFlashing(FlashyUI flashyUI)
    {

        //Debug.Log("Attempt End");
        ConditionsHappening--;
        //If this is the last one
        if (ConditionsHappening == 0)
            return;

        //Debug.Log("End is not last");
        DOTween.To(() => flashyUI.Alpha, alpha => flashyUI.Alpha = alpha, _alphaRange.Max,
                Mathf.Abs(_alphaRange.Max - _currentAlpha) * _duration).SetAutoKill(true).SetRecyclable()
            .OnComplete(() => { _flashingUIs.Add(flashyUI); });

        flashyUI.flashing = false;
        _flashingUIs.Remove(flashyUI);
    }
}
