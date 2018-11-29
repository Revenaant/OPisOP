using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMachineAnimation : MonoBehaviour {

    private Animator _anime;
    private LightSwitch _lights;
    private void Start()
    {
        _anime = GetComponent<Animator>();
        _lights = GetComponentInParent<LightSwitch>();
    }

    private void Update()
    {
        if (_anime == null) return;

        _anime.SetBool("IsOn", _lights.IsON);
    }
}
