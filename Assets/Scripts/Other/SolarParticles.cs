using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SolarPanel))]
public class SolarParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private ParticleSystem FullCleanParticle;
    [SerializeField] private Transform pivot;
    private SolarPanel solar;

	// Use this for initialization
	private void Awake () {
        solar = GetComponent<SolarPanel>();
	}

    private void OnEnable()
    {
        solar.OnCleaned += Scrub;
        solar.OnFullyCleaned += FullScrubbed;
    }

    private void OnDisable()
    {
        solar.OnCleaned -= Scrub;
        solar.OnFullyCleaned -= FullScrubbed;
    }

    public void Scrub()
    {
        ParticleSystem p = Instantiate(particle, pivot.position, Quaternion.identity, solar.transform);
        Destroy(p.gameObject, p.main.startLifetime.constant);
    }

    public void FullScrubbed()
    {
        ParticleSystem p = Instantiate(FullCleanParticle, pivot.position, Quaternion.identity, solar.transform);
        Destroy(p.gameObject, p.main.startLifetime.constant);
    }
}
