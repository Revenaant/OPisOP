using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateShaderValue : MonoBehaviour
{
    //[SerializeField] private float lerpSpeed;
    //[SerializeField] private string variableName;

    private Material mat;
    private Storage battery;

	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().material;
        battery = FindObjectOfType<Storage>();
	}
	
	// Update is called once per frame
	void Update () {
        mat.SetFloat("_FillAmount", (-(battery.Energy / battery.Capacity) * 3.1f) + 2);
	}
}
