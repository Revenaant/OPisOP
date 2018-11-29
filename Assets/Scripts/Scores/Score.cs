using System;
using System.Xml.Serialization;

[Serializable]
public enum Difficulty
{
    Easy, Medium, Hard
}

[Serializable]
public struct JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt)
    {
        //UnityEngine.Debug.Log("Converted to time");
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        //UnityEngine.Debug.Log("Converted to JDT");
        JsonDateTime jdt = new JsonDateTime {value = dt.ToFileTimeUtc()};
        return jdt;
    }
}

[Serializable, XmlRoot("Score")]
public class Score : IComparable, IComparable<Score>, IEquatable<Score>
{
    [XmlAttribute("name", typeof(string))]
    public string name;
    [XmlAttribute("points", typeof(int))]
    public int points;
    [XmlAttribute("checkpoints", typeof(Difficulty))]
    public int checkpoints;
    [XmlAttribute("timestamp", typeof(DateTime))]
    public DateTime timestamp;

    public JsonDateTime jdt;

    public Score()
    {
        name = "default";
        points = 0;
        checkpoints = 0;
        Final();
        //difficulty = Difficulty.Medium;
    }

    public Score(string name = "default", int points = 0, int checkpoints = 0, bool now = true)
        //, Difficulty difficulty = Difficulty.Medium)
    {
        this.name = name;
        this.points = points;
        this.checkpoints = checkpoints;
        //this.difficulty = difficulty;
        if (now) Final();
    }

    /// <summary>
    /// Sets the timestamp to now.
    /// </summary>
    public void Final()
    {
        timestamp = DateTime.Now;
        jdt = (JsonDateTime)timestamp;
    }

    public void DeserealizeTime()
    {
        timestamp = (DateTime)jdt;
    }

    public string NiceString()
    {
        return string.Format("Score - Name: {0} - Points: {1} - Time: {2}", name, points, timestamp.ToLongTimeString());
        //return string.Format("Score - Name: {0} - Points: {1} - Difficulty: {2} - Time: {3}", name, points, difficulty.ToString(), timestamp.ToLongTimeString());
    }

    public override string ToString()
    {
        return string.Format("Score - Name: {0} - Points: {1} - Checkpoints: {2} - Time: {3}", name, points, checkpoints, timestamp.ToLongTimeString());
        //return string.Format("Score - Name: {0} - Points: {1} - Difficulty: {2} - Time: {3}", name, points, difficulty.ToString(), timestamp.ToLongTimeString());
    }

    public int CompareTo(object obj)
    {
        Score score = obj as Score;
        if (score != null) return CompareTo(score);

        return -1;
    }

    public int CompareTo(Score other)
    {
        if (other == null) return -1;

        if (points == other.points) return -timestamp.CompareTo(other.timestamp);

        return points.CompareTo(other.points);
    }

    public override bool Equals(object obj)
    {
        Score score = obj as Score;
        if (score == null) return false;
        return points.Equals(score.points);
    }

    public override int GetHashCode()
    {
        return timestamp.GetHashCode();
    }

    public bool Equals(Score other)
    {
        return other != null && points.Equals(other.points);
    }
}
