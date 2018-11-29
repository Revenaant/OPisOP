using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class GameManager : IHaveEvents
{
    #region Events

    public static Action<int> onTemperatureChange;

    public void ForceEvents()
    {
        if (onTemperatureChange != null)
            onTemperatureChange((int)Temperature);
        //iHaveEvents.ForEach(ihe => ihe.ForceEvents());
        foreach (var iHaveEvent in iHaveEvents)
        {
            //Debug.Log(iHaveEvent.ToString());
            iHaveEvent.ForceEvents();
        }
    }
    #endregion

    #region Restarting

    public static void RestartGame()
    {
        _instance.Pause();
        _instance = null;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    #endregion

    #region Pause

    private bool _paused = false;
    public bool IsPaused
    {
        get { return _paused; }
        set
        {
            _paused = value;
            if (value) PauseList();
            else UnpauseList();

            if (CheckpointManager != null && CheckpointManager.IsCheckpoint)
                CheckpointManager.IsCheckpoint = false;
        }
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Unpause()
    {
        IsPaused = false;
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
    }

    #endregion

    #region Stats
    // Region used to store global game stats
    private float _temperature = -1;
    public float Temperature
    {
        get { return _temperature; }
        set
        {
            _temperature = Mathf.Clamp(value, 0, 100);
            if (onTemperatureChange != null)
                onTemperatureChange((int)_temperature);
        }
    }
    public bool DisplayDebug { get; set; }
    private Score _score = null;

    private static int _counter = 1;
    public Score Score
    {
        get { return _score ?? (_score = new Score("Voorbeeld naam " + _counter++, 0)); }
        set { _score = value; }
    }

    private bool _inDome;
    public Action<bool> onDomeChange;
    public bool InDome
    {
        get { return _inDome; }
        set
        {
            _inDome = value;
            if (onDomeChange != null)
                onDomeChange(value);
        }
    }

    #endregion

    #region Lists

    public List<IBreakable> breakables = new List<IBreakable>();
    public List<IPausable> pausables = new List<IPausable>();
    public List<IPush> pushables = new List<IPush>();
    public List<IPull> pullables = new List<IPull>();
    public List<IUpgradeable> upgradeables = new List<IUpgradeable>();
    public List<IHaveEvents> iHaveEvents = new List<IHaveEvents>();

    public void AddToList(MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour == null) return;

        IBreakable breakable = monoBehaviour as IBreakable;
        if (breakable != null)
            breakables.Add(breakable);

        IPausable pausable = monoBehaviour as IPausable;
        if (pausable != null)
            pausables.Add(pausable);

        IPush push = monoBehaviour as IPush;
        if (push != null)
            pushables.Add(push);

        IPull pull = monoBehaviour as IPull;
        if (pull != null)
            pullables.Add(pull);

        IUpgradeable upgradeable = monoBehaviour as IUpgradeable;
        if (upgradeable != null)
            upgradeables.Add(upgradeable);

        IHaveEvents iHaveEvent = monoBehaviour as IHaveEvents;
        if (iHaveEvent != null)
            iHaveEvents.Add(iHaveEvent);
    }

    public void RemoveFromList(MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour == null) return;

        IBreakable breakable = monoBehaviour as IBreakable;
        if (breakable != null)
            breakables.Remove(breakable);

        IPausable pausable = monoBehaviour as IPausable;
        if (pausable != null)
            pausables.Remove(pausable);

        IPush push = monoBehaviour as IPush;
        if (push != null)
            pushables.Remove(push);

        IPull pull = monoBehaviour as IPull;
        if (pull != null)
            pullables.Remove(pull);

        IUpgradeable upgradeable = monoBehaviour as IUpgradeable;
        if (upgradeable != null)
            upgradeables.Remove(upgradeable);

        IHaveEvents iHaveEvent = monoBehaviour as IHaveEvents;
        if (iHaveEvent != null)
            iHaveEvents.Remove(iHaveEvent);
    }

    public void PauseList()
    {
        pausables.ForEach(p => p.IsPaused = true);
    }

    public void UnpauseList()
    {
        pausables.ForEach(p => p.IsPaused = false);
    }

    //private void AssignTo(ref MonoBehaviour reference, MonoBehaviour value)
    //{
    //    RemoveFromList(reference);
    //    reference = value;
    //    AddToList(reference);
    //}

    #endregion

    #region References

    public Camera MainCamera { get; set; }
    public StaticFunctions StaticFunctions { get; set; }
    public UIManager GameUIManager { get; set; }
    //public NotificationUIManager NotificationUIManager { get; set; }

    public static SceneManager SceneManager { get; set; }

    //Machines
    private SolarPanel _solarPanel = null;
    public SolarPanel SolarPanel
    {
        get { return _solarPanel; }
        set
        {
            //AssignTo(ref _solarPanel, value);
            RemoveFromList(_solarPanel);
            _solarPanel = value;
            AddToList(_solarPanel);
        }
    }

    private FoodMachine _foodMachine = null;
    public FoodMachine FoodMachine
    {
        get { return _foodMachine; }
        set
        {
            RemoveFromList(_foodMachine);
            _foodMachine = value;
            AddToList(_foodMachine);
        }
    }

    private HeatingMachine _heatingMachine = null;
    public HeatingMachine HeatingMachine
    {
        get { return _heatingMachine; }
        set
        {
            RemoveFromList(_heatingMachine);
            _heatingMachine = value;
            AddToList(_heatingMachine);
        }
    }

    private GasTank _gasTank = null;
    public GasTank GasTank
    {
        get { return _gasTank; }
        set
        {
            RemoveFromList(_gasTank);
            _gasTank = value;
            AddToList(_gasTank);
        }
    }

    private Storage _storage = null;
    public Storage Storage
    {
        get { return _storage; }
        set
        {
            RemoveFromList(_storage);
            _storage = value;
            AddToList(_storage);
        }
    }

    private PlantBed _plantBed = null;
    public PlantBed PlantBed
    {
        get { return _plantBed; }
        set
        {
            RemoveFromList(_plantBed);
            _plantBed = value;
            AddToList(_plantBed);
        }
    }

    private NewFoodMachine _newFoodMachine = null;
    public NewFoodMachine NewFoodMachine
    {
        get { return _newFoodMachine; }
        set
        {
            //RemoveFromList(_newFoodMachine);
            _newFoodMachine = value;
            //AddToList(_newFoodMachine);
        }
    }

    private LightSwitch _lightSwitch = null;
    public LightSwitch LightSwitch
    {
        get { return _lightSwitch; }
        set
        {
            //RemoveFromList(_lightSwitch);
            _lightSwitch = value;
            //AddToList(_lightSwitch);
        }
    }

    //Managers
    public LeaderboardManager LeaderboardManager { get; set; }
    public CheckpointManager CheckpointManager { get; set; }

    //public PollutionManager PollutionManager { get; set; }

    private PollutionManager _pollutionManager = null;
    public PollutionManager PollutionManager
    {
        get { return _pollutionManager; }
        set
        {
            RemoveFromList(_pollutionManager);
            _pollutionManager = value;
            AddToList(_pollutionManager);
        }
    }

    private GameRules _gameRules = null;
    public GameRules GameRules
    {
        get { return _gameRules; }
        set
        {
            RemoveFromList(_gameRules);
            _gameRules = value;
            AddToList(_gameRules);
        }
    }

    private DayNightCycle _dayNightCycle = null;
    public DayNightCycle DayNightCycle
    {
        get { return _dayNightCycle; }
        set
        {
            RemoveFromList(_dayNightCycle);
            _dayNightCycle = value;
            AddToList(_dayNightCycle);
        }
    }

    private BreakEventSystem _breakEventSystem = null;
    public BreakEventSystem BreakEventSystem
    {
        get { return _breakEventSystem; }
        set
        {
            RemoveFromList(_breakEventSystem);
            _breakEventSystem = value;
            AddToList(_breakEventSystem);
        }
    }

    private Storms _storms = null;
    public Storms Storms
    {
        get { return _storms; }
        set
        {
            RemoveFromList(_storms);
            _storms = value;
            AddToList(_storms);
        }
    }

    //Other
    public Pet Pet { get; set; }

    #endregion

    #region Self
    public static bool isQuitting = false;

    private GameObject _gameObject = null;
    public GameObject GameObject
    {
        get { return _gameObject; }
    }

    // Singleton Reference to this class
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (!Application.isPlaying) return null;
            if (isQuitting) return null;
            if (_instance != null) return _instance;

            _instance = new GameManager();
            Object.DontDestroyOnLoad(_instance._gameObject);
            return _instance;
        }
    }

    public CameraControls CameraControls { get; set; }
    public DummyTutorial DummyTutorial { get; set; }

    public GameManager()
    {
        _gameObject = new GameObject("_gameManager");

        Pet = null;

        //Days = 0;
        Temperature = -1;
        MainCamera = null;
        //DayNightCycle = null;
        //BreakEventSystem = null;
        //PollutionManager = null;
        //Score = null;
        //LeaderboardManager = null;
        CheckpointManager = null;

        //Testing
        LeaderboardManager = new LeaderboardManager();
        //Score = new Score("Player Name", 123);
        //LeaderboardManager.DailyLeaderboard.Add(Score);

        GameRules.onEnd += OnEnd;
    }

    public void OnEnd()
    {
        Score.Final();
        LeaderboardManager.Add(Score);
        LeaderboardManager.SaveAll();
        StaticFunctions.StartCoroutine(MyCoroutines.Wait(2, KillEverything));
    }

    private void KillEverything()
    {
        Object.Destroy(DummyTutorial.gameObject);
        DummyTutorial = null;

        Score = null;
        MainCamera = null;
        StaticFunctions = null;
        GameUIManager = null;
        SolarPanel = null;
        FoodMachine = null;
        HeatingMachine = null;
        GasTank = null;
        Storage = null;
        PlantBed = null;
        NewFoodMachine = null;
        LightSwitch = null;
        LeaderboardManager = null;
        CheckpointManager = null;
        PollutionManager = null;
        GameRules = null;
        DayNightCycle = null;
        Storms = null;
        Pet = null;
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
        Delete();
    }

    private void OnDisable()
    {
        isQuitting = true;
        Delete();
    }

    private void OnDestroy()
    {
        isQuitting = true;
        Delete();
    }

    public static void ApplicationQuit()
    {
        Debug.Log("Quit");

        SceneManager.gameObject.SetActive(false);
        Object.Destroy(SceneManager.gameObject);
        SceneManager = null;

        Instance.OnApplicationQuit();// save any game data here
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void Delete()
    {
        if (_instance._gameObject != null)
        {
            GameObject.Destroy(_instance._gameObject);
            _instance._gameObject = null;
        }

        _instance = null;
        //OnApplicationQuit();
    }
    #endregion
}
