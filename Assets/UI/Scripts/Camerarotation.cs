using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerarotation : MonoBehaviour
{
    [ReadOnly]
    public Quaternion _startRotation = Quaternion.identity;
    public float amplitude = 2;
    public float timesASecond = 5;
    [ReadOnly]
    public float period = 1;

    private void OnValidate()
    {
        period = 1 / timesASecond;
    }

    // Use this for initialization
    void Start()
    {
        _startRotation = transform.localRotation;
    }




    // Update is called once per frame
    void Update()
    {
        float newRotation = Mathf.Sin(Time.time * 2 * Mathf.PI / period) * amplitude;
        transform.localRotation = Quaternion.AngleAxis(newRotation, Vector3.up) * _startRotation;
    }
}
