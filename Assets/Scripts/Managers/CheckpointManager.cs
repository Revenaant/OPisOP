using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckpointManager : MonoBehaviour
{
    [System.Serializable]
    private class Checkpoint
    {
        public int day = -1;
        public int upgrades = 1;

        public Checkpoint(int day, int upgrades)
        {
            this.day = day;
            this.upgrades = upgrades;
        }
    }

    private int _currentDay = -1;
    private Checkpoint _currentCheckpoint = null;

    [SerializeField, ReadOnly]
    private bool _isCheckpoint = false;

    [Header("Checkpoints happen at the start of the assigned day.")]

    [SerializeField, ReadOnly]
    private int _checkpointsPassed = 0;

    [SerializeField]
    private bool _cumulativeUpgrades = true;
    [SerializeField]
    private List<Checkpoint> _checkpoints = new List<Checkpoint>();

    [Header("Checkpoint Events")]
    public UnityEvent OnCheckpointStart;
    public UnityEvent OnCheckpointEnd;

    public int CheckpointsPassed
    {
        get { return _checkpointsPassed; }
    }

    //Other
    public static Action onCheckpointStart;
    public static Action onCheckpointEnd;

    public static Action onUpgradeSolarPanel;
    public static Action onUpgradeStorage;
    public static Action onUpgradeCacti;

    public static Action onSuccessUpgradeSolarPanel;
    public static Action onSuccessUpgradeStorage;
    public static Action onSuccessUpgradeCacti;

    public static Action onFailUpgradeSolarPanel;
    public static Action onFailUpgradeStorage;
    public static Action onFailUpgradeCacti;

    public static Action<int> onUpgradesChange;

    public bool IsCheckpoint
    {
        get { return _isCheckpoint; }
        set
        {
            //_isCheckpoint = value;
            if (value) StartCheckpoint();
            else EndCheckpoint();
        }
    }

    // Use this for initialization
    private void Start()
    {
        GameManager.Instance.CheckpointManager = this;

        //Debug.Log(null);
        //Debug.Log("Start: " + (GameManager.Instance.SolarPanel != null));
    }

    private void OnEnable()
    {
        StartCoroutine(MyCoroutines.WaitOneFrame(SubscribeUpgrades));
        onCheckpointStart += UpdateUpgradesUI;
        onCheckpointStart += () => _checkpointsPassed++;
        GameRules.onEnd += StartCheckpoint;
    }

    private void OnDisable()
    {
        if (onCheckpointStart != null)
            onCheckpointStart -= UpdateUpgradesUI;
        if (GameRules.onEnd != null)
            GameRules.onEnd -= StartCheckpoint;
    }

    private void UpdateUpgradesUI()
    {
        if (_currentCheckpoint != null && onUpgradesChange != null)
            onUpgradesChange(_currentCheckpoint.upgrades);
    }

    private void SubscribeUpgrades()
    {
        onUpgradeSolarPanel += () =>
        {
            if (Upgrade(GameManager.Instance.SolarPanel))
            {
                if (onSuccessUpgradeSolarPanel != null)
                    onSuccessUpgradeSolarPanel();
            }
            else
            {
                Debug.Log("Failed to upgrade Solar Panel.");
                if (onFailUpgradeSolarPanel != null)
                    onFailUpgradeSolarPanel();
            }
        };
        onUpgradeStorage += () =>
        {
            if (Upgrade(GameManager.Instance.Storage))
            {
                if (onSuccessUpgradeStorage != null)
                    onSuccessUpgradeStorage();
            }
            else
            {
                Debug.Log("Failed to upgrade Battery Storage.");
                if (onFailUpgradeStorage != null)
                    onFailUpgradeStorage();
            }
        };
        onUpgradeCacti += () =>
        {
            if (Upgrade(GameManager.Instance.PlantBed))
            {
                if (onSuccessUpgradeCacti != null)
                    onSuccessUpgradeCacti();
            }
            else
            {
                Debug.Log("Failed to upgrade Plant Bed of Cacti.");
                if (onFailUpgradeCacti != null)
                    onFailUpgradeCacti();
            }
        };
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isCheckpoint)
        {
            //if (Input.GetKeyDown(KeyCode.C))
            //    EndCheckpoint();
            return;
        }

        //if (Input.GetKeyDown(KeyCode.X))
        //    StartCheckpoint();

        int day = GameManager.Instance.GameRules.Days;

        if (_currentDay != day)
        {
            CheckIfCheckpoint(day);
            _currentDay = day;
        }
    }

    private void CheckIfCheckpoint(int currentDay)
    {
        foreach (Checkpoint cp in _checkpoints)
        {
            if (currentDay == cp.day)
            {
                if (_cumulativeUpgrades && _currentCheckpoint != null)
                    cp.upgrades += _currentCheckpoint.upgrades;
                _currentCheckpoint = cp;
                StartCheckpoint();
                break;
            }
        }
    }

    private bool Upgrade(IUpgradeable upgradeable)
    {
        //Debug.Log("Checkpoint is null: " + (_currentCheckpoint == null));
        //Debug.Log("Upgradeable is null: " + (upgradeable == null));
        //Debug.Log(upgradeable);

        if ((_currentCheckpoint != null && _currentCheckpoint.upgrades <= 0) ||
            (upgradeable != null && upgradeable.CurrentUpgradeLevel >= upgradeable.MaxUpgradeLevel)) return false;

        upgradeable.Upgrade(1);

        if (_currentCheckpoint != null)
        {
            _currentCheckpoint.upgrades--;
            if (onUpgradesChange != null)
                onUpgradesChange(_currentCheckpoint.upgrades);
        }

        return true;
    }

    public void StartCheckpoint()
    {
        _isCheckpoint = true;
        GameManager.Instance.PauseList();
        OnCheckpointStart.Invoke();
        if (onCheckpointStart != null)
            onCheckpointStart.Invoke();
    }

    public void EndCheckpoint()
    {
        //_currentCheckpoint = null;

        _isCheckpoint = false;
        GameManager.Instance.UnpauseList();
        OnCheckpointEnd.Invoke();
        if (onCheckpointEnd != null)
            onCheckpointEnd.Invoke();
    }
}
