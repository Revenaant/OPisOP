using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TouchScript.Custom;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Gestures.TransformGestures.Base;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ScreenTransformGesture))]
public class CameraControls : MonoBehaviour
{
    private List<ScreenTransformReleaseGesture> _otherGestures = null;

    private Vector3 _startCameraPosition = Vector3.zero;
    private Quaternion _startCameraRotation = Quaternion.identity;
    private Vector3 _startPivotPosition = Vector3.zero;

    public Vector3 StartCameraPosition
    {
        get { return _startCameraPosition; }
        set { _startCameraPosition = value; }
    }

    public Vector3 StartPivotPosition
    {
        get { return _startPivotPosition; }
        set { _startPivotPosition = value; }
    }

    [Header("Zooming")]
    [SerializeField, ReadOnly] private float _currentZoom = 0;
    [SerializeField] private float _zoomSpeed = 10;
    [SerializeField] private Vector2 _zoomMinMax = new Vector2(10, 10);
    [SerializeField] private float _inDoorZoom = 0.5f;

    private Vector3 _startZoomPos;
    private Vector3 _angle;
    private Vector3 _newPos;

    [Header("Panning")]

    [SerializeField] private float _panSpeed = 1;
    [SerializeField] private Collider _panBounds = null;
    //[SerializeField] private float _rotationSpeed = 1;

    [Header("Gestures")]
    [SerializeField] private ScreenTransformGesture _panGesture = null;
    [SerializeField] private ScreenTransformGesture _manipulationGesture = null;

    [Header("Transforms")]
    [SerializeField] private Transform _pivot = null;
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _focus = null;
    [SerializeField] private Transform _inDoorFocus = null;
    private Coroutine rotateUntil;

    [SerializeField] private float _lerpSpeed = 10f;

    private bool _paused = false;

    public Vector3 inCameraPosTarget { get; set; }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(_pivot.position, _panMaxLimit - _panMinLimit);
        Vector3 cameraPosition = Application.isPlaying ? _startCameraPosition : _camera.position;

        GizmosHelper.DrawGizmoLineArrow(cameraPosition, cameraPosition + _camera.forward * _zoomMinMax.x,
            Color.yellow);
        GizmosHelper.DrawGizmoLineArrow(cameraPosition, cameraPosition - _camera.forward * _zoomMinMax.y,
            Color.magenta);
        //GizmosHelper.DrawGizmoLineArrow(cameraPosition, cameraPosition + _camera.forward, Color.yellow);
        //GizmosHelper.DrawGizmoLineArrow(cameraPosition + _boxPosition, cameraPosition + _boxBottom, Color.magenta);
        //Gizmos.DrawLine(cameraPosition + _boxPosition, cameraPosition + _boxBottom);

        //Vector3 p1 = panMinLimit;
        //Vector3 p2 = new Vector3(panMinLimit.x, 0, panMaxLimit.z);
        //Vector3 p3 = panMaxLimit;
        //Vector3 p4 = new Vector3(panMaxLimit.x, 0, panMinLimit.z);

