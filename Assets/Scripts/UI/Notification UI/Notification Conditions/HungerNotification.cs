using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HungerNotification : Notification
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
        Pet.onHungerChanged += Trigger100;
    }

    public override void FindFocus()
    {
        //if (GameManager.Instance == null || GameManager.Instance.Pet == null) return;
        //if (GameManager.Instance == null || GameManager.Instance.FoodMachine == null) return;
        if (GameManager.Instance == null || GameManager.Instance.NewFoodMachine == null) return;
        focus = GameManager.Instance.NewFoodMachine.GetComponentInChildren<TargetFocus>();
        //focus = GameManager.Instance.FoodMachine.GetComponentInChildren<TargetFocus>();
        //focus = GameManager.Instance.Pet.transform;
    }

    private void Trigger100(float hunger)
    {
        Trigger(hunger / 100);
    }

    private void Trigger(float hunger)
    {
        bool notHungry = hunger < _triggerOverPercent;
        if (!_hasEnded && notHungry)
        {
            if (onEnd != null) onEnd();
            _hasEnded = true;
        }
        if (Time.time <= cooldownTimestamp + cooldownInterval) return;
        if (hunger < _triggerOverPercent) return;

        //Debug.Log("Notification: Rabbit is Hungry!");
        if (onTrigger != null) onTrigger();
        _hasEnded = false;
    }
}
