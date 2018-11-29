using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct DayEvent : IComparable, IComparable<DayEvent>, IEquatable<DayEvent>
{
    public int time;
    public Action action;

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        if (obj is DayEvent) return CompareTo((DayEvent)obj);

        return -1;
    }

    public int CompareTo(DayEvent other)
    {
        return time.CompareTo(other.time);
    }

    public bool Equals(DayEvent other)
    {
        return time == other.time;
    }
}

public class DayNightCycle : MonoBehaviour, IPausable, IHaveEvents
{
    private class Hour
    {
        public int hour = 0;
        public bool done = false;
        public List<DayEvent> events = new List<DayEvent>();

        public Hour(int hour)
        {
            this.hour = hour;
        }
    }

    private float _angle = 0;
    [SerializeField, HideInInspector]
    private DayNightAutoTuning _dnat = null;

    [SerializeField] private bool _paused = false;

    //Visual
    //Between 0 and 1 - 0 is dawn and 0 = 1
    [Header("Time (programmer)")]

    [SerializeField, Range(0, 1)] private float _timeNormalized = 0;
    [SerializeField, ReadOnly] private float _timeDayNormalized = 0;
    [SerializeField, ReadOnly] private float _timeNightNormalized = 0;
    [ReadOnly, SerializeField] private float _totalCycle = 0;
    [SerializeField, Range(0, 23)] private int _hourMorningOffset = 5;
    [ReadOnly, SerializeField] private int _hour = 0;
    [ReadOnly, SerializeField] private int _minute = 0;

    //Input
    [Header("Time (seconds)")]
    [SerializeField] private float _dayLength = 120;
    [SerializeField] private float _nightLength = 60;

#pragma warning disable 0414
    [ReadOnly, SerializeField] private float _time = 0;
#pragma warning restore 0414

    [Header("Rotation")]
    //[SerializeField] private bool _swapSkyboxSun = true;
    [SerializeField] private Transform _rotationPivot = null;
    //[SerializeField] private Transform moonPivot = null;
    [SerializeField] private Light _lightSource = null;
    //[SerializeField] private Light _moon = null;

    #region Set Time Events
    //[Header("Time Events")]
    [HideInInspector]
    public UnityEvent onDawn;
    [HideInInspector]
    public UnityEvent onNoon;
    [HideInInspector]
    public UnityEvent onAfternoon;
    [HideInInspector]
    public UnityEvent onDusk;
    [HideInInspector]
    public UnityEvent onMidnight;

    private bool _duskInvoked = false;
    private bool _dawnInvoked = false;
    private bool _noonInvoked = false;
    private bool _midnightInvoked = false;
    private bool _afternoonInvoked = false;

    private float _dawnTime = 0;
    private float _duskTime = 0;
    private float _noonTime = 0;
    private float _afternoonTime = 0;
    private float _midnightTime = 0;
    #endregion

    public Transform RotationPivot
    {
        get { return _rotationPivot; }
    }

    public Light LightSource
    {
        get { return _lightSource; }
    }

    //public Light MoonLight
    //{
    //    get { return _moon; }
    //}

    private SortedList<int, Hour> _dayEvents = new SortedList<int, Hour>();

    //public static System.Action<float> onTimeUpdate;
    public static Action<int, int> onHourMinuteChange;
    public static Action<int> onTimeLeftChange;
    public static Action<float> onRotationUpdate;

    // Use this for initialization
    private void Start()
    {
        GameManager.Instance.DayNightCycle = this;
        GameManager.Instance.Temperature = 50;
        //UpdateRotation();
    }

    public void ForceEvents()
    {
        if (Application.isPlaying && onHourMinuteChange != null)
            onHourMinuteChange(_hour, _minute);

        if (Application.isPlaying && onTimeLeftChange != null)
            onTimeLeftChange((int)(_totalCycle - _time));

        if (Application.isPlaying && onRotationUpdate != null)
            onRotationUpdate(_angle + (IsDay() ? 0 : 180));
        //onRotationUpdate;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (_paused) return;

        UpdateTime();
        UpdateSetTimeEvents();
        UpdateRotation();

        UpdateHourMinute();
    }

    private void OnValidate()
    {
        Debug.Assert(_lightSource != null/* && _moon != null*/);
        Debug.Assert(_rotationPivot != null/* && moonPivot != null*/);

        //_time = Mathf.Clamp01(_time);
        _totalCycle = _dayLength + _nightLength;
        UpdateHourMinute();

        UpdateSetTimeEvents();
        UpdateRotation();

        if (_dnat == null) _dnat = GetComponent<DayNightAutoTuning>();
        _dnat.OnValidate();
    }

