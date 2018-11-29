using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFoodButtons : MonoBehaviour
{
    [SerializeField] private GameObject _buttonPlant = null;
    [SerializeField] private GameObject _buttonHarvest = null;
    [SerializeField] private GameObject _disabledButtonPlant = null;
    [SerializeField] private GameObject _disabledButtonHarvest = null;

    //private void OnEnable()
    //{
    //    NewFoodMachine.onStateChange += StateHandler;
    //    //NewFoodMachine.onGrown += OnGrownHandler;
    //    //NewFoodMachine.onHarvest += OnHarvestHandler;
    //}

    private void Start()
    {
        //if (NewFoodMachine.onStateChange == null)
            NewFoodMachine.onStateChange += StateHandler;
    }

    private void OnDestroy()
    {
        if (NewFoodMachine.onStateChange != null)
            NewFoodMachine.onStateChange -= StateHandler;
        //NewFoodMachine.onGrown -= OnGrownHandler;
        //NewFoodMachine.onHarvest -= OnHarvestHandler;
    }

    private void StateHandler(FoodMachineState state)
    {
        //StartCoroutine(MyCoroutines.WaitOneFrame(() =>
        //{
        var stateCopy  = state;
        switch (stateCopy)
        {
            case FoodMachineState.Harvested:
                {
                    //Debug.Log(4);
                    OnHarvested();
                    break;
                }
            case FoodMachineState.Harvesting:
                {
                    //Debug.Log(3);
                    OnHarvesting();
                    break;
                }
            case FoodMachineState.Harvestable:
                {
                    //Debug.Log(2);
                    OnHarvestable();
                    break;
                }
            case FoodMachineState.Growing:
                {
                    //Debug.Log(1);
                    OnGrowing();
                    break;
                }
        }
        //}));
    }

    private void OnGrowing()
    {
        //if (_buttonPlant == null || _disabledButtonPlant == null) return;

        //Debug.Log("On Grow");
        _buttonPlant.SetActive(false);
        _disabledButtonPlant.SetActive(true);
        _buttonHarvest.SetActive(false);
        _disabledButtonHarvest.SetActive(false);

        //print("GOGOGOGOGOGO1");
    }

    private void OnHarvestable()
    {
        //if (_buttonHarvest == null || _disabledButtonPlant == null) return;

        //Debug.Log("On Harvestable");
        _buttonPlant.SetActive(false);
        _disabledButtonPlant.SetActive(false);
        _buttonHarvest.SetActive(true);
        _disabledButtonHarvest.SetActive(false);

    }

    private void OnHarvesting()
    {
        //if (_buttonHarvest == null || _disabledButtonHarvest == null) return;

        //Debug.Log("On Harvesting");
        _buttonPlant.SetActive(false);
        _disabledButtonPlant.SetActive(false);
        _buttonHarvest.SetActive(false);
        _disabledButtonHarvest.SetActive(true);

    }

    private void OnHarvested()
    {
        //if (_buttonPlant == null || _disabledButtonHarvest == null) return;

        //Debug.Log("On Harvested");
        _buttonPlant.SetActive(true);
        _disabledButtonPlant.SetActive(false);
        _buttonHarvest.SetActive(false);
        _disabledButtonHarvest.SetActive(false);

    }
}
