using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TouchScript.Gestures;
using UnityEngine;

public class PlantBed : Connectable, IPull, IUpgradeable
{
    [Serializable]
    private class Cactus
    {
        [HideInInspector]
        public GameObject prefab = null;

        [SerializeField, ReadOnly] private float _timestamp = 0;
        [SerializeField, ReadOnly] private float _lifetime = 0;
        [SerializeField, ReadOnly] private GameObject _gameObject = null;

        public Cactus(GameObject prefab, float lifetime = 180)
        {
            _timestamp = Time.time;
            _lifetime = lifetime;
            this.prefab = prefab;
            _gameObject = Instantiate(prefab);
        }

        public void Kill()
        {
            Destroy(_gameObject);
        }

        public float Lifetime
        {
            get { return _lifetime; }
            set { _lifetime = value; }
        }

        public GameObject GameObject
        {
            get { return _gameObject; }
            set { _gameObject = value; }
        }

        public float TimeOfDeath
        {
            get { return _timestamp + _lifetime; }
        }
    }

    [Serializable]
    private class Spot
    {
        [ReadOnly]
        public Cactus cactus = null;

        [SerializeField] private Transform _transform = null;
        [SerializeField] private GameObject _gameObject = null;

        public Transform Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public GameObject GameObject
        {
            get { return _gameObject; }
            set { _gameObject = value; }
        }

        public bool Occupied
        {
            get { return CactusExists(ref cactus); }
        }

        public void Occupy(Cactus p_cactus)
        {
            cactus = p_cactus;
            cactus.GameObject.transform.SetParent(_transform, true);
            cactus.GameObject.transform.position = _transform.position;
        }

        public void Vacate()
        {
            //occupant = null;
            _transform.DetachChildren();
        }
    }

    //private TapGesture _tapGesture = null;
    private float _lastCleaned = 0;
    private Dictionary<int, Spot> _spots = new Dictionary<int, Spot>();
    private List<GameObject> _cactusPrefabsExclusive = new List<GameObject>();

    private bool _waitingForEnergy = false;

    [Header("Cacti")]
    [SerializeField] private List<GameObject> _cactusPrefabs = new List<GameObject>();
    [SerializeField] private float _energyPerCactus = 10;
    [SerializeField] private float _pollutionReductionPerCactus = 0.05f;
    [SerializeField, ReadOnly] private int _currentCacti = 0;

    [Header("Upgrades")]
    [SerializeField, ReadOnly] private int _maxCacti = 3;
    [SerializeField] private int _currentMaxCacti = 1;

    [Header("Time (seconds)")]
    [SerializeField] private float _cactiLifetime = 180;
    [SerializeField] private float _cactiCleanInterval = 4;

    [Header("Spots")]
    [SerializeField] private Spot _spot1 = null;
    [SerializeField] private Spot _spot2 = null;
    [SerializeField] private Spot _spot3 = null;

    public System.Action OnMake;
    public System.Action OnDead;

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
    //    //Debug.Log("Plant Bed tap");
    //    //Populate();
    //    RequestEnergy(_energyPerCactus);
    //}
    #endregion

    private void OnValidate()
    {
        _lastCleaned = Application.isPlaying ? Time.time : 0;
    }

    // Use this for initialization
    private void Start()
    {
        GameManager.Instance.PlantBed = this;

        //Debug.Assert(_cactusPrefab != null, "Cactus Prefab must be assigned.");
        Debug.Assert(_spot1 != null && _spot1.Transform != null && _spot1.GameObject != null,
            "Spot 1 and Transform and GameObject must be assigned.");
        Debug.Assert(_spot2 != null && _spot2.Transform != null && _spot2.GameObject != null,
            "Spot 2 and Transform and GameObject  must be assigned.");
        Debug.Assert(_spot3 != null && _spot3.Transform != null && _spot3.GameObject != null,
            "Spot 3 and Transform and GameObject  must be assigned.");

        _spots[1] = _spot1;
        _spots[2] = _spot2;
        _spots[3] = _spot3;

        _cactusPrefabsExclusive = new List<GameObject>(_cactusPrefabs);

        CurrentUpgradeLevel = _currentMaxCacti;
    }

