using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackParticles : MonoBehaviour {

    [SerializeField] private ParticleSystem _particle;

    public ParticleSystem Particle
    {
        get { return _particle; }
        set { _particle = value; }
    }

    public void Spawn(Vector3 pos)
    {
        Instantiate(Particle, pos, Quaternion.identity);
    }
}
