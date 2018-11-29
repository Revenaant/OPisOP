using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycleUI : MonoBehaviour
{
    [SerializeField]
    private bool _timeIsHourMinute = false;

    [SerializeField]
    private Transform _sunMoonPivot = null;

    [SerializeField]
    private Text _timeText = null;

    [SerializeField]
    private Text _dayText = null;

    [SerializeField]
    private string _dayTextPrefix = "Dag ";

    // Use this for initialization
    private void Start()
    {
        if (_timeIsHourMinute)
            DayNightCycle.onHourMinuteChange += UpdateTime;
        else
            DayNightCycle.onTimeLeftChange += UpdateTimeLeft;
        DayNightCycle.onRotationUpdate += UpdateRotation;
        GameRules.onDayChange += UpdateDay;
    }

    private void UpdateRotation(float angle)
    {
        //angle += 180;
        if (_sunMoonPivot != null)
            _sunMoonPivot.transform.localRotation = Quaternion.AngleAxis(angle - 90, Vector3.back);
    }

    private void UpdateDay(int day)
    {
        if (_dayText != null)
            _dayText.text = _dayTextPrefix + day;
    }

    private void UpdateTime(int hour, int minute)
    {
        if (_timeText != null)
            _timeText.text = hour + ":" + (minute / 10 > 0 ? "" : "0") + minute;
    }

    private void UpdateTimeLeft(int secondsLeft)
    {
        if (_timeText != null)
        {
            int minutes = secondsLeft / 60;
            int seconds = secondsLeft % 60;

            _timeText.text = minutes + ":" + (seconds / 10 > 0 ? "" : "0") + seconds;
        }
    }
}
