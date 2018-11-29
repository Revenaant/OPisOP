using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDissolveTrigger : MonoBehaviour
{	
	// Update is called once per frame
    [ExecuteInEditMode]
	void Update () {
        float t = Mathf.PingPong(Time.time * 0.25f, 1);
        GetComponent<Renderer>().material.SetFloat("_DissolveScale", t);
	}
}
