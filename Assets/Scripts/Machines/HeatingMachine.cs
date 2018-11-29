using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeatingMachine : Connectable, IPull, IBreakable, IPausable
{

    [Tooltip("Has enough energy to be running")]
    [ReadOnly, SerializeField] private bool _energized = false;

    private bool _wasOn;
    private bool _awaitingAnswer = false;
    [SerializeField] private bool _isOn = true;

    [Header("Consumption Properties")]

    [Tooltip("Energy that the machine asks for")]
    [SerializeField] private float _energyPull;
    [Tooltip("Energy that the machine consumes every tick")]
    [SerializeField] private float _consumption;
    [Tooltip("Rate at which the consumption triggers")]
    [SerializeField] private float _consumptionRate;

    [SerializeField] private float _tempChange;
    [SerializeField] private float _polutionEmited;

    [Header("Debug")]
    public DisplayStats displayStats;
    private float timer = 0;
    private float otherTimer = 0;
    public System.Action<bool> OnPowered; // On and Off

    private Animator _anime;

    // Use this for initialization
    void Awake()
    {
        GameManager.Instance.HeatingMachine = this;

        _anime = GetComponentInChildren<Animator>();

        // Initialization
        _wasOn = _isOn;
        timer = _consumptionRate;
        OnPowered += Power;

        // Send an energy request
        StartCoroutine(delay());
    }

    // Update is called once per frame
    void Update()
    {
        // IsOn dirty check
        if (_wasOn != _isOn)
        {
            if (OnPowered != null) OnPowered.Invoke(_isOn);
            _wasOn = _isOn;
        }

        if (_anime != null && _anime.isActiveAndEnabled) _anime.SetBool("IsOn", _isOn);

        // Checks if there's enough enery to power the machine
        _energized = _energy >= _consumption;
        if (_energized)
        {
            // Consumes energy
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                _energy -= _consumption;
                timer = _consumptionRate;

                if (GameManager.Instance.PollutionManager != null)
                    GameManager.Instance.PollutionManager.Pollution += _polutionEmited;
            }
        }

        if (_isOn)
        {
            //otherTimer += Time.deltaTime;
            //if (otherTimer > 1)
            //{
            if (GameManager.Instance.Temperature > 50) GameManager.Instance.Temperature -= _tempChange / 60;
            else GameManager.Instance.Temperature += _tempChange / 60;
            //otherTimer = 0;
            //}
        }

        // Debug
        if (displayStats.texts.Count == 0) return;
        foreach (var text in displayStats.texts)
            if (text == null)
                return;
        displayStats.texts[0].gameObject.transform.parent.gameObject.SetActive(GameManager.Instance.DisplayDebug);

        displayStats.texts[0].text = "ON?: " + _isOn;
        displayStats.texts[1].text = "Global T: " + (int)GameManager.Instance.Temperature;
        displayStats.texts[2].text = "Energy Pull: " + _energyPull;
    }

    /// <summary>
    /// IPull implementation, receives energy and sends a new request
    /// </summary>
    /// <param name="value"></param>
    public void Pull(float value)
    {
        if (!_isOn) return;

        ReceiveEnergy(value);
        _awaitingAnswer = false;

        // SendRequest wrapped in a 1 frame delay
        StartCoroutine(delay());
    }

    public void ToggleOnOff()
    {
        _isOn = !_isOn;
    }

    public void Break()
    {
        _isOn = false;
    }

    public void Fix()
    {
        if (_isOn == false)
        {
            _isOn = true;
            GameManager.Instance.BreakEventSystem.Fix(this);
        }
    }

    // SendRequest wrapped in a 1 frame delay
    private IEnumerator delay()
    {
        yield return null;
        if (SendRequest != null)
            SendRequest.Invoke(this, _energyPull);
    }

    /// <summary>
    /// if Powered On, send energy pull request
    /// </summary>
    /// <param name="value"></param>
    private void Power(bool value)
    {
        if (value == true)
        {
            // Send a request
            if (_awaitingAnswer == false && SendRequest != null)
            {
                SendRequest.Invoke(this, _energyPull);
                _awaitingAnswer = true;
            }
        }
    }

    public void TogglePause()
    {
        ToggleOnOff();
    }

    public bool IsPaused
    {
        get { return _isOn; }
        set { _isOn = value; }
    }

    public bool IsOn
    {
        get { return _isOn; }
        set { _isOn = value; }
    }
}