    // Update is called once per frame
    private void Update()
    {
        ////Debug
        //if (Input.GetKeyDown(KeyCode.Q)) Populate();
        //if (Input.GetKeyDown(KeyCode.E)) Clear(ref _spot1.cactus);
        //if (Input.GetKeyDown(KeyCode.R)) Clear(ref _spot2.cactus);
        //if (Input.GetKeyDown(KeyCode.T)) Clear(ref _spot3.cactus);

        ClearPollution();
        CheckCactiLifetime();
    }

    private void Clear(ref Cactus cactus)
    {
        if (cactus == null) return;

        cactus.Kill();
        cactus = null;
        _currentCacti--;

        if (OnDead != null)
            OnDead.Invoke();
    }

    private void ClearAll()
    {
        //for (int i = _cacti.Count - 1; i >= 0; i--)
        //    _cacti[i] = null;

        //_cacti.Clear();

        foreach (var kvp in _spots)
        {
            var spot = kvp.Value;

            if (!spot.Occupied) continue;

            spot.Vacate();
            Clear(ref spot.cactus);
        }
    }

    private void CheckCactiLifetime()
    {
        foreach (var kvp in _spots)
        {
            var spot = kvp.Value;
            if (!spot.Occupied || !(Time.time > spot.cactus.TimeOfDeath)) continue;

            _cactusPrefabsExclusive.Add(spot.cactus.prefab);
            spot.Vacate();
            Clear(ref spot.cactus);
        }
    }

    private void ClearPollution()
    {
        float cleaned = _currentCacti * _pollutionReductionPerCactus;

        if (!(Time.time > _lastCleaned + _cactiCleanInterval) || GameManager.Instance.PollutionManager == null) return;

        GameManager.Instance.PollutionManager.Pollution -= cleaned;
        _lastCleaned = Time.time;

    }

    private static bool CactusExists(ref Cactus cactus)
    {
        return cactus != null && cactus.GameObject != null;
    }

    private void CreateCactus(int spot)
    {
        if (_cactusPrefabsExclusive.Count == 0) return;

        _spots[spot].Occupy(new Cactus(GetRandomExclusiveCactusPrefab(), _cactiLifetime));
        _currentCacti++;

        if (OnMake != null)
            OnMake.Invoke();
    }

    private GameObject GetRandomExclusiveCactusPrefab()
    {
        int spot = UnityEngine.Random.Range(0, _cactusPrefabsExclusive.Count);
        GameObject prefab = _cactusPrefabsExclusive[spot];
        _cactusPrefabsExclusive.RemoveAt(spot);

        return prefab;
    }

    private void Populate()
    {
        string message = "No available spots to create a cactus in.";
        bool found = false;
        int spot = 0;

        foreach (var kvp in _spots)
        {
            found = !kvp.Value.Occupied && _currentCacti < _currentMaxCacti;
            if (!found) continue;

            spot = kvp.Key;
            break;
        }

        if (found)
        {
            CreateCactus(spot);
            message = "Cactus created at spot " + spot + ".";
        }

        Debug.Log(message);
    }

    public int Cacti
    {
        get { return _currentCacti; }
    }

    #region Energy

    public void PingForNewCactus()
    {
        if (_waitingForEnergy) return;
        if (_currentCacti == _currentMaxCacti) return;
        RequestEnergy(_energyPerCactus);
        _waitingForEnergy = true;
    }

    public void Pull(float value)
    {
        //ReceiveEnergy(value);

        //if (_energy >= _energyPerCactus)
        if (value >= _energyPerCactus)
        {
            Populate();
            //StartCoroutine(MyCoroutines.WaitOneFrame(() =>
            //    _energy -= _energyPerCactus));
            //_energy -= _energyPerCactus;
        }

        _waitingForEnergy = false;
    }
    #endregion

    #region Upgrades
    public int MaxUpgradeLevel
    {
        get { return _maxCacti; }
        set { _maxCacti = value; }
    }

    public int CurrentUpgradeLevel
    {
        get { return _currentMaxCacti; }
        set
        {
            _currentMaxCacti = Mathf.Clamp(value, 1, MaxUpgradeLevel);

            for (int i = 0; i < _maxCacti; i++)
                _spots[i + 1].GameObject.SetActive(i < _currentMaxCacti);
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
    #endregion
}
