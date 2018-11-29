using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//[Serializable]
//public class Date
//{
//    public DateTime date;
//    public JsonDateTime jsonDate;

//    public void DateToJson()
//    {
//        jsonDate = date;
//    }

//    public void JsonToDate()
//    {
//        date = jsonDate;
//    }
//}

//[Serializable]
//public class Dates
//{
//    private string _filename = "dates.json";

//    public Date day = new Date();
//    public Date month = new Date();
//    public Date year = new Date();

//    public void DateToJson()
//    {
//        day.DateToJson();
//        month.DateToJson();
//        year.DateToJson();
//    }

//    public void JsonToDate()
//    {
//        day.JsonToDate();
//        month.JsonToDate();
//        year.JsonToDate();
//    }

//    public void Load()
//    {
//        Directory.CreateDirectory(Application.streamingAssetsPath);

//        //Path.Combine combines strings into a file path
//        //Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
//        string filePath = Path.Combine(Application.streamingAssetsPath, _filename);

//        if (File.Exists(filePath))
//        {
//            string dataAsJson = File.ReadAllText(filePath);
//            Dates loadedData = JsonUtility.FromJson<Dates>(dataAsJson);

//            if (loadedData == null) return;

//            day.jsonDate = loadedData.day.jsonDate;
//            month.jsonDate = loadedData.month.jsonDate;
//            year.jsonDate = loadedData.year.jsonDate;

//            JsonToDate();
//        }
//        else using (FileStream fs = File.Create(filePath)) { }
//    }

//    public void Save()
//    {
//        string dataAsJson = JsonUtility.ToJson(this, true);

//        string filePath = Path.Combine(Application.streamingAssetsPath, _filename);
//        File.WriteAllText(filePath, dataAsJson);
//    }
//}

public class LeaderboardManager
{
    private List<Leaderboard> _leaderboards = new List<Leaderboard>();
    //private Dates _dates = new Dates();

    public LeaderboardManager()
    {
        //const string folder = "leaderboards/";
        //const string folder = "";

        //DailyLeaderboard = new Leaderboard(folder + "daily.xml");
        //MonthlyLeaderboard = new Leaderboard(folder + "mothly.xml");
        //YearlyLeaderboard = new Leaderboard(folder + "yearly.xml");
        //AllTimeLeaderboard = new Leaderboard(folder + "overall.xml");

        //_dates.Load();

        DailyLeaderboard = new Leaderboard("daily.json");
        MonthlyLeaderboard = new Leaderboard("monthly.json");
        YearlyLeaderboard = new Leaderboard("yearly.json");
        AllTimeLeaderboard = new Leaderboard("overall.json");

        //string dateString = "Dates: ";
        //dateString += "\n Day: " + _dates.day.date; 
        //dateString += "\n Month: " + _dates.month.date; 
        //dateString += "\n Year: " + _dates.year.date; 
        //Debug.Log(dateString);

        //Debug.Log("Daily: " + DailyLeaderboard.date);
        //Debug.Log("Month: " + MonthlyLeaderboard.date);
        //Debug.Log("Year: " + YearlyLeaderboard.date);
        //Debug.Log("All Time: " + AllTimeLeaderboard.date);

        DateTime today = DateTime.Today;

        if (!DailyLeaderboard.date.Equals(today))
        {
            Debug.Log("New Day.");
            DailyLeaderboard.New();
            DailyLeaderboard.SaveToFile();
            //_dates.day.date = DateTime.Today;
        }

        if (MonthlyLeaderboard.date.Month != today.Month
            || MonthlyLeaderboard.date.Year != today.Year)
        {
            Debug.Log("New Month.");
            MonthlyLeaderboard.New();
            MonthlyLeaderboard.SaveToFile();
            //_dates.month.date = DateTime.Today;
        }

        if (YearlyLeaderboard.date.Year != today.Year)
        {
            Debug.Log("New Year.");
            YearlyLeaderboard.New();
            YearlyLeaderboard.SaveToFile();
            //_dates.year.date = DateTime.Today;
        }

        //_dates.DateToJson();
        //_dates.Save();

        _leaderboards.Add(DailyLeaderboard);
        _leaderboards.Add(MonthlyLeaderboard);
        _leaderboards.Add(YearlyLeaderboard);
        _leaderboards.Add(AllTimeLeaderboard);
    }

    public void Add(Score score)
    {
        foreach (Leaderboard leaderboard in _leaderboards)
            leaderboard.Add(score);
    }

    public void RemoveScore(Score score)
    {
        foreach (Leaderboard leaderboard in _leaderboards)
            leaderboard.Remove(score);
    }

    public void RemoveScore(string name)
    {
        foreach (Leaderboard leaderboard in _leaderboards)
            leaderboard.Remove(name);
    }

    public void FinalizeAll()
    {
        foreach (Leaderboard leaderboard in _leaderboards)
        {
            foreach (Score score in leaderboard.scores)
            {
                score.Final();
            }
        }
    }

    public void PrintTimeAll()
    {

        foreach (Leaderboard leaderboard in _leaderboards)
        {
            foreach (Score score in leaderboard.scores)
            {
                Debug.Log(score.timestamp.ToShortTimeString());
            }
        }
    }

    public void ClearAll()
    {
        foreach (Leaderboard leaderboard in _leaderboards)
            leaderboard.Clear();
    }

    public void SaveAll()
    {
        foreach (Leaderboard leaderboard in _leaderboards)
            leaderboard.SaveToFile();
    }

    //public void SaveFinalizeAll()
    //{
    //    foreach (Leaderboard leaderboard in _leaderboards)
    //    {
    //        //leaderboard.
    //        leaderboard.SaveToFile();
    //    }
    //}


    public Leaderboard DailyLeaderboard { get; private set; }
    public Leaderboard MonthlyLeaderboard { get; private set; }
    public Leaderboard YearlyLeaderboard { get; private set; }
    public Leaderboard AllTimeLeaderboard { get; private set; }
}
