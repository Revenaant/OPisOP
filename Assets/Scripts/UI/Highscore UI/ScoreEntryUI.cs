using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public enum LeaderboardType { Daily, Monthly, Yearly, AllTime }

public class ScoreEntryUI : MonoBehaviour
{
    private static readonly CultureInfo cultureInfo = new CultureInfo("nl-NL");
    public static LeaderboardType leaderboardType = LeaderboardType.Daily;

    //[SerializeField] private LeaderboardType _leaderboardType = LeaderboardType.Daily;
    //private List<ImageColorSwapper> _stars = new List<ImageColorSwapper>();
    private ImageColorSwapper[] _stars = new ImageColorSwapper[3];

    [Header("Checkpoints")]
    [SerializeField] private ImageColorSwapper _star1 = null;
    [SerializeField] private ImageColorSwapper _star2 = null;
    [SerializeField] private ImageColorSwapper _star3 = null;

    [Header("Score")]
    [SerializeField] private Sprite _sadFaceScoreSprite = null;
    [SerializeField] private Sprite _neutralFaceScoreSprite = null;
    [SerializeField] private Sprite _happyFaceScoreSprite = null;
    [SerializeField, MinMax(0, 10000)] private MinMaxPair _scoreFaceRange = new MinMaxPair(1000, 5000);

    [Header("Refences")]
    [SerializeField] private Image _faceImage = null;
    [SerializeField] private Text _nameText = null;
    [SerializeField] private Text _scoreText = null;
    [SerializeField] private Text _timeText = null;

    private void Start()
    {
        //_stars.Add(_star1);
        //_stars.Add(_star2);
        //_stars.Add(_star3);

        _stars[0] = _star1;
        _stars[1] = _star2;
        _stars[2] = _star3;
    }

    public void UpdateInformation(Score score)
    {
        UpdateText(score);
        UpdateTime(score.timestamp);
        UpdateImage(score.points);
        UpdateCheckpoints(score.checkpoints);
        //Debug.Log(score);
    }

    private void UpdateText(Score score)
    {
        if (_nameText == null || _scoreText == null) return;

        _nameText.text = score.name;
        _scoreText.text = score.points.ToString();
    }

    private void UpdateCheckpoints(int checkpoints)
    {
        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
        {
            for (int i = 1; i <= 3; i++)
            {
                ImageColorSwapper icw = _stars[i - 1];
                if (icw == null) continue;

                //Debug.Log("Not null motherfucker");
                if (checkpoints >= i) icw.Color1();
                else icw.Color2();
            }
        }));
    }

    private void UpdateImage(int points)
    {
        if (_faceImage == null || _sadFaceScoreSprite == null ||
            _happyFaceScoreSprite == null || _neutralFaceScoreSprite == null) return;

        MinMaxPair.Position positionInRange = _scoreFaceRange.Evaluate(points);

        switch (positionInRange)
        {
            default:
            case MinMaxPair.Position.Invalid:
                Debug.Log("Invalid Score Thing!");
                break;
            case MinMaxPair.Position.Under:
                _faceImage.sprite = _sadFaceScoreSprite;
                break;
            case MinMaxPair.Position.InRange:
                _faceImage.sprite = _neutralFaceScoreSprite;
                break;
            case MinMaxPair.Position.Over:
                _faceImage.sprite = _happyFaceScoreSprite;
                break;
        }
    }

    private void UpdateTime(DateTime timestamp)
    {
        if (_timeText == null) return;

        string text;

        switch (leaderboardType)
        {
            case LeaderboardType.Daily:
                text = timestamp.ToString("t", cultureInfo);
                break;
            case LeaderboardType.Monthly:
            case LeaderboardType.Yearly:
                text = timestamp.ToString("M", cultureInfo);
                break;
            case LeaderboardType.AllTime:
                text = timestamp.ToString("d", cultureInfo);
                break;
            default:
                text = timestamp.ToString("d", cultureInfo);
                break;
        }

        _timeText.text = text;
    }
}
