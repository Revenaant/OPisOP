using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FoodMachine), typeof(AudioSource))]
public class FoodAudioBehaviour : MonoBehaviour
{
    private FoodMachine machine;
    private AudioSource source;

    [SerializeField] private AudioClip plantClip;
    [SerializeField] private AudioClip harvestClip;
    [SerializeField] private AudioClip machineClip;

    // Use this for initialization
    void Awake()
    {
        machine = GetComponent<FoodMachine>();
        source = GetComponent<AudioSource>();

        //source.clip = runningClip;
        source.loop = true;
    }

    private void OnEnable()
    {
        machine.OnPowered += PlayMachine;

        //machine.OnPlant += PlayPlant;
        //machine.onHarvest += PlayHarvest;
    }

    private void OnDisable()
    {
        machine.OnPowered -= PlayMachine;

        //machine.OnPlant -= PlayPlant;
        //machine.onHarvest -= PlayHarvest;
    }

    private void PlayMachine(bool on)
    {
        if (on) source.PlayOneShot(machineClip);
    }

    private void PlayPlant()
    {
        source.PlayOneShot(plantClip);
    }

    private void PlayHarvest()
    {
        source.PlayOneShot(harvestClip);
    }
}
