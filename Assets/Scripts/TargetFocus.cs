using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFocus : MonoBehaviour
{
    private float _oldZoom = 60;

    private CameraControls _cameraControls = null;

    [SerializeField] private bool _shouldZoom = true;
    [SerializeField, Range(0, 100)] private float _zoom = 40;

    [SerializeField] private bool _shouldActivateActionPanel = true;
    [SerializeField] private ActionPanel _actionPanel = null;

    public bool ShouldZoom
    {
        get { return _shouldZoom; }
        set { _shouldZoom = value; }
    }

    public float OldZoom
    {
        get { return _oldZoom; }
        set { _oldZoom = value; }
    }

    // Use this for initialization
    private void Start()
    {
        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
        {
            _cameraControls = GameManager.Instance.CameraControls;
            if (_cameraControls == null) _cameraControls = FindObjectOfType<CameraControls>();
            Debug.Assert(_cameraControls != null, "Can't find Camera Controls.");
        }));
    }

    public void Focus()
    {
        if (!isActiveAndEnabled) return;

        //Debug.Log("Inside: " + GameManager.Instance.InDome);
        if (GameManager.Instance.InDome)
        {
            InsideFocus();
            //Debug.Log("Focus Pls");
            //return;
        }

        //Debug.Log("Focus");
        if (_cameraControls == null)
        {
            _cameraControls = GameManager.Instance.CameraControls;
            if (_cameraControls == null) _cameraControls = FindObjectOfType<CameraControls>();
        }

        _cameraControls.MoveFocus(transform);

        if (ShouldZoom)
        {
            float buffer = -_cameraControls.GetZoom();
            if (buffer > _zoom)
            //if (Math.Abs(buffer - _zoom) > Mathf.Epsilon)
                    _oldZoom = buffer;
            _cameraControls.SetZoom(_zoom);
        }
    }

    private void InsideFocus()
    {
        var ci = _cameraControls.GetComponent<CameraInterior>();
        ci.enabled = false;
        _cameraControls.enabled = true;
        _cameraControls.ResetRotation();

        //Focus();
    }

    public void Unfocus()
    {
        if (!isActiveAndEnabled) return;
        //Debug.Log("Unfocus");
        if (ShouldZoom)
            //_cameraControls.SetZoom(60);
            _cameraControls.SetZoom(_oldZoom);
    }

    public void ActivateActionPanel()
    {
        if (!isActiveAndEnabled || !_shouldActivateActionPanel || _actionPanel == null) return;

        //_actionPanel.
        _actionPanel.Activate();
    }

    public void DeactivateActionPanel()
    {
        if (!isActiveAndEnabled || !_shouldActivateActionPanel || _actionPanel == null) return;

        _actionPanel.Deactivate();
    }
}
