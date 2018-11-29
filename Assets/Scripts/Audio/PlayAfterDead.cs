using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAfterDead : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField, Range(0, 256)] private int priority = 128;
    [SerializeField, Range(0, 1)] private float volume = 1;
    [SerializeField, Range(-3, 3)] private float pitch = 1;
    [SerializeField, Range(0, 1)] private float spatialBlend = 0;

    public AudioSource PlayClip()
    {
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = transform.position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip; // define the clip
        aSource.volume = volume;
        aSource.spatialBlend = spatialBlend;
        aSource.priority = priority;
        aSource.pitch = pitch;

        aSource.Play(); // start the sound
        Destroy(tempGO, clip.length); // destroy object after clip duration
        return aSource; // return the AudioSource reference
     }

    public void PlayClipVoid()
    {
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = transform.position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clip; // define the clip
        aSource.volume = volume;
        aSource.spatialBlend = spatialBlend;
        aSource.priority = priority;
        aSource.pitch = pitch;

        aSource.Play(); // start the sound
        Destroy(tempGO, clip.length); // destroy object after clip duration
    }

    public void PlayClip(AudioClip clipy)
    {
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = transform.position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clipy; // define the clip
        aSource.volume = volume;
        aSource.spatialBlend = spatialBlend;
        aSource.priority = priority;
        aSource.pitch = pitch;

        aSource.Play(); // start the sound
        Destroy(tempGO, clipy.length); // destroy object after clip duration
    }

    public void PlayClipCode(AudioClip clipy, float pVolume = 1, float pSpatialBlend = 0, float pPitch = 1, int pPriority = 128)
    {
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = transform.position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        aSource.clip = clipy; // define the clip
        aSource.volume = pVolume;
        aSource.spatialBlend = pSpatialBlend;
        aSource.priority = pPriority;
        aSource.pitch = pPitch;

        aSource.Play(); // start the sound
        Destroy(tempGO, clipy.length); // destroy object after clip duration
    }
}
