using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Scores
{
    public JsonDateTime date;
    public List<Score> scores;

    public Scores(JsonDateTime date, List<Score> list)
    {
        this.date = date;
        scores = list;
    }

    public Scores(Leaderboard leaderboard) : this(leaderboard.date, leaderboard.scores)
    {

    }
}

public class Leaderboard
{
    public DateTime date;
    public List<Score> scores;
    public string filename;

    protected void Sort()
    {
        scores.Sort();
        scores.Reverse();
    }

    public Leaderboard(string filename)
    {
        UnitySystemConsoleRedirector.Redirect();
        this.filename = filename;

        ReadFromFile();
    }

    public void New()
    {
        scores = new List<Score>();
        date = DateTime.Today;
    }

    public Score Find(string name)
    {
        return scores.Find(localScore => localScore.name == name);
    }

    public void Add(Score score)
    {
        if (scores.Find(localScore => localScore == score) != null &&
            scores.Find(localScore => localScore.name == score.name) != null) return;

        scores.Add(score);
        Sort();
    }

    public bool Remove(Score score)
    {
        if (scores.Find(localScore => localScore == score) == null) return false;

        bool result = scores.Remove(score);
        if (result) Sort();

        return result;
    }

    public bool Remove(string name)
    {
        Score score = scores.Find(localScore => localScore.name == name);
        if (score == null) return false;

        bool result = scores.Remove(score);
        if (result) Sort();

        return result;
    }

    public void Clear()
    {
        scores.Clear();
    }

    public void ReadFromFile()
    {
        var scoresObject = ReadFromFile(filename);

        if (scoresObject != null)
        {
            scores = scoresObject.scores;
            date = scoresObject.date;
        }
        else
        {
            New();
        }
    }

    public void SaveToFile()
    {
        SaveToFile(this);
    }

    #region JSON

    public static Scores ReadFromFile(string filename)
    {
        List<Score> scores = new List<Score>();

        Directory.CreateDirectory(Application.streamingAssetsPath);

        //Path.Combine combines strings into a file path
        //Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            Scores loadedData = JsonUtility.FromJson<Scores>(dataAsJson);

            if (loadedData != null) scores = loadedData.scores;

            foreach (Score score in scores)
                score.DeserealizeTime();

            return loadedData;
        }

        using (FileStream fs = File.Create(filePath)) { }

        return null;
    }

    public static void SaveToFile(Leaderboard leaderboard, string filename)
    {
        string dataAsJson = JsonUtility.ToJson(new Scores(leaderboard), true);

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        File.WriteAllText(filePath, dataAsJson);
    }

    public static void SaveToFile(Leaderboard leaderboard)
    {
        string dataAsJson = JsonUtility.ToJson(new Scores(leaderboard), true);

        string filePath = Path.Combine(Application.streamingAssetsPath, leaderboard.filename);
        File.WriteAllText(filePath, dataAsJson);
    }
    #endregion

    #region XML 
    //public static List<Score> ReadFromFile(string filename)
    //{
    //    XmlSerializer xs = new XmlSerializer(typeof(List<Score>));

    //    TextReader tr = new StreamReader(filename);
    //    var data = xs.Deserialize(tr) as List<Score>;
    //    tr.Close();

    //    return data;
    //}

    //public static void SaveToFile(Leaderboard leaderboard, string filename)
    //{
    //    XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));
    //    using (TextWriter writer = new StreamWriter(filename))
    //    {
    //        serializer.Serialize(writer, leaderboard.scores);
    //    }
    //}

    //public static void SaveToFile(Leaderboard leaderboard)
    //{
    //    XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));
    //    using (TextWriter writer = new StreamWriter(leaderboard.filename))
    //    {
    //        serializer.Serialize(writer, leaderboard.scores);
    //    }
    //}
    #endregion
}
