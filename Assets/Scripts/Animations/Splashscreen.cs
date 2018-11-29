using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Splashscreen : MonoBehaviour {

    private Animator _anim;

    public GameObject _oldui;
    public GameObject _newui;
    public ParticleSystem _Sleeping;

    //private ParticleSystem.EmissionModule emission;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        //_Sleeping = GetComponent<ParticleSystem>();
        //emission = _sleeping.emission;
    }

    public void touchPet()
    {
        _anim.SetBool("Splashscreen", true);
        //_anim.SetTrigger("transition");
        _oldui.SetActive(false);
        _newui.SetActive(true);
        _Sleeping.Stop();
        //emission.rateOverTime = 0;
    }
}
