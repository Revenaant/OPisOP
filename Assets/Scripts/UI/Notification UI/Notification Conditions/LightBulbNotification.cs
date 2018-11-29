using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LightBulbNotification : Notification
{
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

    public override void Start()
    {
        base.Start();
        if (DayNightCycle != null)
        {
            DayNightCycle.onDusk.AddListener(Trigger);
            DayNightCycle.onDawn.AddListener(End);
        }
    }

    public override void FindFocus()
    {
        if (GameManager.Instance == null || GameManager.Instance.LightSwitch == null) return;
        focus = GameManager.Instance.LightSwitch.GetComponentInChildren<TargetFocus>();
        //focus = GameManager.Instance.LightSwitch.transform;
    }

    private void Trigger()
    {
        if (Time.time <= cooldownTimestamp + cooldownInterval) return;
        //Debug.Log("Notification: It's night! Turn on the Lights!");
        if (onTrigger != null) onTrigger();
    }

    private void End()
    {;
        if (onEnd != null) onEnd();
    }
}