    private void UpdateHourMinute()
    {
        _time = GetTimeInSeconds();

        _timeDayNormalized = IsNight() ? 1 : TimeNormalized() / _dayLength;
        _timeNightNormalized = IsDay() ? 0 : (TimeNormalized() - _dayLength) / _nightLength;

        float hour = _timeNormalized * 24;
        //_hour = Wrap(min: 0, max: 24, value: (int)(hour) + 5);

        _hour = Wrap((int)hour + _hourMorningOffset, 0, 24);
        _minute = Wrap((int)(hour % 1 * 60), 0, 60);

        if (_hour >= 24) _hour = 0;

        if (Application.isPlaying && onHourMinuteChange != null)
            onHourMinuteChange(_hour, _minute);

        if (Application.isPlaying && onTimeLeftChange != null)
            onTimeLeftChange((int)(_totalCycle - _time));
    }

    private static int Wrap(int value, int min, int max)
    {
        int delta = max - min;
        if (value < min) value += delta;
        else if (value > max) value -= delta;
        return value;
    }

    private float GetTimeInSeconds()
    {
        return _totalCycle * TimeNormalized();
    }

    public float TimeNormalized()
    {
        return _timeNormalized;
    }

    private void UpdateRotation()
    {
        //float angle = 0;
        if (IsDay())
        {
            //if (_swapSkyboxSun)
            //    RenderSettings.sun = _sun;

            //_sun.enabled = true;
            //_moon.enabled = false;
            _angle = GetTimeInSeconds() / _dayLength * 180;
            _rotationPivot.localRotation =
                Quaternion.AngleAxis(_angle, Vector3.right);

            if (Application.isPlaying)
                GameManager.Instance.Temperature += Time.deltaTime * 2;                         //temperature for the pet
        }
        else
        {
            //if (_swapSkyboxSun)
            //    RenderSettings.sun = _moon;

            //_sun.enabled = false;
            //_moon.enabled = true;
            //_angle = 180 + (GetTimeInSeconds() - _dayLength) / _nightLength * 180; //old
            _angle = (GetTimeInSeconds() - _dayLength) / _nightLength * 180;
            _rotationPivot.localRotation =
                Quaternion.AngleAxis(_angle, Vector3.right);

            if (Application.isPlaying)
                GameManager.Instance.Temperature -= Time.deltaTime * 2;                         //temperature for the pet
        }

        if (Application.isPlaying && onRotationUpdate != null)
            onRotationUpdate(_angle + (IsDay() ? 0 : 180));
        //_moon.transform.LookAt(transform.position);
        //_sun.transform.LookAt(transform.position);
    }

    private void UpdateTime()
    {
        _timeNormalized += Time.deltaTime / _totalCycle;

        //InvokeTimeEvents();
        InvokeHourEvents();

        if (_timeNormalized >= 1) ResetDay();
    }

    private void ResetDay()
    {
        _timeNormalized = 0;
        //ResetTimeEvents();
        IncrementDays();

        //_maxTemperatureToday = UnityEngine.Random.Range(_minimumTemperature, _maximumTemperature);
        //_maxTemperatureToday = Mathf.PerlinNoise(_minimumTemperature, _maximumTemperature);
    }

    private void IncrementDays()
    {
        if (GameManager.Instance.GameRules != null)
            GameManager.Instance.GameRules.Days++;
    }

    #region Hour of Day
    public DayEvent AddEvent(int hour, System.Action action)
    {
        DayEvent dayEvent;
        dayEvent.time = hour;
        dayEvent.action = action;
        AddEvent(dayEvent);
        return dayEvent;
    }

    public void AddEvent(DayEvent dayEvent)
    {
        //Wrap
        dayEvent.time = Wrap(dayEvent.time, 0, 24);
        if (dayEvent.time == 24) dayEvent.time = 0;

        //Add
        int hour = dayEvent.time;
        if (!_dayEvents.ContainsKey(hour))
            _dayEvents[dayEvent.time] = new Hour(hour);

        _dayEvents[hour].events.Add(dayEvent);
    }

