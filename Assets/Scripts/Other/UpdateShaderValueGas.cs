using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateShaderValueGas : MonoBehaviour
{
    //[SerializeField] private float lerpSpeed;
    //[SerializeField] private string variableName;

    private Material mat;
    private GasTank gas;

    // Use this for initialization
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        gas = FindObjectOfType<GasTank>();
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_FillAmount", (-(gas.Energy / gas.Capacity) * 3.1f) + 2);
    }
}
