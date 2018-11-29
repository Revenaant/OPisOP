using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Question
{
    public string name = "";
    public int question1 = 0;
    public int question2 = 0;

    public override string ToString()
    {
        return string.Format("Questionairre: {0}, Q1: {1}, Q2: {2}", name, question1, question2);
    }
}

[System.Serializable]
public class Questions
{
    public List<Question> questions = new List<Question>();
}

public class Questionaire : MonoBehaviour
{
    [SerializeField] private string _filename = "questionaire.json";

    [SerializeField]
    private Slider _sliderQ1 = null;
    [SerializeField]
    private Slider _sliderQ2 = null;

    private Questions _questions = null;

    private void Start()
    {
        _questions = ReadFromFile(_filename) ?? new Questions();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
    //        string text = "";
    //        foreach (Question question in _questions.questions)
    //        {
    //            text += question.ToString() + "\n";
    //        }
    //        Debug.Log(text);
    //    }
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        _questions = ReadFromFile(_filename);
    //    }
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        Final();
    //    }
    //}

    public void Final()
    {
        _questions.questions.Add(new Question()
        { 
            name = GameManager.Instance.Score.name,
            question1 = (int)_sliderQ1.value,
            question2 =  (int)_sliderQ2.value
        });

        SaveToFile(_questions, _filename);
    }

    #region JSON

    public static Questions ReadFromFile(string filename)
    {
        Directory.CreateDirectory(Application.streamingAssetsPath);

        //Path.Combine combines strings into a file path
        //Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, filename);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            Questions loadedData = JsonUtility.FromJson<Questions>(dataAsJson);
            return loadedData;
        }

        using (FileStream fs = File.Create(filePath)) { }

        return new Questions();
    }

    public static void SaveToFile(Questions questions, string filename)
    {
        string dataAsJson = JsonUtility.ToJson(questions, true);

        string filePath = Path.Combine(Application.streamingAssetsPath, filename);
        File.WriteAllText(filePath, dataAsJson);
    }
    #endregion
}