        if (_panBounds != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_panBounds.bounds.center, _panBounds.bounds.size);
        }

    }

    private void Start()
    {
        Debug.Assert(_camera != null, "Camera is not assigned.");
        Debug.Assert(_pivot != null, "Pivot is not assigned.");
        Debug.Assert(_focus != null, "Focus is not assigned.");
        Debug.Assert(_panBounds != null, "pan Bounds are not assigned.");
        Debug.Assert(_panGesture != null, "Pan Gesture is not assigned.");
        Debug.Assert(_manipulationGesture != null, "Manipulation Gesture is not assigned.");

        GameManager.Instance.MainCamera = _camera.GetComponent<Camera>();
        GameManager.Instance.CameraControls = this;

        _startCameraPosition = _camera.position;
        _startPivotPosition = _pivot.position;
        _startCameraRotation = _camera.rotation;

        _newPos = _startPivotPosition;
        _currentZoom = -60f;

        if (_focus != null) _angle = (_focus.position - _pivot.position).normalized;

        MoveFocus(_inDoorFocus);
        SetZoom(_inDoorZoom);

        _paused = true;
        StartCoroutine(MyCoroutines.Wait(2, () => _paused = false));
    }

    public void ReplaceExistingCamera()
    {
        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != _camera.gameObject)
            {
                //_camera.position = cameras[i].transform.position;
                ReplaceRotation(cameras[i].transform.rotation);
                _newPos = cameras[i].transform.position;
                inCameraPosTarget = cameras[i].transform.position;
                cameras[i].SetActive(false);
                //FixedUpdate();
            }
        }
    }

    private void ReplaceRotation(Quaternion newRotation)
    {
        if (rotateUntil != null) StopCoroutine(rotateUntil);

        rotateUntil = StartCoroutine(MyCoroutines.DoUntil(
        // Condition
        () => { return Quaternion.Angle(_camera.rotation, newRotation) > 0.1f; },
        // What to do
        () =>
        {
            _camera.rotation = Quaternion.Slerp(_camera.rotation, newRotation, 4.5f * Time.deltaTime);
        },
        // Do After
        () =>
        {
            _camera.rotation = newRotation; StartCoroutine(MyCoroutines.Wait(0.75f,
        () =>
        {
            if (GameManager.Instance.InDome)
            { this.enabled = false; GetComponent<CameraInterior>().enabled = true; }
        }));
        }));
    }

    private void OnEnable()
    {
        _otherGestures = FindObjectsOfType<ScreenTransformReleaseGesture>().ToList();

        _otherGestures.ForEach(strg => strg.Transformed += PanTransformHandler);
        _panGesture.Transformed += PanTransformHandler;
        //_manipulationGesture.TransformStarted += CaptureZoomStartPos;
        _manipulationGesture.Transformed += ManipulationTransformedHandler;
    }

    private void OnDisable()
    {
        _otherGestures.ForEach(strg => strg.Transformed -= PanTransformHandler);
        _panGesture.Transformed -= PanTransformHandler;
        //_manipulationGesture.TransformStarted -= CaptureZoomStartPos;
        _manipulationGesture.Transformed -= ManipulationTransformedHandler;
    }

    private void LateUpdate()
    {
        if (_paused) return;

        ReplaceExistingCamera();
        UpdateCameraZoom();

        // Update the position of the camera
        _pivot.position = Vector3.Lerp(_pivot.position, _newPos, _lerpSpeed * Time.deltaTime);

        // Snap when close enough to prevent annoying slerp behavior
        if ((_newPos - _pivot.position).sqrMagnitude <= float.Epsilon)
            _pivot.position = _newPos;
    }

    //private void CaptureZoomStartPos(object sender, EventArgs e)
    //{
    //    _startZoomPos = new Vector3(_pivot.position.x, _startPivotPosition.y, _pivot.position.z);
    //}

    private void ManipulationTransformedHandler(object sender, EventArgs e)
    {
        //var rotation = Quaternion.Euler(_manipulationGesture.DeltaPosition.y / Screen.height * _rotationSpeed,
        //    -_manipulationGesture.DeltaPosition.x / Screen.width * _rotationSpeed,
        //    _manipulationGesture.DeltaRotation);
        //_pivot.localRotation *= rotation;
        //_pivot.transform.localPosition += _camera.transform.forward * (_manipulationGesture.DeltaScale - 1f) * _zoomSpeed;

        ////Clamp
        //Vector3 delta = _startPosition - _camera.transform.position;
        //Debug.Log(delta);
        //_currentZoom = (-delta).magnitude * Mathf.Sign(Vector3.Dot(-delta, _camera.forward));
        //if (Vector3.Dot(delta, _camera.forward) < 0) _currentZoom *= -1;
        //if ()

        _currentZoom += (_manipulationGesture.DeltaScale - 1f) * _zoomSpeed;
        _currentZoom = Mathf.Clamp(_currentZoom, -_zoomMinMax.x, _zoomMinMax.y);
        _lerpSpeed = 10f;

        //UpdateCameraZoom();
    }

    /// <summary>
    /// Updates the position of the focus when panning around
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PanTransformHandler(object sender, EventArgs e)
    {
        //_pivot.localPosition += _pivot.rotation * _panGesture.DeltaPosition * _panSpeed;
        //var moved = pivot.rotation * panGesture.DeltaPosition * panSpeed;
        //moved = ClampVector3(moved,
        //    panMinLimit + pivot.transform.position,
        //    panMaxLimit + pivot.transform.position);
        //_pivot.position = ClampVector3(_pivot.position, _startPosition + _panMinLimit, _startPosition + _panMaxLimit);
        //var pivotLocalPos = pivot.localPosition;
        //Vector3 pivotPosition = _pivot.position;
        //moved.y = 0;
        //Vector3 boxPos = _pivot.InverseTransformPoint(_boxPosition);
        //_pivot.Translate(moved.x, 0, moved.y);
        //_pivot.position += ;

        //print(sender);

        Vector3 delta = _panGesture.DeltaPosition;
        _otherGestures.ForEach(g => delta += g.DeltaPosition);
        Vector3 moved = delta * (_panSpeed * (_currentZoom / 60f));
        Vector3 clampedPos = _focus.position + (_pivot.rotation * new Vector3(moved.x, 0, moved.y));
        Bounds bounds = _panBounds.bounds;

        clampedPos.x = Mathf.Clamp(clampedPos.x, bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x);
        clampedPos.z = Mathf.Clamp(clampedPos.z, bounds.center.z - bounds.extents.z, bounds.center.z + bounds.extents.z);

        _focus.position = clampedPos;
        _lerpSpeed = 10f;

        //// Update the position of the camera
        //_pivot.position = Vector3.Slerp(_pivot.position, newPos, 10f * Time.deltaTime);

        //// Snap when close enough to prevent annoying slerp behavior
        //if ((newPos - _pivot.position).sqrMagnitude <= float.Epsilon)
        //    _pivot.position = newPos;

        //_pivot.position = ClampVector3(
        //    pivotPosition + moved,
        //    boxPos,
        //    boxPos + _boxSize
        //);
    }

    private void UpdateCameraZoom()
    {
        //if (_currentZoom != _oldZoom)
        //{

        //    _zooming = true;
        //}
        //if (_zooming == true)
        //{
        //    _startZoomPos = _pivot.position;
        //}
        //Vector3 pivotPos = new Vector3(_pivot.position.x, _startPivotPosition.y, _pivot.position.z);


        //Vector3 newpos = _startZoomPos + _currentZoom * _camera.transform.forward;

        if (_focus == null || _panBounds == null) return;

        Vector3 newpos = _focus.position + _currentZoom * _angle;

        Bounds bounds = _panBounds.bounds;
        newpos.x = Mathf.Clamp(newpos.x, bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x);
        newpos.z = Mathf.Clamp(newpos.z, bounds.center.z - bounds.extents.z, bounds.center.z + bounds.extents.z);

        _newPos = newpos;

        // Update the position of the camera
        //_pivot.transform.localPosition = Vector3.Slerp(_pivot.position, newpos, 10f * Time.deltaTime);

    }

    /// <summary>
    /// Moves the focus to the specified position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="focus"></param>
    public void MoveFocus(Vector3 position)
    {
        _lerpSpeed = 5f;
        //if (_focus != null)
        _focus.position = new Vector3(position.x, _focus.position.y, position.z);
    }

    public void MoveFocus(Transform location)
    {
        //if (location != null)
        MoveFocus(location.position);
    }

    /// <summary>
    /// Sets the zoom between 0(min) and 100(max). 0 is closer to the focus.
    /// </summary>
    /// <param name="zoom"></param>
    public void SetZoom(float zoom)
    {
        _currentZoom = -zoom;/* -Mathf.Clamp(zoom, _zoomMinMax.x, _zoomMinMax.y);*/
    }

    public float GetZoom()
    {
        return _currentZoom;
    }

    /// <summary>
    /// Resets the rotation of the camera to the starting value
    /// </summary>
    public void ResetRotation()
    {
        ReplaceRotation(_startCameraRotation);
    }
}
