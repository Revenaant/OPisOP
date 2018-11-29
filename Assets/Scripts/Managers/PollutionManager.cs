using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PollutionManager : MonoBehaviour, IHaveEvents
{
    private float _weightStartNormal = 1;
    private float _weightStartPolluted = 1;
    private bool _dirty = true;

    [SerializeField] private bool _invertPollutedStartWeight = true;

    [SerializeField, Range(0, 1)] private float _pollution = 0.1f;
    [SerializeField, ReadOnly] private float _pollutionDamage = 0.01f;
    //[SerializeField, Range(0, 1)] private float _heavyPollution = 0.1f;

    [SerializeField] private PostProcessVolume _postProcessVolumeNormal = null;
    [SerializeField] private PostProcessVolume _postProcessVolumePolluted = null;

    public static Action<float> onPollutionChange;
    public static Action<float> onPollutionDamageChange;

    public float Pollution
    {
        get { return _pollution; }
        set
        {
            _pollution = Mathf.Clamp01(value);
            if (onPollutionChange != null)
                onPollutionChange(_pollution);
        }
    }

    public float PollutionDamage
    {
        get { return _pollutionDamage; }
    }

    private void OnValidate()
    {
        _dirty = true;
        //UpdatePollutionDamage();
    }

    private void Start()
    {
        GameManager.Instance.PollutionManager = this;

        //Debug.Assert(_postProcessVolumeNormal != null, "The Normal Post Processing Volume is not assigned.");
        //Debug.Assert(_postProcessVolumePolluted != null, "The Polluted Post Processing Volume is not assigned.");

        if (_postProcessVolumeNormal != null)
            _weightStartNormal = _postProcessVolumeNormal.weight;

        if (_postProcessVolumePolluted != null)
        {
            if (_invertPollutedStartWeight)
                _weightStartPolluted = 1 - _postProcessVolumePolluted.weight;
            else
                _weightStartPolluted = _postProcessVolumePolluted.weight;
        }
    }

    public void ForceEvents()
    {
        if (Application.isPlaying && onPollutionChange != null)
            onPollutionChange(_pollution);
        if (Application.isPlaying && onPollutionDamageChange != null)
            onPollutionDamageChange(_pollutionDamage);
    }

    private void Update()
    {
        if (!_dirty) return;
        UpdatePollutionDamage();
        _dirty = true;
    }

    private void UpdatePollutionDamage()
    {
        _pollutionDamage = _pollution * _pollution;

        if (onPollutionDamageChange != null)
            onPollutionDamageChange(_pollutionDamage);

        if (_postProcessVolumeNormal != null)
            _postProcessVolumeNormal.weight = (1 - _pollutionDamage) * _weightStartNormal;
        if (_postProcessVolumePolluted != null)
            _postProcessVolumePolluted.weight = _pollutionDamage * _weightStartPolluted;
    }
}
