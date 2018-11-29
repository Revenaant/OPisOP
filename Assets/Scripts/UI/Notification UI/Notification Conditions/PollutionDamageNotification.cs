using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PollutionDamageNotification : Notification
{
    private bool _hasEnded = true;

    [SerializeField, Range(0, 1)]
    private float _triggerOverPercent = 0.25f;

    public float TriggerOverPercent
    {
        get { return _triggerOverPercent; }
        set { _triggerOverPercent = value; }
    }

    public override void Start()
    {
        base.Start();
        PollutionManager.onPollutionDamageChange += Trigger;
    }

    public override void FindFocus()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlantBed == null) return;
        focus = GameManager.Instance.PlantBed.GetComponentInChildren<TargetFocus>();
        //focus = GameManager.Instance.PlantBed.transform;
    }

    private void Trigger(float damage)
    {
        bool notDamaged = damage < _triggerOverPercent;
        if (!_hasEnded && notDamaged)
        {
            if (onEnd != null) onEnd();
            _hasEnded = true;
        }

        if (Time.time <= cooldownTimestamp + cooldownInterval) return;
        if (notDamaged) return;

        //Debug.Log("Notification: Too Much Pollution!");
        if (onTrigger != null) onTrigger();
        _hasEnded = false;
    }
}
