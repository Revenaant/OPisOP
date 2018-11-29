using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitParticles : MonoBehaviour
{
    private float _oldHappiness = float.PositiveInfinity;
    private float _timer = 0;

    [SerializeField]
    private UIParticleSystem _particles = null;

    [SerializeField] private float _cooldown = 3;

    // Use this for initialization
    private void OnEnable()
    {
        Pet.onHappinessChanged += CheckParticles;
    }

    private void OnDisable()
    {
        if (Pet.onHappinessChanged != null)
            Pet.onHappinessChanged -= CheckParticles;
    }

    private void Update()
    {
        if (_timer > 0)
            _timer -= Time.deltaTime;
    }

    private void CheckParticles(float happiness)
    {
        //if (happiness >= 1)
        //{
        //    PlayParticles();
        //    _oldHappiness = 1.1f;
        //    return;
        //}

        if (_timer <= Mathf.Epsilon && happiness > _oldHappiness)
        {
            PlayParticles();
            _timer = _cooldown;
        }

        _oldHappiness = happiness;
    }

    // Update is called once per frame
    private void PlayParticles()
    {
        if (_particles != null)
            _particles.Play();
    }
}
