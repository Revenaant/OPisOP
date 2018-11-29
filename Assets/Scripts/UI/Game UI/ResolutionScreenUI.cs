using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionScreenUI : MonoBehaviour
{
    [SerializeField] private ScoreUI3 _score = null;

    private void OnEnable()
    {
        UpdateScore();
    }

    // Update is called once per frame
    private void UpdateScore()
    {
        if (_score == null || GameManager.Instance.Score == null) return;

        int score = GameManager.Instance.Score.points;
        _score.SetTarget(score);
    }
}
