using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : Connectable, IPull, IUpgradeable, IHaveEvents
{

    [Header("Storage properties")]
    [SerializeField] private int _maximumCapacity = 1000;
    [SerializeField] private int _minimumCapacity = 500;

    [Header("Overcharge properties")]
    [SerializeField] private float _overchargeTimer = 5f;
    [SerializeField] private int _overchargePenalty = 250;
    [SerializeField, Range(0, 1)] private float _Threshold = 0.9f;
    private float _currentOverTime = 0f;
    private bool _isOvercharging = false;
    private bool _isOverThreshold = false;

    [Header("Upgrades")]
    [SerializeField, Range(0, 10)] private int _currentUpgradeLevel = 0;
    [SerializeField] private int _maxUpgradeLevel = 2;
    [SerializeField, Range(0, 1)] private float _upgradePercentage = 0.3f;

    [SerializeField] private GameObject[] _upgradeGameObjects = null;

    [SerializeField] private UIFillBar _overchargeBar;
    private Coroutine _overchargeCor = null;
    public DisplayStats displayStats;

    public System.Action<int> OnUpgradeChanged;
    public System.Action OnOverheatStarted;
    public System.Action OnFixed;

    public float Capacity
    {
        get { return _capacity; }
        set
        {
            _capacity = Mathf.Clamp(value, _minimumCapacity, _maximumCapacity);
            if (onEnergyChange != null)
                onCapacityChange((int)_capacity);
        }
    }

    public float Energy
    {
        get { return _energy; }
        set
        {
            _energy = Mathf.Clamp(value, 0, Capacity);
            if (onEnergyChange != null)
                onEnergyChange((int)_energy);
        }
    }

    public float Threshold
    {
        // Gets 90% of the capacity
        get { return _capacity * _Threshold; }
    }

    public static Action<int> onMaxCapacityChange;
    public static Action<int> onCapacityChange;
    public static Action<int> onEnergyChange;

    private void OnValidate()
    {
        ForceEvents();
    }

    private void Start()
    {
        GameManager.Instance.Storage = this;
        //ForceEvents();
    }

    public void ForceEvents()
    {
        if (onMaxCapacityChange != null)
            onMaxCapacityChange(_maximumCapacity);
        if (onCapacityChange != null)
            onCapacityChange((int)_capacity);
        if (onEnergyChange != null)
            onEnergyChange((int)_energy);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the overcharge state
        checkOvercharge();
        UpdateVisualUpgrades();

        Capacity *= (1 + _currentUpgradeLevel * _upgradePercentage);
        _maximumCapacity *= (int)(1 + _currentUpgradeLevel * _upgradePercentage);
        _minimumCapacity *= (int)(1 + _currentUpgradeLevel * _upgradePercentage);

        #region Debug
        // Update Debug Bar

        transform.GetChild(0).GetChild(0).GetComponent<UIFillBar>().Value = (float)_energy / (float)_capacity;
        transform.GetChild(0).GetChild(1).GetComponent<UIFillBar>().Value = (float)_currentOverTime / (float)_overchargeTimer;

        // Update Debug tests
        if (displayStats.texts.Count == 0) return;
        foreach (var text in displayStats.texts)
            if (text == null)
                return;
        displayStats.texts[0].gameObject.transform.parent.gameObject.SetActive(GameManager.Instance.DisplayDebug);

        displayStats.texts[0].text = "Storage: " + Energy;
        displayStats.texts[1].text = "Capacity: " + Capacity;
        displayStats.texts[2].text = "MaximumCapacity: " + _maximumCapacity;
        displayStats.texts[3].text = "MinimumCapacity: " + _minimumCapacity;
        #endregion
    }

    /// <summary>
    /// Implements IPull. Receives Energy
    /// </summary>
    /// <param name="value"></param>
    public void Pull(float value)
    {
        ReceiveEnergy(value);
        Energy = _energy;
    }

    /// <summary>
    /// Pushes X amount of energy to a specific Connectable [Used Internally]
    /// </summary>
    /// <param name="con"></param>
    /// <param name="value"></param>
    public void PushTo(Connectable con, float value)
    {
        IPull puller = con as IPull;
        // If it has enough energy stored, send it to the requester
        if (!_isOvercharging && Energy >= value)
        {
            puller.Pull(value);
            Energy -= value;
        }
        else
        {
            // Still transmit that the puller has gotten an answer
            puller.Pull(0);
        }
    }

    /// <summary>
    /// Stops the overcharging
    /// </summary>
    public void Fix()
    {
        if (_overchargeBar != null)
            _overchargeBar.gameObject.SetActive(false);

        if (OnFixed != null) OnFixed.Invoke();

        Energy = Mathf.Min(Energy, _capacity * 0.9f);
        _isOvercharging = false;
        _currentOverTime = 0;
        if (_overchargeCor != null) StopCoroutine(_overchargeCor);
    }

    /// <summary>
    /// Depletes the storage and reduces the maximum capacity
    /// </summary>
    private void checkOvercharge()
    {
        if (_isOvercharging) return;

        _isOverThreshold = Energy > Threshold;

        if (Energy >= Capacity)
        {
            if (OnOverheatStarted != null) OnOverheatStarted.Invoke();

            _isOvercharging = true;
            _overchargeCor = StartCoroutine(overchargeTimer());
        }
    }

    /// <summary>
    /// Depletes the energy stored and decreases capacity
    /// </summary>
    private void Overcharge()
    {
        _isOvercharging = false;

        if (Capacity > _minimumCapacity)
        {
            //Capacity -= _overchargePenalty;
            Energy = Mathf.Clamp(Energy, _minimumCapacity, _capacity);
        }

    }

    /// <summary>
    /// Activates the bar and runs the timer until done.
    /// </summary>
    /// <returns></returns>
    private IEnumerator overchargeTimer()
    {
        // Activate UI
        _overchargeBar.gameObject.SetActive(true);

        // Run the timer
        while (_isOvercharging && _currentOverTime < _overchargeTimer)
        {
            yield return new WaitForSeconds(0.01f);
            _currentOverTime += 0.01f;
            if (Energy < Capacity) _isOvercharging = false;
        }

        if (_isOvercharging)
        {
            Overcharge();
        }

        // Deactivate UI and reset timer.
        //_overchargeBar.gameObject.SetActive(false);
        _currentOverTime = 0;
    }

    public bool Overcharging
    {
        get { return _isOvercharging; }
        set { _isOvercharging = value; }
    }

    public bool OverThreshold
    {
        get { return _isOverThreshold; }
        set { _isOverThreshold = value; }
    }
    //public float Energy
    //{
    //    get { return _energy; }
    //    set { _energy = value; }
    //}

    #region Upgrades
    public int MaxUpgradeLevel
    {
        get { return _maxUpgradeLevel; }
        set { _maxUpgradeLevel = value; }
    }

    public int CurrentUpgradeLevel
    {
        get { return _currentUpgradeLevel; }
        set
        {
            _currentUpgradeLevel = Mathf.Clamp(value, 0, MaxUpgradeLevel);

            if (OnUpgradeChanged != null)
                OnUpgradeChanged.Invoke(_currentUpgradeLevel);
        }
    }

    public void Upgrade(int byHowManyLevels)
    {
        CurrentUpgradeLevel += byHowManyLevels;
    }

    public void UpgradeTo(int setLevel)
    {
        CurrentUpgradeLevel = setLevel;
    }

    private void UpdateVisualUpgrades()
    {
        if (_upgradeGameObjects[0])
            _upgradeGameObjects[0].SetActive(_currentUpgradeLevel >= 1);
        if (_upgradeGameObjects[1])
            _upgradeGameObjects[1].SetActive(_currentUpgradeLevel >= 2);
    }
    #endregion
}
