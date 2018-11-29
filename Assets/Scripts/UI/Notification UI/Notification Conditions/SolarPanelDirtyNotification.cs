using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SolarPanelDirtyNotification : Notification
{
    private bool _hasEnded = true;

    [SerializeField, Range(0, 1)]
    private float _triggerOverPercent = 0.5f;

    public float TriggerOverPercent
    {
        get { return _triggerOverPercent; }
        set { _triggerOverPercent = value; }
    }

    public override void Start()
    {
        base.Start();
        //if (SolarPanel.onDirtChange != null)
        SolarPanel.onDirtChange += Trigger;
        GameManager.Instance.SolarPanel.OnFullyCleaned += End;
    }

    public override void FindFocus()
    {
        if (GameManager.Instance == null || GameManager.Instance.SolarPanel == null) return;
        focus = GameManager.Instance.SolarPanel.GetComponentInChildren<TargetFocus>();
        //focus = GameManager.Instance.SolarPanel.transform;
    }

    private void Trigger(float dirt)
    {
        if (Time.time <= cooldownTimestamp + cooldownInterval) return;
        if (dirt < _triggerOverPercent) return;

        //Debug.Log("Notification: Solar Panel is Dirty!");
        if (onTrigger != null) onTrigger();
        _hasEnded = false;
    }

    private void End()
    {
        if (_hasEnded) return;
        if (onEnd != null) onEnd();
        _hasEnded = true;
    }
}
