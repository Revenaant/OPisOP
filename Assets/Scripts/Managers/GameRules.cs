using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class GameRules : MonoBehaviour, IHaveEvents
{
    private StaticFunctions _staticFunctions = null;
    private bool _dirty = true;
    private bool _calledOnEnd = false;

    [Header("Restart on Idle too long")]

    [SerializeField]
    private float _maxIdleTime = 30;

    [SerializeField, ReadOnly]
    private float _idleTime = 0;

    [Header("Happiness")]
    [SerializeField, Range(0, 100)] private float _minimumHappinessDeath = 0;
    [SerializeField] private float _scoreModifierHappyTime = 100;

    [Header("Days")]

    [SerializeField] private int _days = 0;
    [SerializeField] private int _maxDays = 10;


    [Header("End Conditions")]
    [SerializeField] private bool _canEnd = false;
    [SerializeField] private bool _hasEnded = false;

    public static System.Action onEnd;
    public UnityEvent OnEnd;

    public static System.Action<int> onDayChange;
    public static System.Action<int> onScoreChange;

    private StaticFunctions StaticFunctions
    {
        get
        {
            if (_staticFunctions != null) return _staticFunctions;

            _staticFunctions = FindObjectOfType<StaticFunctions>();
            if (_staticFunctions == null)
                Debug.LogWarning("Cannot find StaticFunctions Component in scene.");

            return _staticFunctions;
        }
    }

    public int Days
    {
        get { return _days; }
        set
        {
            _days = value;
            _dirty = true;
            if (onDayChange != null)
                onDayChange(_days);
        }
    }

    public bool CanEnd
    {
        get { return _canEnd; }
        set { _canEnd = value; }
    }

    public bool HasEnded
    {
        get { return _hasEnded; }
        set { _hasEnded = value && CanEnd; }
    }
    //private bool _started;
    private void OnValidate()
    {
        _dirty = true;
    }

    // Use this for initialization
    private void Start()
    {
        HasEnded = false;
        GameManager.Instance.GameRules = this;

        onDayChange += day => DayHasEnded();
    }

    public void ForceEvents()
    {
        if (Application.isPlaying && onDayChange != null)
            onDayChange(_days);
        if (Application.isPlaying && onScoreChange != null)
            onScoreChange(GameManager.Instance.Score.points);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && StaticFunctions != null) StaticFunctions.RestartGame();

        //if (Input.GetKeyDown(KeyCode.R)) GameManager.RestartGame();
        if (_idleTime > _maxIdleTime && StaticFunctions != null) StaticFunctions.RestartGame();
        if (!_dirty || HasEnded) return;
        //if (_ended) return;

        CheckIfItShouldEnd();

        if (!HasEnded || _calledOnEnd) return;

        ForceEnd();
    }

    public void ForceEnd()
    {
        Debug.Log("THE GAME HAS ENDED.");
        if (onEnd != null) onEnd();
        OnEnd.Invoke();
        _calledOnEnd = true;
    }

    private void LateUpdate()
    {
        //_idleTime += Time.deltaTime;
    }

    private void CheckIfItShouldEnd()
    {
        if (_days >= _maxDays)
            HasEnded = true;

        if (GameManager.Instance.Pet == null) return;

        if (GameManager.Instance.Pet.Happiness < _minimumHappinessDeath)
            HasEnded = true;

        _dirty = false;
    }

    private void DayHasEnded()
    {
        if (GameManager.Instance.Score == null || GameManager.Instance.Pet == null) return;

        GameManager.Instance.Score.points += (int)(GameManager.Instance.Pet.HappyTime * _scoreModifierHappyTime);
        if (GameManager.Instance.CheckpointManager != null)
            GameManager.Instance.Score.checkpoints = GameManager.Instance.CheckpointManager.CheckpointsPassed;
        GameManager.Instance.Pet.HappyTime = 0;
        if (onScoreChange != null)
            onScoreChange(GameManager.Instance.Score.points);
    }

    public void End()
    {
        HasEnded = true;
        if (onEnd != null) onEnd();
    }
}
