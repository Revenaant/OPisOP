using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public enum FoodMachineState { None, Idle, WaitingForPower, Growing, Harvestable, Harvesting, Harvested }
public class NewFoodMachine : Connectable, IPull/*, IPausable*/
{
    private Animator _anime = null;
    //private FoodMachineLinternal fml = null;

    [Header("States")]
    [SerializeField, ReadOnly] private FoodMachineState _state = FoodMachineState.Idle;

    public FoodMachineState State
    {
        get { return _state; }
        set
        {
            _state = value;
            //Debug.Log(_state.ToString());
            if (onStateChange != null)
                onStateChange(_state);
        }
    }

    [Header("Power and Production")]
    [SerializeField] private float _energyNeed = 20;
    [SerializeField] private int _unitsProduced = 4;

    public float EnergyNeed
    {
        get { return _energyNeed; }
        set { _energyNeed = value; }
    }

    public static Action<FoodMachineState> onStateChange;

    // Use this for initialization
    private void Start()
    {
        NewFoodMachineInternal.Food = 0;
        _anime = GetComponentInChildren<Animator>();
        GameManager.Instance.NewFoodMachine = this;

        //StartCoroutine(MyCoroutines.Wait(0.5f, () => _isOn = true));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Comma))
            NewFoodMachineInternal.Food++;
    }

    //public void Do()
    //{
    //    //Debug.Log("Yo");
    //    switch (State)
    //    {
    //        case FoodMachineState.Idle:
    //            {
    //                WantToPlant();
    //                break;
    //            }
    //        case FoodMachineState.Harvestable:
    //            {
    //                Harvest();
    //                break;
    //            }
    //        default:
    //            {
    //                Debug.Log("Unrecognized FoodMachineState");
    //                break;
    //            }
    //        case FoodMachineState.None:
    //        case FoodMachineState.Growing:
    //        case FoodMachineState.Harvesting:
    //            {
    //                //_anime.SetTrigger("Harvest");
    //                break;
    //            }
    //        case FoodMachineState.WaitingForPower:
    //            {
    //                //State = FoodMachineState.Idle;
    //                break;
    //            }
    //    }
    //}

    public void WantToPlant()
    {
        if (State != FoodMachineState.Idle) return;

        State = FoodMachineState.WaitingForPower;
        RequestEnergy(_energyNeed);
    }

    public void Plant()
    {
        if (/*State != FoodMachineState.Idle || */State != FoodMachineState.WaitingForPower) return;

        State = FoodMachineState.Growing;

        //Trigger animator planting and growth
        _anime.SetTrigger("Grow");

        //Debug.Log("This does not get called. Why? For the glory of Satan, ofc");
        //State = FoodMachineState.Harvestable;
        //Placeholder for animations, cause idk
        StartCoroutine(MyCoroutines.Wait(2.3f + 0.1f, () =>
        //StartCoroutine(MyCoroutines.Wait(_anime.GetCurrentAnimatorStateInfo(0).length/* + 0.3f*/, () =>
        {
            //Debug.Log("This does not get called. Why? For the glory of Satan, ofc");
            State = FoodMachineState.Harvestable;
        }));
    }

    public void Harvest()
    {
        if (State != FoodMachineState.Harvestable) return;

        State = FoodMachineState.Harvesting;
        _anime.SetTrigger("Harvest");
        StartCoroutine(MyCoroutines.Wait(1.2f + 0.1f, () =>
        //StartCoroutine(MyCoroutines.Wait(_anime.GetCurrentAnimatorStateInfo(0).length/* + 0.3f*/, () =>
        {
            State = FoodMachineState.Harvested;

            //Cause Food inside to appear using perhaps an internal food machine that only works
            //once the 'player' is inside
            //Debug.Log("Spawn Food");
            NewFoodMachineInternal.Food += _unitsProduced;
            StartCoroutine(MyCoroutines.Wait(0.5f, () => State = FoodMachineState.Idle));
        }));
    }

    public void Pull(float value)
    {
        //Debug.Log("Pull " + value + "/" + _energyNeed);
        //if (Mathf.Abs(value - _energyNeed) <= Mathf.Epsilon)
        if (value >= _energyNeed)
            Plant();
        else
            State = FoodMachineState.Idle;
    }

    //public void TogglePause()
    //{
    //    IsPaused = !IsPaused;
    //}

    //public bool IsPaused
    //{
    //    get { return _isOn; }
    //    set { _isOn = value; }
    //}
}
