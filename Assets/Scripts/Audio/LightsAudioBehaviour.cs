﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LightSwitch), typeof(AudioSource))]
public class LightsAudioBehaviour : MonoBehaviour {

    private LightSwitch machine;
    private AudioSource source;

    [SerializeField] private AudioClip startClip;
    [SerializeField] private AudioClip endClip;
    [SerializeField] private AudioClip runningClip;

    // Use this for initialization
    void Awake()
    {
        machine = GetComponent<LightSwitch>();
        source = GetComponent<AudioSource>();

        source.clip = runningClip;
        source.loop = true;
    }

    private void OnEnable()
    {
        machine.OnPowered += Play;
    }

    private void OnDisable()
    {
        machine.OnPowered -= Play;
    }

    private void Play(bool on)
    {
        if (on)
        {
            source.PlayOneShot(startClip);

            source.PlayDelayed(startClip.length);
        }
        else
        {
            source.Stop();
            source.PlayOneShot(endClip);
        }
    }
}