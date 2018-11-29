using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableThingsBeforeDayTwo : MonoBehaviour
{

    private ScoreUI3 _score;
    private NotificationsConditionManager _notifications;
    private bool _showStuff;

    // Use this for initialization
    void Start()
    {
        _notifications = FindObjectOfType<NotificationsConditionManager>();
        _score = FindObjectOfType<ScoreUI3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameRules == null) return;

        if (GameManager.Instance.GameRules.Days == 1)
        {
            _notifications.gameObject.SetActive(false);
            _score.gameObject.SetActive(false);
        }
        else if (GameManager.Instance.GameRules.Days != 1 && !_showStuff)
        {
            _notifications.gameObject.SetActive(true);
            _score.gameObject.SetActive(true);
            _showStuff = true;
            StartCoroutine(MyCoroutines.WaitOneFrame(() =>
                GameManager.Instance.GameRules.ForceEvents()));
        }
    }
}
