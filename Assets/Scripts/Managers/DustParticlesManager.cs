using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DustParticlesManager : MonoBehaviour
{
    private List<ParticleSystem> _particleSystems = null;
    private ParticleSystem _windParticle = null;

    // Use this for initialization
    private void Start()
    {
        _windParticle = GetComponent<ParticleSystem>();
        _particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
        _particleSystems.Remove(_windParticle);
    }

    public void PlayWind()
    {
        if (!_windParticle.isPlaying)
            _windParticle.Play(false);
    }

    public void PlayStorm()
    {
        foreach (ParticleSystem system in _particleSystems)
            if (!system.isPlaying)
                system.Play(false);
    }
}
