using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Notification
{
    //[Header("Base Notification")]
    protected float cooldownTimestamp = float.PositiveInfinity;
    public bool attemptToFindFocusAutomatically = true;

    //public bool coolingDown = false;
    public float cooldownInterval = 10;
    //public Transform focus = null;
    public TargetFocus focus = null;
    public GameObject prefabUI = null;

    //[Header("Rest of it")]

    public Action onTrigger;
    public Action onEnd;

    public virtual void OnValidate()
    {
        cooldownTimestamp = -cooldownInterval;
    }

    public virtual void Start()
    {
        if (attemptToFindFocusAutomatically && focus == null) FindFocus();
    }

    public abstract void FindFocus();

    public void TriggerCooldown()
    {
        cooldownTimestamp = Time.time;
    }

    public override string ToString()
    {
        return GetType().ToString();
    }
}
