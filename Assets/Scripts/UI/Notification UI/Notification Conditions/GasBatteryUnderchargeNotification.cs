using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GasBatteryUnderchargeNotification : Notification
{
    private bool _hasEnded = true;
    private int _capacity = 0;
    private int _energy = 0;

    [SerializeField, Range(0, 1)]
    private float _triggerUnderPercent = 0.1f;

    public float TriggerUnderPercent
    {
        get { return _triggerUnderPercent; }
        set { _triggerUnderPercent = value; }
    }

    public override void Start()
    {
        base.Start();
        Storage.onCapacityChange += UpdateCapacity;
        Storage.onEnergyChange += UpdateEnergy;
        //Debug.Log("Start");
    }

    public override void FindFocus()
    {
        if (GameManager.Instance == null || GameManager.Instance.Storage == null) return;
        focus = GameManager.Instance.GasTank.GetComponentInChildren<TargetFocus>();
        //focus = GameManager.Instance.Storage.transform;
        //Debug.Log("Find");
    }

    private void UpdateCapacity(int amount)
    {
        _capacity = amount;

        Trigger();
        End();
    }

    private void UpdateEnergy(int amount)
    {
        _energy = amount;

        Trigger();
        End();
    }

    private void Trigger()
    {
        if (Time.time <= cooldownTimestamp + cooldownInterval) return;
        if (_energy / (float)_capacity > _triggerUnderPercent) return;

        //Debug.Log("Notification: Battery is Low! Use the Gas Tank!");
        if (onTrigger != null) onTrigger();
        _hasEnded = false;
    }

    private void End()
    {
        if (_energy / (float)_capacity <= _triggerUnderPercent) return;

        if (_hasEnded || onEnd == null) return;

        onEnd();
        _hasEnded = true;
    }
}