    public void RemoveEvent(DayEvent dayEvent)
    {
        //Wrap
        dayEvent.time = Wrap(dayEvent.time, 0, 24);
        if (dayEvent.time == 24) dayEvent.time = 0;

        //Add
        int hour = dayEvent.time;
        if (_dayEvents.ContainsKey(hour))
        {
            Hour hourObject = _dayEvents[dayEvent.time];

            hourObject.events.Remove(dayEvent);
        }
    }

    private void InvokeHourEvents()
    {
        if (!_dayEvents.ContainsKey(_hour) || _dayEvents[_hour].done) return;

        foreach (DayEvent dayEvent in _dayEvents[_hour].events)
            dayEvent.action();

        _dayEvents[_hour].done = true;
    }
    #endregion

    #region Time of Day

    private void InvokeTimeEvents()
    {
        if (!_dawnInvoked && _time >= _dawnTime)
        {
            onDawn.Invoke();
            _dawnInvoked = true;
        }

        if (!_noonInvoked && _time >= _noonTime)
        {
            onNoon.Invoke();
            _noonInvoked = true;
        }

        if (!_afternoonInvoked && _time >= _afternoonTime)
        {
            onAfternoon.Invoke();
            _afternoonInvoked = true;
        }

        if (!_duskInvoked && _time >= _duskTime)
        {
            onDusk.Invoke();
            _duskInvoked = true;
        }

        if (!_midnightInvoked && _time >= _midnightTime)
        {
            onMidnight.Invoke();
            _midnightInvoked = true;
        }
    }

    private void ResetTimeEvents()
    {
        _dawnInvoked = false;
        _noonInvoked = false;
        _afternoonInvoked = false;
        _duskInvoked = false;
        _midnightInvoked = false;
    }

    private void UpdateSetTimeEvents()
    {
        _dawnTime = 0;
        _duskTime = _dayLength / _totalCycle;
        _noonTime = _duskTime / 2;
        _midnightTime = 1 - _nightLength / _totalCycle / 2;
        _afternoonTime = _noonTime * 1.1f;
    }

    /// <summary>
    /// Sets the time of day. Time is between 0 and 1. 0 and 1 are Dawn. Everything else is relative.
    /// </summary>
    /// <param name="time">Between 0 and 1.</param>
    public void SetTimeOfDay(float time)
    {
        time = Mathf.Clamp01(time);
        _timeNormalized = time;

        UpdateRotation();
    }

    /// <summary>
    /// Automatically sets the time to dawn.
    /// </summary>
    public void SetDawn()
    {
        SetTimeOfDay(_dawnTime);
    }

    /// <summary>
    /// Automatically sets the time to dusk.
    /// </summary>
    public void SetDusk()
    {
        SetTimeOfDay(_duskTime);
    }

    /// <summary>
    /// Automatically sets the time to the start of the day.
    /// </summary>
    public void SetStartOffDay()
    {
        SetTimeOfDay(0.1f);
    }

    /// <summary>
    /// Automatically sets the time to the end of the day.
    /// </summary>
    public void SetEndOfDay()
    {
        SetTimeOfDay(_duskTime * 0.9f);
    }

    /// <summary>
    /// Automatically sets the time to the start of the night.
    /// </summary>
    public void SetStartOfNight()
    {
        SetTimeOfDay(_duskTime * 1.1f);
    }

    /// <summary>
    /// Automatically sets the time to the end of the night.
    /// </summary>
    public void SetEndOfNight()
    {
        SetTimeOfDay(0.9f);
    }

    /// <summary>
    /// Sets the time in seconds, as set by the total time of day and night in the inspector.
    /// </summary>
    /// <param name="time">The time in seconds for the day.</param>
    public void SetAbsoluteTime(float time)
    {
        SetTimeOfDay(time / _totalCycle);
    }
    #endregion

    #region Is Day and Is Night

    public float TimeDayNormalized()
    {
        return _timeDayNormalized;

        //if (IsNight()) return 1;

        //return TimeNormalized() / _dayLength;
    }

    public float TimeNightNormalized()
    {
        return _timeNightNormalized;

        //if (IsDay()) return 0;

        //return (TimeNormalized() - _dayLength) / _nightLength;
    }

    /// <summary>
    /// Is the sun in the sky?
    /// </summary>
    /// <returns>If it's day.</returns>
    public bool IsDay()
    {
        return GetTimeInSeconds() <= _dayLength;
    }

    /// <summary>
    /// Is the sun not in the sky?
    /// </summary>
    /// <returns>If it's night.</returns>
    public bool IsNight()
    {
        return !IsDay();
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
