using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewFoodMachine), typeof(AudioSource))]
public class NewFoodAudioBehaviour : MonoBehaviour
{
    //private NewFoodMachine _machine;
    private AudioSource _source;

    [SerializeField] private AudioClip _plantClip = null;
    [SerializeField] private AudioClip _harvestClip = null;
    //[SerializeField] private AudioClip _machineClip = null;

    // Use this for initialization
    private void Awake()
    {
        //_machine = GetComponent<NewFoodMachine>();
        _source = GetComponent<AudioSource>();

        //source.clip = runningClip;
        //_source.loop = true;
    }

    //private void OnEnable()
    //{
    //    //_machine = GetComponent<NewFoodMachine>();
    //    //if (_machine == null) return;

    //    //_machine.onPowered += PlayMachine;
    //    //_machine.onStartGrowing += PlayPlant;
    //    //_machine.onHarvest += PlayHarvest;
    //    NewFoodMachine.onStartGrowing += PlayPlant;
    //    NewFoodMachine.onHarvest += PlayHarvest;
    //}

    //private void OnDisable()
    //{
    //    //if (_machine == null) return;

    //    //if (_machine.onPowered != null)
    //    //    _machine.onPowered -= PlayMachine;

    //    //if (_machine.onStartGrowing != null)
    //    //    _machine.onStartGrowing -= PlayPlant;

    //    //if (_machine.onHarvest != null)
    //    //    _machine.onHarvest -= PlayHarvest;

    //    if (NewFoodMachine.onStartGrowing != null)
    //        NewFoodMachine.onStartGrowing -= PlayPlant;

    //    if (NewFoodMachine.onHarvest != null)
    //        NewFoodMachine.onHarvest -= PlayHarvest;
    //}

    private void OnEnable()
    {
        NewFoodMachine.onStateChange += StateHandler;
        //NewFoodMachine.onGrown += OnGrownHandler;
        //NewFoodMachine.onHarvest += OnHarvestHandler;
    }

    private void OnDisable()
    {
        if (NewFoodMachine.onStateChange != null)
            NewFoodMachine.onStateChange -= StateHandler;
        //NewFoodMachine.onGrown -= OnGrownHandler;
        //NewFoodMachine.onHarvest -= OnHarvestHandler;
    }

    private void StateHandler(FoodMachineState state)
    {
        switch (state)
        {
            case FoodMachineState.Harvestable:
                PlayPlant();
                break;
            case FoodMachineState.Harvested:
                PlayHarvest();
                break;
        }
    }

    //private void PlayMachine()
    //{
    //    Debug.Log("PlayMachine");
    //    if (_source != null && _machineClip != null)
    //        _source.PlayOneShot(_machineClip);
    //}

    private void PlayPlant()
    {
        if (_source != null && _plantClip != null)
            _source.PlayOneShot(_plantClip);
        //Debug.Log("PlayPlant");
    }

    private void PlayHarvest()
    {
        //Debug.Log("PlayHarvest");
        if (_source != null && _harvestClip != null)
            _source.PlayOneShot(_harvestClip);
    }
}
