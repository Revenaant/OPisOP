using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;

//[RequireComponent(typeof(TapGesture))]
public class SolarPanel : Connectable, IBreakable, IPush, IUpgradeable, IHaveEvents
{
    //UnityEditor.

    //private TapGesture _tapGesture = null;

    //private float _targetLightRange = 0;
    //private float _startRange = 0;
    private float _timeSinceLastInterval = -1;
    private float _timeSinceLastCleanCooldown = -1;
    private bool _hasBeenFullyCleanedOnce = true;

    [Header("Checks")]
    [SerializeField]
    private bool _paused = false;
    [SerializeField] private bool _canClean = false;

    [Header("Time (seconds)")]

    [SerializeField, Tooltip("Per time given, or no cooldown less than 0.")]
    private float _cleanCooldown = 0.5f;
    [SerializeField, Tooltip("Per time given, or every frame if less than 0.")]
    private float _interval = -1;

    [Header("Resource Generation")]

    [SerializeField] private float _resourceGeneration = 1.2f;
    [SerializeField, ReadOnly] private float _currentResourceGeneration = 0;
    //[SerializeField] private bool _isDirty = false;

    [Header("Dirt and Cleaning")]

    [SerializeField, Range(0, 1)] private float _dirt = 0;
    [SerializeField, Range(0, 1)] private float _dirtPerBreak = 0.3f;
    [SerializeField, Range(0, 1)] private float _cleanPerFix = 0.2f;

    [Header("Upgrades")]

    [SerializeField, Range(0, 10)] private int _currentUpgradeLevel = 0;
    [SerializeField] private int _maxUpgradeLevel = 2;
    [SerializeField, Range(0, 1)] private float _upgradePercentage = 0.3f;

    //[SerializeField] private GameObject _upgradeGameObject1 = null;
    //[SerializeField] private GameObject _upgradeGameObject2 = null;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private Light _sun = null;
    [SerializeField] private Transform _rotationPivot = null;

    [Header("Visual Feedback")]
    //[SerializeField]
    //private Light _panelLight = null;
    [SerializeField] private List<GameObject> _upgradeGameObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> _dirtyGameObjects = new List<GameObject>();
    private Animator _anime;

    public static Action<float> onDirtChange;

    // For Debug testing
    public DisplayStats displayStats;
    public System.Action<float> OnGenerate;

    public System.Action<int> OnUpgradeChanged;
    public System.Action OnCleaned;
    public System.Action OnFullyCleaned;

    #region Tap Gesture
    //private void OnEnable()
    //{
    //    if (_tapGesture == null)
    //        _tapGesture = GetComponent<TapGesture>();
    //    _tapGesture.Tapped += TappedHandler;
    //}

    //private void OnDisable()
    //{
    //    _tapGesture.Tapped -= TappedHandler;
    //}

    //private void TappedHandler(object sender, System.EventArgs e)
    //{
    //    //Debug.Log("Solar Panel Tap");
    //    Fix();
    //}
    #endregion

    // Use this for initialization
    private void Start()
    {
        GameManager.Instance.SolarPanel = this;
        //Debug.Assert(_dirt != null, "Dirt is not assigned.");
        //Debug.Assert(_panelLight != null, "Panel Light is not assigned.");

        //_startRange = _panelLight.range;
        _anime = GetComponentInChildren<Animator>();
        OnGenerate += Push;
    }

    private void OnValidate()
    {
        Dirt = _dirt;
        UpdateVisualUpgrades();
        UpdateSolarPanelRotation();

        _timeSinceLastInterval = Time.time;
        _timeSinceLastCleanCooldown = Time.time;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_paused)
        {
            //_panelLight.enabled = false;
            return;
        }

        UpdateVisualUpgrades();
        UpdateSolarPanelRotation();

        if (!_canClean) CheckCleaningCooldown();

        //if (IsDirty())
        _currentResourceGeneration = _resourceGeneration / (1 + _dirt) * (1 + _currentUpgradeLevel * _upgradePercentage);

        bool day = IsDay();

        //_panelLight.enabled = day;

