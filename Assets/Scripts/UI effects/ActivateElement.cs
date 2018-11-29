using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateElement : MonoBehaviour {

    public Vector3 displacement;
    Vector3 startPos;
    bool display = true;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if(display)
            transform.position = Vector3.Lerp(transform.position, startPos, 5 * Time.deltaTime);
        else
            transform.position = Vector3.Lerp(transform.position, startPos + displacement, 5 * Time.deltaTime);
    }

    public void Toggle()
    {
        display = !display;
    }
}
