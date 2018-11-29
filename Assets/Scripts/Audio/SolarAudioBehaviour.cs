using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(SolarPanel))]
public class SolarAudioBehaviour : MonoBehaviour {

    private SolarPanel machine;
    private AudioSource source;

    [SerializeField] private AudioClip upgradeClip;
    [SerializeField] private AudioClip singleCleanClip;
    [SerializeField] private AudioClip cleanClip;

    // Use this for initialization
    void Awake () {
        machine = GetComponent<SolarPanel>();
        source = GetComponent<AudioSource>();
	}

    private void OnEnable()
    {
        machine.OnFullyCleaned += playClean;
        machine.OnCleaned += playSingleClean;
        machine.OnUpgradeChanged += playUpgrade;
    }

    private void OnDisable()
    {
        machine.OnFullyCleaned -= playClean;
        machine.OnCleaned -= playSingleClean;
        machine.OnUpgradeChanged -= playUpgrade;
    }

    private void playClean()
    {
        source.PlayOneShot(cleanClip);
    }

    private void playUpgrade(int n)
    {
        source.PlayOneShot(upgradeClip);
    }

    private void playSingleClean()
    {
        source.PlayOneShot(singleCleanClip);
    }
}