        if (day)
        {
            //LightPulsating();

            GenerateResource();
        }

        DisplayStats();
    }

    private void DisplayStats()
    {

        //Display Text
        if (displayStats.texts.Count == 0) return;

        foreach (var text in displayStats.texts)
            if (text == null)
                return;
        displayStats.texts[0].gameObject.transform.parent.gameObject.SetActive(GameManager.Instance.DisplayDebug);

        {
            //displayStats.texts[0].text = "Energy: " + allResources;
            displayStats.texts[1].text = "Generation: " + _resourceGeneration;
            displayStats.texts[2].text = "Interval: " + _interval + "s";
            displayStats.texts[3].text = IsDirty() ? "Dirty" : "Clean";
        }
    }

    //private void LightPulsating()
    //{
    //    _panelLight.range = Mathf.Lerp(_panelLight.range, _targetLightRange, 0.05f);
    //    if (!(Mathf.Abs(_panelLight.range - _targetLightRange) <= 0.1f)) return;

    //    if (_targetLightRange > _panelLight.range) _targetLightRange = _startRange - 2.5f;
    //    else _targetLightRange = _startRange + 5;
    //}

    private void GenerateResource()
    {
        if (_interval < 0)
        {
            IncreaseResources(_currentResourceGeneration);
        }
        else if (Time.time > _timeSinceLastInterval + _interval)
        {
            IncreaseResources(_currentResourceGeneration);
            _timeSinceLastInterval = Time.time;
        }
    }

    public void IncreaseResources(float amount)
    {
        //allResources += amount;
        if (OnGenerate != null)
            OnGenerate.Invoke(amount);
    }

    private bool IsDay()
    {
        var dnc = Application.isPlaying ? GameManager.Instance.DayNightCycle : null;
        return dnc == null || dnc.IsDay();
    }

    private void UpdateSolarPanelRotation()
    {
        if (_rotationPivot == null) return;

        if (_sun != null && IsDay())
        {
            _rotationPivot.rotation = Quaternion.RotateTowards(
                _rotationPivot.rotation,
                Quaternion.LookRotation(-_sun.transform.forward)/* * Quaternion.AngleAxis(90, Vector3.right)*/,
                //Quaternion.LookRotation((_sun.transform.position - _pivot.position).normalized),
                _rotationSpeed);
            //Quaternion.LookRotation(Vector3.forward);
            //_pivot.LookAt(_sun.transform);
            //var rotation = _pivot.localRotation;
            //var rotation = Quaternion.Inverse(_pivot.localRotation);
            //rotation.x = 0;
            //rotation.z = 0;
            //_pivot.rotation = rotation;
            //Quaternion.Inverse(rotation);
        }
        else
        {
            _rotationPivot.rotation = Quaternion.RotateTowards(
                _rotationPivot.rotation,
                Quaternion.LookRotation(Vector3.up)/* * Quaternion.AngleAxis(90, Vector3.right)*/,
                _rotationSpeed);
        }

        //_pivot.rotation = Quaternion.Angle
    }

    public void Push(float value)
    {
        // Divide the amount between all connections
        value /= _connections.Count;

        for (int i = 0; i < _connections.Count; i++)
        {
            IPull puller = GetConnection(i) as IPull;
            if (puller != null) puller.Pull(value);
            //allResources -= value;
        }
    }

    #region Dirt

    private void UpdateVisualDirt()
    {
        if (_dirtyGameObjects.Count == 0) return;

        int shouldBeEnabled = (int)(_dirt * _dirtyGameObjects.Count);
        for (int i = 0; i < _dirtyGameObjects.Count; i++)
            if (_dirtyGameObjects[i] != null)
                _dirtyGameObjects[i].SetActive(i < shouldBeEnabled);
    }

    private void StartCleaningCooldown()
    {
        _canClean = false;
        _timeSinceLastCleanCooldown = Time.time;
    }

    private void CheckCleaningCooldown()
    {
        if (Time.time > _timeSinceLastCleanCooldown + _cleanCooldown)
            _canClean = true;
    }

    /// <summary>
    /// Is the Solar Panel Dirty.
    /// </summary>
    /// <returns></returns>
    public bool IsDirty()
    {
        return _dirt > Mathf.Epsilon;
    }

    /// <summary>
    /// How dirty the Solar Panel is.
    /// </summary>
    public float Dirt
    {
        get { return _dirt; }
        set
        {
            float clamped = Mathf.Clamp01(value);
            if (Math.Abs(_dirt - clamped) > Mathf.Epsilon
                && onDirtChange != null) onDirtChange(clamped);
            _dirt = clamped;
            UpdateVisualDirt();
            //_hasBeenFullyCleanedOnce = value < Mathf.Epsilon;
        }
    }

    /// <summary>
    /// Clean the dirt off the Solar Panel by a specified amount in the inspector.
    /// </summary>
    public void Clean()
    {
        if (!_canClean) return;
        Dirt -= _cleanPerFix;
        StartCleaningCooldown();

        if (Dirt > Mathf.Epsilon)
        {
            if (OnCleaned != null) OnCleaned.Invoke();
            _hasBeenFullyCleanedOnce = false;
        }
        else if (!_hasBeenFullyCleanedOnce)
        {
            if (OnFullyCleaned != null)
                OnFullyCleaned.Invoke();
            _hasBeenFullyCleanedOnce = true;
        }
    }

    /// <summary>
    /// Make the Solar Panel dirty by a specified amount in the inspector.
    /// </summary>
    public void MakeDirty()
    {
        Dirt += _dirtPerBreak;
    }

    /// <summary>
    /// Completely clean the dirt off the Solar Panel.
    /// </summary>
    public void CleanAll()
    {
        Dirt = 0;
    }

    /// <summary>
    /// Completely make the Solar Panel dirty.
    /// </summary>
    public void MakeCompletelyDirty()
    {
        Dirt = 1;
    }
    #endregion
    #region Break Fix
    /// <summary>
    /// Breaks the machine.
    /// </summary>
    public void Break()
    {
        MakeDirty();
    }

    /// <summary>
    /// Fix the machine and make sure the system knows it's fixed.
    /// </summary>
    public void Fix()
    {
        Clean();
        if (GameManager.Instance.BreakEventSystem != null)
            GameManager.Instance.BreakEventSystem.Fix(this);
    }
    #endregion
    #region Pause
    /// <summary>
    /// Toggles the pause.
    /// </summary>
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

            GameManager.Instance.StaticFunctions.
            StartCoroutine(MyCoroutines.DoWhen(() => isActiveAndEnabled, () =>
            {
                //Debug.Log("Finally enabled");
                if (_anime != null)
                    _anime.SetInteger("Upgrade", CurrentUpgradeLevel);
            }));
            //if (_anime != null)
            //    _anime.SetInteger("Upgrade", CurrentUpgradeLevel);

            if (OnUpgradeChanged != null)
                OnUpgradeChanged.Invoke(_currentUpgradeLevel);
        }
    }

    public void Upgrade(int byHowManyLevels)
    {
        CurrentUpgradeLevel += byHowManyLevels;
        //_anime.SetInteger("Upgrade", CurrentUpgradeLevel);
    }

    public void UpgradeTo(int setLevel)
    {
        CurrentUpgradeLevel = setLevel;
        //_anime.SetInteger("Upgrade", CurrentUpgradeLevel);
    }

    private void UpdateVisualUpgrades()
    {
        for (int i = 0; i < 3; i++)
            _upgradeGameObjects[i].SetActive(_currentUpgradeLevel > i);
    }

    //private void UpdateVisualUpgrades()
    //{
    //    if (_upgradeGameObject1)
    //        _upgradeGameObject1.SetActive(_currentUpgradeLevel >= 1);
    //    if (_upgradeGameObject2)
    //        _upgradeGameObject2.SetActive(_currentUpgradeLevel >= 2);
    //}
    #endregion

    public void ForceEvents()
    {
        if (onDirtChange != null)
            onDirtChange(_dirt);
    }
}
