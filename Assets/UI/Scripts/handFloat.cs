using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handFloat : MonoBehaviour
{
    public Vector3 _startPosition;
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
        _startPosition = transform.localPosition;
    }




    // Update is called once per frame
    void Update()
    {
        float newPos = Mathf.Sin(Time.time * 2 * Mathf.PI / period) * amplitude;

        transform.localPosition = _startPosition + new Vector3(0, newPos, 0);

    }
}
