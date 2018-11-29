using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class CarrotPhysicsFlick : MonoBehaviour
{
    private Vector3 _lastDelta = Vector3.zero;
    private TransformGesture _transformGesture = null;
    private Rigidbody _rigidbody = null;

    [SerializeField] private float _forceModifier = 10;

    private void OnEnable()
    {
        if (_transformGesture == null) _transformGesture = GetComponent<TransformGesture>();
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();

        _transformGesture.Transformed += TransformedHandler;
        _transformGesture.TransformCompleted += TransformCompleteHandler;
    }

    private void OnDisable()
    {
        if (_transformGesture == null) return;

        _transformGesture.Transformed -= TransformedHandler;
        _transformGesture.TransformCompleted -= TransformCompleteHandler;
    }

    private void TransformedHandler(object sender, EventArgs args)
    {
        _lastDelta = _transformGesture.DeltaPosition;
    }

    private void TransformCompleteHandler(object sender, EventArgs args)
    {
        Throw(_lastDelta);
    }

    private void Throw(Vector3 directionAndPower)
    {
        //Debug.Log("Throw");
        _rigidbody.AddForce(directionAndPower * _forceModifier, ForceMode.Impulse);
    }
}
