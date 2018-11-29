using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class LeaderboardUI : MonoBehaviour
{
    private Leaderboard _currentLeaderboard = null;

    [SerializeField] private RectTransform _content = null;
    [SerializeField] private SimpleObjectPool _simpleObjectPool = null;

    [SerializeField] private int _maxEntriesDaily = 20;
    [SerializeField] private int _maxEntriesMonthly = 10;
    [SerializeField] private int _maxEntriesYearly = 10;
    [SerializeField] private int _maxEntriesAllTime = 10;

    [SerializeField] private ImageColorSwapper _imageDaily = null;
    [SerializeField] private ImageColorSwapper _imageMonthly = null;
    [SerializeField] private ImageColorSwapper _imageYearly = null;
    [SerializeField] private ImageColorSwapper _imageAllTime = null;

    private void OnEnable()
    {
        if (_currentLeaderboard == null) SwitchToDaily(true);
    }
    
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.U))
        //    GameManager.Instance.LeaderboardManager.Add(new Score());
        //if (Input.GetKeyDown(KeyCode.O))
        //    GameManager.Instance.LeaderboardManager.ClearAll();
        //if (Input.GetKeyDown(KeyCode.I))
        //    GameManager.Instance.LeaderboardManager.SaveAll();
        //if (Input.GetKeyDown(KeyCode.P))
        //    GameManager.Instance.LeaderboardManager.FinalizeAll();
        //if (Input.GetKeyDown(KeyCode.LeftBracket))
        //    GameManager.Instance.LeaderboardManager.PrintTimeAll();
    }

    public void SwitchToDaily(bool value)
    {
        //Debug.Log("Daily Leaderboard");
        _currentLeaderboard = GameManager.Instance.LeaderboardManager.DailyLeaderboard;
        ScoreEntryUI.leaderboardType = LeaderboardType.Daily;
        UpdateEntries();
        UpdateImages();
    }

    public void SwitchToMonthly(bool value)
    {
        //Debug.Log("Monthly Leaderboard");
        _currentLeaderboard = GameManager.Instance.LeaderboardManager.MonthlyLeaderboard;
        ScoreEntryUI.leaderboardType = LeaderboardType.Monthly;
        UpdateEntries();
        UpdateImages();
    }

    public void SwitchToYearly(bool value)
    {
        //Debug.Log("Yearly Leaderboard");
        _currentLeaderboard = GameManager.Instance.LeaderboardManager.YearlyLeaderboard;
        ScoreEntryUI.leaderboardType = LeaderboardType.Yearly;
        UpdateEntries();
        UpdateImages();
    }

    public void SwitchToAllTime(bool value)
    {
        //Debug.Log("All Time Leaderboard");
        _currentLeaderboard = GameManager.Instance.LeaderboardManager.AllTimeLeaderboard;
        ScoreEntryUI.leaderboardType = LeaderboardType.AllTime;
        UpdateEntries();
        UpdateImages();
    }

    public void UpdateEntries()
    {
        if (_content == null || _simpleObjectPool == null || _currentLeaderboard == null) return;

        int entries = GetEntries();

        //Clear
        //int fuckingUnity = 0;
        //int fuckingUnity = 1;
        //while (_content.childCount > fuckingUnity)
        //{
        //    //if (Input.GetKey(KeyCode.Escape)) break;
        //    GameObject toRemove = transform.GetChild(fuckingUnity).gameObject;
        //    _simpleObjectPool.ReturnObject(toRemove);
        //}

        var pooledObjects = _content.GetComponentsInChildren<PooledObject>();
        for (int i = pooledObjects.Length - 1; i >= 0; i--)
            _simpleObjectPool.ReturnObject(pooledObjects[i].gameObject);

        //Add
        for (int i = 0; i < entries; i++)
        {
            GameObject newEntry = _simpleObjectPool.GetObject();
            //newEntry.transform.SetParent(_content, false);
            newEntry.transform.SetParent(_content, false);
            newEntry.transform.position = _content.transform.position;

            ScoreEntryUI score = newEntry.GetComponent<ScoreEntryUI>();
            score.UpdateInformation(_currentLeaderboard.scores[i]);
        }
    }

    private void UpdateImages()
    {
        switch (ScoreEntryUI.leaderboardType)
        {
            case LeaderboardType.Daily:
                _imageDaily.Color1();
                _imageMonthly.Color2();
                _imageYearly.Color2();
                _imageAllTime.Color2();
                break;
            case LeaderboardType.Monthly:
                _imageDaily.Color2();
                _imageMonthly.Color1();
                _imageYearly.Color2();
                _imageAllTime.Color2();
                break;
            case LeaderboardType.Yearly:
                _imageDaily.Color2();
                _imageMonthly.Color2();
                _imageYearly.Color1();
                _imageAllTime.Color2();
                break;
            case LeaderboardType.AllTime:
                _imageDaily.Color2();
                _imageMonthly.Color2();
                _imageYearly.Color2();
                _imageAllTime.Color1();
                break;
            default:
                _imageDaily.Color2();
                _imageMonthly.Color2();
                _imageYearly.Color2();
                _imageAllTime.Color1();
                break;
        }
    }

    private int GetEntries()
    {
        int entries = 0;

        switch (ScoreEntryUI.leaderboardType)
        {
            case LeaderboardType.Daily:
                entries = _maxEntriesDaily;
                break;
            case LeaderboardType.Monthly:
                entries = _maxEntriesMonthly;
                break;
            case LeaderboardType.Yearly:
                entries = _maxEntriesYearly;
                break;
            case LeaderboardType.AllTime:
                entries = _maxEntriesAllTime;
                break;
            default:
                entries = _maxEntriesYearly;
                break;
        }

        if (entries > _currentLeaderboard.scores.Count)
            entries = _currentLeaderboard.scores.Count;

        return entries;
    }
}
