using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLightSwitch : MonoBehaviour {

    private Animator _anim;
    // Use this for initialization
    void Start () {
        _anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	public void LightSwitch() {

        _anim.SetTrigger("transition");

    }
}
