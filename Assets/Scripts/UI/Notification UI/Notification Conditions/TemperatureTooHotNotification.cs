using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TemperatureTooHotNotification : Notification
{
    private bool _hasEnded = true;

    private Pet _pet = null;
    private Pet Pet
    {
        get
        {
            if (_pet == null &&
                GameManager.Instance != null &&
                GameManager.Instance.Pet != null)
                _pet = GameManager.Instance.Pet;
            return _pet;
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

    [SerializeField] private bool _hasToBeDay = true;

    public override void Start()
    {
        base.Start();
        PollutionManager.onPollutionDamageChange += Trigger;
    }

    public override void FindFocus()
    {
        if (GameManager.Instance == null || GameManager.Instance.HeatingMachine == null) return;
        focus = GameManager.Instance.HeatingMachine.GetComponentInChildren<TargetFocus>();
        //focus = GameManager.Instance.HeatingMachine.transform;
    }

    private void Trigger(float heat)
    {
        bool notTooHot = heat < Pet.TooHotTemperature;
        if (!_hasEnded && notTooHot)
        {
            if (onEnd != null) onEnd();
            _hasEnded = true;
        }

        if (Time.time <= cooldownTimestamp + cooldownInterval) return;

        if (notTooHot) return;

        if (_hasToBeDay)
        {
            if (DayNightCycle.IsDay()) ActuallyTrigger();
        }
        else
        {
            ActuallyTrigger();
        }
    }

    private void ActuallyTrigger()
    {
        //Debug.Log("Notification: Rabbit is Too Hot!");
        if (onTrigger != null) onTrigger();
        _hasEnded = false;
    }
}
