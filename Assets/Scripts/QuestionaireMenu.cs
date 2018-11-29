using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class QuestionaireMenu : MonoBehaviour
{
    private Questions _questions = null;
    private string _filename = "questionaire.json";

    [SerializeField] private Text _amountOfPeople = null;
    [SerializeField] private RectTransform _q1_filler = null; 
    [SerializeField] private RectTransform _q2_filler = null;
    [SerializeField] private Text _q1_amount = null;
    [SerializeField] private Text _q2_amount = null;
    [SerializeField] private float _maxWidth = 329;
    [SerializeField] private int _jumps = 5;

    private float _jumpWidth = 0;

    // Use this for initialization
    private void Start()
    {
        _jumpWidth = _maxWidth / _jumps;
        _questions = Questionaire.ReadFromFile(_filename) ?? new Questions();

        if (_questions == null) return;

        int count = _questions.questions.Count;

        if (_amountOfPeople != null) _amountOfPeople.text = count.ToString();

        float averageQ1 = 0;
        float averageQ2 = 0;
        foreach (Question question in _questions.questions)
        {
            averageQ1 += question.question1;
            averageQ2 += question.question2;
        }

        averageQ1 /= count;
        averageQ2 /= count;

        float height = _q1_filler.sizeDelta.y;
        if (_q1_filler != null) _q1_filler.sizeDelta = new Vector2(averageQ1 * _jumpWidth, height);
        if (_q2_filler != null) _q2_filler.sizeDelta = new Vector2(averageQ2 * _jumpWidth, height);
        
        if (_q1_amount != null) _q1_amount.text = (averageQ1).ToString("0.#", CultureInfo.InvariantCulture);
        if (_q2_amount != null) _q2_amount.text = (averageQ2).ToString("0.#", CultureInfo.InvariantCulture);
        ;
    }
}
