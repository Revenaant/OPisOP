using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class StormNotification : Notification
{
    private Storms _storms = null;
    private Storms Storms
    {
        get
        {
            if (_storms == null &&
                GameManager.Instance != null &&
                GameManager.Instance.Storms != null)
                _storms = GameManager.Instance.Storms;
            return _storms;
        }
    }

    private DayNightCycle _dayNightCycle = null;
    private DayNightCycle DayNightCycle
    {
        get
        {
            if (_dayNightCycle == null &&
                GameManager.Instance != null &&
                GameManager.Instance.DayNightCycle != null)
                _dayNightCycle = GameManager.Instance.DayNightCycle;
            return _dayNightCycle;
        }
    }

    //public enum TriggerType { SecondsBeforeStorm, HourOfDay }
    //public TriggerType trigger = TriggerType.SecondsBeforeStorm;
    [SerializeField, Range(0, 23)] private int _timeOfDay = 13;

    public override void OnValidate()
    {
        base.OnValidate();
        attemptToFindFocusAutomatically = false;
    }

    public override void Start()
    {
        base.Start();
        if (DayNightCycle != null)
            DayNightCycle.AddEvent(_timeOfDay, Trigger);
        if (Storms != null)
            Storms.onStormEnd.AddListener(End);
    }

    public override void FindFocus()
    {
        throw new InvalidOperationException("Storm Notification should not have a focus");
    }

    private void Trigger()
    {
        if (Time.time <= cooldownTimestamp + cooldownInterval) return;

        //Debug.Log("Notification: Impending Storm!");
        if (onTrigger != null) onTrigger();
    }

    private void End()
    {
        if (onEnd != null) onEnd();
    }
}
