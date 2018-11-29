using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(AudioLowPassFilter))]
public class AudioOcclusion : MonoBehaviour
{
    AudioSource source;
    AudioLowPassFilter LPF;
    Transform audioListener;

    bool occluded = false;

    private float normalVolume;
    private float normalReverb;
    private float normalCutoff;

    private float occludedVolume;
    private float occludedReverb;
    private float occludedCutoff;

    private float interpolationTime = 0.075f;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        LPF = GetComponent<AudioLowPassFilter>();
        audioListener = FindObjectOfType<AudioListener>().transform;

        normalVolume = source.volume;
        occludedVolume = normalVolume * 0.5f;

        normalReverb = source.reverbZoneMix;
        occludedReverb = normalReverb * 1.5f;

        normalCutoff = LPF.cutoffFrequency;
        occludedCutoff = normalCutoff * 0.1f;
    }


    private void Update()
    {
        occluded = !PlayerInSight(transform.position, 0);

        if (occluded)
        {
            source.volume = Mathf.Lerp(source.volume, occludedVolume, interpolationTime);
            source.reverbZoneMix = Mathf.Lerp(source.reverbZoneMix, occludedReverb, interpolationTime);
            LPF.cutoffFrequency = Mathf.Lerp(LPF.cutoffFrequency, occludedCutoff, interpolationTime);
        }
        else
        {
            source.volume = Mathf.Lerp(source.volume, normalVolume, interpolationTime);
            source.reverbZoneMix = Mathf.Lerp(source.reverbZoneMix, normalReverb, interpolationTime);
            LPF.cutoffFrequency = Mathf.Lerp(LPF.cutoffFrequency, normalCutoff, interpolationTime);
        }
    }

    private bool PlayerInSight(Vector3 point, float offset = 0)
    {
        if (audioListener == null)
        {
            print("WARNING: SightCheck script is not attached to a player");
            return false;
        }

        Vector3 direction = audioListener.position - point;
        Ray lineOfSight = new Ray(point + direction.normalized * offset, direction);

        RaycastHit hit;

        if (Physics.Raycast(lineOfSight, out hit) && hit.collider.gameObject == audioListener.gameObject)
            return true;

        return false;
    }

}
