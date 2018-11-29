using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlantBed), typeof(AudioSource))]
public class PlantBedAudioBehaviour : MonoBehaviour {

    private PlantBed machine;
    private AudioSource source;

    [SerializeField] private AudioClip makeClip;
    [SerializeField] private AudioClip deadClip;

    // Use this for initialization
    void Awake()
    {
        machine = GetComponent<PlantBed>();
        source = GetComponent<AudioSource>();

        source.loop = true;
    }

    private void OnEnable()
    {
        machine.OnMake += PlayMake;
        machine.OnDead += PlayDead;
    }

    private void OnDisable()
    {
        machine.OnMake -= PlayMake;
        machine.OnDead -= PlayDead;
    }
    
    private void PlayMake()
    {
        source.PlayOneShot(makeClip);
    }

    private void PlayDead()
    {
        source.PlayOneShot(deadClip);
    }
}
