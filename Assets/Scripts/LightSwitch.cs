using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Connectable, IPull, IPausable
{
    [Tooltip("Has enough energy to be running")]
    [ReadOnly, SerializeField] private bool _energized = false;
    private bool _wasEnergized = false;

    private bool _wasOn;
    private bool _awaitingAnswer = false;
    [SerializeField] private bool _isOn = false;

    [Header("Consumption Properties")]

    [Tooltip("Energy that the machine asks for")]
    [SerializeField] private float _energyPull;
    [Tooltip("Energy that the machine consumes every tick")]
    [SerializeField] private float _consumption;
    [Tooltip("Rate at which the consumption triggers")]
    [SerializeField] private float _consumptionRate;

    [Header("Debug")]
    public DisplayStats displayStats;

    private float timer = 0;
    public System.Action<bool> OnPowered; // On and Off

    // Use this for initialization
    void Start()
    {
        GameManager.Instance.LightSwitch = this;

        Lights[] lights = FindObjectsOfType<Lights>();
        for(int i = 0; i < lights.Length; i++)
            OnPowered += lights[i].ToggleOnOff;

        // Initialization
        IsPaused = false;
        _wasOn = _isOn;
        timer = _consumptionRate;
        OnPowered += Power;

        // Send an energy request
        StartCoroutine(delay());
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPaused) return;

        // IsOn dirty check
        if (_wasOn != _isOn)
        {
            if (OnPowered != null) OnPowered.Invoke(_isOn);
            _wasOn = _isOn;
        }

        // Checks if there's enough enery to power the machine
        _energized = _energy >= _consumption;

        if (_energized && _isOn)
        {
            // Consumes energy
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                _energy -= _consumption;
                timer = _consumptionRate;
            }
        }
        if (!_energized && _energized != _wasEnergized && _isOn)
        {
            if (OnPowered != null) OnPowered.Invoke(false);
            _wasEnergized = _energized;
            _isOn = false;
        }
        // Safety assigning?
        _wasEnergized = _energized;


        // Debug
        if (displayStats.texts.Count == 0) return;
        foreach (var text in displayStats.texts)
            if (text == null)
                return;
        displayStats.texts[0].gameObject.transform.parent.gameObject.SetActive(GameManager.Instance.DisplayDebug);

        displayStats.texts[0].text = "Energy: " + _energy;
        displayStats.texts[1].text = "Consumption: " + _consumption;
        displayStats.texts[2].text = "Energy Pull: " + _energyPull;
        displayStats.texts[3].text = "Capacity: " + _capacity;
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
        Debug.Log("On off");
        _isOn = !_isOn;
    }

    public bool IsPaused { get; set; }
    public void TogglePause()
    {
        IsPaused = !IsPaused;
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

    public bool IsON
    {
        get { return _isOn; }
        set { _isOn = value; }
    }
}
