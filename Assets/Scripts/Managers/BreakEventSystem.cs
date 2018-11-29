using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public sealed class BreakEventSystem : MonoBehaviour, IPausable
{
    [SerializeField] private bool _paused = false;

    [Header("Input")]
    [SerializeField] private float _breakInterval = 10;
    [SerializeField] private float _breakRandomIntervalVariance = 2;
    [SerializeField] private int _breakMaximum = 5;

    [Header("Visuals")]
    [SerializeField, ReadOnly] private float _randomBreakInterval = 0;
    [SerializeField, ReadOnly] private int _brokenCount = 0;

#pragma warning disable 0414
    [SerializeField, ReadOnly] private MonoBehaviour _nextToBreak_ = null;
#pragma warning restore 0414
    [SerializeField, ReadOnly] private List<MonoBehaviour> _breakables_ = new List<MonoBehaviour>();

    private IBreakable _nextToBreak = null;
    private List<IBreakable> _breakables = new List<IBreakable>();
    private List<IBreakable> _currentlyBroken = new List<IBreakable>();

    private float _timeSinceLastInterval = 0;
    private int _maxBroken = -1;

    /// <summary>
    /// The interval in seconds when breaking breakable stuff occurs.
    /// </summary>
    public float breakInterval
    {
        get { return _breakInterval; }
        set { _breakInterval = value; }
    }

    /// <summary>
    /// The interval that randomises the break interval [-variance, variance].
    /// </summary>
    public float breakRandomIntervalVariance
    {
        get { return _breakRandomIntervalVariance; }
        set { _breakRandomIntervalVariance = value; }
    }

    /// <summary>
    /// The maximum amount of broken stuff at once.
    /// </summary>
    public int breakMaximum
    {
        get { return _breakMaximum; }
        set { _breakMaximum = value; }
    }

    public UnityEvent onBreak;

    private void OnValidate()
    {
        _timeSinceLastInterval = Time.time;
    }

    // Use this for initialization
    private void Start()
    {
        foreach (MonoBehaviour monoBehaviour in FindObjectsOfType<MonoBehaviour>())
            if (monoBehaviour is IBreakable)
            {
                _breakables_.Add(monoBehaviour);
                _breakables.Add(monoBehaviour as IBreakable);
            }

        _maxBroken = _breakables.Count;

        GameManager.Instance.BreakEventSystem = this;
    }

    // Update is called once per frame
    private void Update()
    {
        _breakMaximum = Mathf.Clamp(_breakMaximum, 0, _maxBroken);
        _brokenCount = Mathf.Clamp(_brokenCount, 0, _breakMaximum);
        if (_paused || _brokenCount >= _breakMaximum || _breakables.Count == 0) return; 

        if (Math.Abs(_randomBreakInterval) < Mathf.Epsilon)
            _randomBreakInterval =
                _breakInterval + Random.Range(-_breakRandomIntervalVariance, _breakRandomIntervalVariance);

        if (_nextToBreak == null)
        {
            int index = Random.Range(0, _breakables.Count);
            _nextToBreak = _breakables[index];
            _nextToBreak_ = _breakables_[index];
        }

        if (_breakInterval < 0)
        {
            Break(_nextToBreak);
            _nextToBreak = null;
            _nextToBreak_ = null;

            if (onBreak != null) onBreak.Invoke();
        }
        else if (Time.time > _timeSinceLastInterval + _randomBreakInterval)
        {
            Break(_nextToBreak);
            _nextToBreak = null;
            _nextToBreak_ = null;

            _timeSinceLastInterval = Time.time;
            _randomBreakInterval = 0;

            if (onBreak != null) onBreak.Invoke();
        }

    }

    public void TogglePause()
    {
        _paused = !_paused;
    }

    public bool IsPaused
    {
        get { return _paused; }
        set { _paused = value; }
    }

    private void Break(IBreakable breakable)
    {
        breakable.Break();
        _breakables.Remove(breakable);
        _currentlyBroken.Add(breakable);
        _brokenCount++;

        //Debug.Log("Something broke");
    }

    public void Fix(IBreakable breakable)
    {
        _currentlyBroken.Remove(breakable);
        _breakables.Add(breakable);
        _brokenCount--;
    }
}