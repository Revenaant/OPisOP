using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusAnimation : MonoBehaviour {

    private float _lifetime;
    private Animator _anime;

	private void Start ()
    {
        _anime = GetComponent<Animator>();
	}
	
	private void Update ()
    {
        _lifetime += Time.deltaTime;
        if (_lifetime >= 178) _anime.SetTrigger("Ded");
	}
}
