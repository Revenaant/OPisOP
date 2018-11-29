using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class CarrotPetFollower : MonoBehaviour
{
    private Pet _petTarget = null;
    private TransformGesture _transformGesture = null;
    private bool _follow = true;

    private void OnEnable()
    {
        if (_transformGesture == null) _transformGesture=GetComponent<TransformGesture>();

        _transformGesture.TransformStarted += TransformStartedHandler;
        _transformGesture.TransformCompleted += TransformCompletedHandler;
    }

    // Use this for initialization
    private void Start()
    {
        _petTarget = GameManager.Instance.Pet;
        if (_petTarget == null) _petTarget = FindObjectOfType<Pet>();
        Debug.Assert(_petTarget != null, "Could not find Pet behaviour");
    }

    private void OnDisable()
    {
        if (_transformGesture == null) return;

        _transformGesture.TransformStarted -= TransformStartedHandler;
        _transformGesture.TransformCompleted -= TransformCompletedHandler;
    }


    private void TransformCompletedHandler(object sender, EventArgs e)
    {
        _follow = true;
    }

    private void TransformStartedHandler(object sender, EventArgs e)
    {
        _follow = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_follow || _petTarget == null || _transformGesture == null) return;

        Vector3 position = transform.position;
        Vector3 targetPosition = _petTarget.transform.position;
        Vector3 delta = targetPosition - position;
        Vector3 deltaNormalized = delta.normalized;

        Vector3 perpendicular = Vector3.Cross(deltaNormalized, Vector3.up);

        _transformGesture.Projection = TransformGesture.ProjectionType.Global;
        _transformGesture.ProjectionPlaneNormal = perpendicular;
    }
}
