using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Storage), typeof(AudioSource))]
public class BatteryAudioBehaviour : MonoBehaviour {

    private Storage machine;
    private AudioSource source;

    [SerializeField] private AudioClip upgradeClip;
    [SerializeField] private AudioClip fixClip;
    [SerializeField] private AudioClip overheatClip;

    // Use this for initialization
    void Awake()
    {
        machine = GetComponent<Storage>();
        source = GetComponent<AudioSource>();

        source.clip = overheatClip;
        source.loop = true;
    }

    private void OnEnable()
    {
        machine.OnFixed += PlayFix;
        machine.OnOverheatStarted += PlayOverheat;
        machine.OnUpgradeChanged += PlayUpgrade; 
    }

    private void OnDisable()
    {
        machine.OnFixed -= PlayFix;
        machine.OnOverheatStarted -= PlayOverheat;
        machine.OnUpgradeChanged -= PlayUpgrade;
    }

    private void PlayUpgrade(int n)
    {
        source.PlayOneShot(upgradeClip);
    }

    private void PlayFix()
    {
        source.Stop();
        source.PlayOneShot(fixClip);
    }

    private void PlayOverheat()
    {
        source.Play();
    }
}
