using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {

    [Tooltip("Distance range of the animation")]
    [SerializeField] private float _amplitude = 0.1f;

    [Tooltip("Speed of the animation")]
    [SerializeField] private float _period = 1.1f;

    [Tooltip("Direction of the oscillation")]
    [SerializeField] private Vector3 _direction = Vector3.up;

    [Tooltip("Speed of the local rotation of the object")]
    [SerializeField] private float _rotationSpeed = 3.0f;

    private Vector3 _startPosition;

    protected void Start() {
        _startPosition = transform.position;
    }

    protected void Update() {
        Vector3 pos = _startPosition + _direction * _amplitude * Mathf.Sin(2.0f * Mathf.PI * Time.time / _period);
        transform.position = pos;
        transform.Rotate(0, _rotationSpeed, 0, Space.World);
    }
}
