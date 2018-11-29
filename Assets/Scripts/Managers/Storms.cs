using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Storms : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private float _timeSinceLastWind = 0;
    private float _timeSinceLastStorm = 0;
    //private float _randomStormInterval = 0;
    private SolarPanel _solarPanel = null;

    [SerializeField] private bool _paused = false;
    [SerializeField] private bool _isStorm = false;
    [SerializeField] private bool _automaticStorms = true;

    [Header("Time (seconds)")]

    [SerializeField] private float _windInterval = 20;
    [SerializeField] private float _windIntervalAmplitude = 2;
    [SerializeField] private float _stormDuration = 10;
    [SerializeField] private float _stormTickRate = 1;

    [Header("Time (hours)")]

    [SerializeField, Range(0, 23)] private int _stormStart = 14;

    [Header("Solar Panel")]

    [SerializeField, Range(0, 1)] private float _dirtPerWind = 0.1f;

    [SerializeField, Range(0, 1)] private float _dirtPerStorm = 0.6f;
    [SerializeField, ReadOnly] private float _dirtPerStormSecond = 0;

    [Header("Output")]
    [ReadOnly, SerializeField] private float _randomWindInterval = 0;

    public UnityEvent onWind;
    public UnityEvent onStormTick;
    public UnityEvent onStormStart;
    public UnityEvent onStormEnd;

    private void OnValidate()
    {
        _timeSinceLastWind = Time.time;
        _randomWindInterval = 0;

        if (_windInterval < 0) _windInterval = 0;
        if (_windIntervalAmplitude < 0) _windIntervalAmplitude = 0;

        if (_stormDuration < Mathf.Epsilon) _stormDuration = Mathf.Epsilon;

        _dirtPerStormSecond = _dirtPerStorm / _stormDuration;
    }

    // Use this for initialization
    private void Start()
    {
        GameManager.Instance.Storms = this;

        _solarPanel = FindObjectOfType<SolarPanel>();
        Debug.Assert(_solarPanel != null);

        DayNightCycle dnc = GameManager.Instance.DayNightCycle;
        if (dnc == null) dnc = FindObjectOfType<DayNightCycle>();
        dnc.AddEvent(_stormStart, StartStormAutomatic);
        //dnc.onAfternoon.AddListener(StartStorm);

        onStormEnd.AddListener(ResetWindTime);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_paused) return;

        if (_isStorm)
        {
            if (Time.time > _timeSinceLastStorm + _stormTickRate)
            {
                //ResetWindTime();
                _timeSinceLastStorm = Time.time;

                BlowStorm();
                if (onStormTick != null) onStormTick.Invoke();
            }
        }
        else
        {
            if (Mathf.Abs(_randomWindInterval) < Mathf.Epsilon)
                _randomWindInterval =
                    _windInterval + Random.Range(-_windIntervalAmplitude, _windIntervalAmplitude);

            if (Time.time > _timeSinceLastWind + _randomWindInterval)
            {
                StartWind();
            }
        }

    }

    #region Wind
    private void ResetWindTime()
    {
        _timeSinceLastWind = Time.time;
        _randomWindInterval = 0;
    }

    public void StartWind()
    {
        ResetWindTime();
        BlowWind();
        //Debug.Log("YOOOOO");
        onWind.Invoke();
    }

    public void BlowWind()
    {
        _solarPanel.Dirt += _dirtPerWind;
    }
    #endregion

    #region Storm

    public void BlowStorm()
    {
        _solarPanel.Dirt += _dirtPerStormSecond;
    }

    public bool IsStorming
    {
        get { return _isStorm; }
        set
        {
            if (_isStorm != value)
            {
                if (value)
                    StartStorm();
                else StopStorm();
            }

            //_isStorm = value;
        }
    }

    public float StormDuration
    {
        get { return _stormDuration; }
        set { _stormDuration = value; }
    }

    public bool AutomaticStorms
    {
        get { return _automaticStorms; }
        set { _automaticStorms = value; }
    }

    private void StartStormAutomatic()
    {
        if (!_automaticStorms) return;
        if (_isStorm) return;

        _isStorm = true;
        onStormStart.Invoke();

        StartCoroutine(StormDurationCouroutine());
    }

    public void StartStorm()
    {
        if (_isStorm) return;

        _isStorm = true;
        onStormStart.Invoke();

        StartCoroutine(StormDurationCouroutine());
    }

    private IEnumerator StormDurationCouroutine()
    {
        yield return new WaitForSeconds(_stormDuration + 0.1f);
        StopStorm();
    }

    public void StopStorm()
    {
        _isStorm = false;
        onStormEnd.Invoke();
    }

    #endregion

    #region Pause
    public void TogglePause()
    {
        _paused = !_paused;
    }

    public bool IsPaused
    {
        get { return _paused; }
        set { _paused = value; }
    }
    #endregion
}