using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BatteryOverchargeNotification : Notification
{
    private bool _hasEnded = true;
    private int _capacity = 0;
    private int _energy = 0;

    [SerializeField, Range(0, 1)]
    private float _triggerOverPercent = 0.9f;

    public float TriggerOverPercent
    {
        get { return _triggerOverPercent; }
        set { _triggerOverPercent = value; }
    }

    public override void Start()
    {
        base.Start();
        //if (Storage.onCapacityChange != null)
        Storage.onCapacityChange += UpdateCapacity;
        //if (Storage.onEnergyChange != null)
        Storage.onEnergyChange += UpdateEnergy;
    }

    public override void FindFocus()
    {
        if (GameManager.Instance == null || GameManager.Instance.Storage == null) return;
        focus = GameManager.Instance.Storage.GetComponentInChildren<TargetFocus>();
        //focus = GameManager.Instance.Storage.transform;
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
        bool inCooldown = Time.time <= cooldownTimestamp + cooldownInterval;
        if (inCooldown) return;

        if (_energy / (float)_capacity < _triggerOverPercent) return;

        /*if ()*/
        _hasEnded = false;
        //Debug.Log("Notification: Battery is Overcharging!");
        if (onTrigger != null) onTrigger();
    }

    private void End()
    {
        if (_energy / (float)_capacity >= _triggerOverPercent) return;

        if (_hasEnded || onEnd == null) return;

        onEnd();
        _hasEnded = true;
    }
}
