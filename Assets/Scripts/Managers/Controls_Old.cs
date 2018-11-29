using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controls : MonoBehaviour
{
    [Header("Input")]
    [Header("Zooming")]
    [SerializeField]
    private float _zoomSpeedTouch = 0.1f;
    [SerializeField] private float _zoomSpeedMouse = 0.5f;

    [SerializeField] private float _lowZoomBound = 10;
    [SerializeField] private float _highZoomBound = 85;

    [Header("Panning - Currently on XZ")]
    [SerializeField]
    private float _panSpeed = 20f;

    [SerializeField] private bool _freezeX = false;
    [SerializeField] private bool _freezeY = true;
    [SerializeField] private bool _freezeZ = false;

    [SerializeField] private Vector3 _lowPanBound = new Vector3(-10, 0, -18);
    [SerializeField] private Vector3 _highPanBound = new Vector3(5, 0, 4);

    //[Header("Zoom Anchors")]
    //[SerializeField] private float _zoomAnchorRadius = 10;

    //[ReadOnly, SerializeField]
    //private List<ZoomAnchor> _zoomAnchors = new List<ZoomAnchor>();

    [Header("Swipe for BACK")]
    [SerializeField]
    private float _swipeForBackSpeed = 10;

    private Vector3 _lastPanPosition;
    private int _panFingerId = -1; //Touch mode only

    private bool _wasZoomingLastFrame = false; //Touch mode only
    private Vector2[] _lastZoomPositions; //Touch mode only

    private Camera _camera = null;

    public UnityEvent onBack;

    private void OnDrawGizmosSelected()
    {
        Vector3 size = _highPanBound - _lowPanBound;
        Vector3 center = _lowPanBound + size / 2;

        Color previous = Gizmos.color;
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(center, size);
    }

    //Use this for initialization
    private void Start()
    {
        _camera = GetComponent<Camera>();
        Debug.Assert(_camera != null);

        //_zoomAnchors.AddRange(FindObjectsOfType<ZoomAnchor>());
    }

    //Update is called once per frame
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Return)) GameManager.Instance.LeaderboardManager.DailyLeaderboard.SaveToFile();

        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
            HandleTouch();
        else
            HandleMouse();
    }

    private void HandleTouch()
    {
        switch (Input.touchCount)
        {
            //Panning
            case 1:
                {
                    _wasZoomingLastFrame = false;

                    //If the touch began, capture its position and its finger ID.
                    //Otherwise, if the finger ID of the touch doesn't match, skip it.
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        _lastPanPosition = touch.position;
                        _panFingerId = touch.fingerId;
                    }
                    else if (touch.fingerId == _panFingerId && touch.phase == TouchPhase.Moved)
                    {
                        Vector3 newPosition = touch.position;
                        Vector3 delta = newPosition - _lastPanPosition;

                        if (delta.x < 0 && delta.sqrMagnitude > _swipeForBackSpeed * _swipeForBackSpeed)
                        {
                            Back();
                            _lastPanPosition = newPosition;
                        }
                        else
                            PanCamera(touch.position);
                    }

                    break;
                }

            //Zooming
            case 2:
                {
                    Vector2[] newPositions = { Input.GetTouch(0).position, Input.GetTouch(1).position };
                    if (!_wasZoomingLastFrame)
                    {
                        _lastZoomPositions = newPositions;
                        _wasZoomingLastFrame = true;
                    }
                    else
                    {
                        //Zoom based on the distance between the new positions compared to the distance between the previous positions.
                        float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                        float oldDistance = Vector2.Distance(_lastZoomPositions[0], _lastZoomPositions[1]);
                        float offset = newDistance - oldDistance;

                       
                        //foreach (var zoomAnchor in _zoomAnchors)
                        //{
                        //    Vector3 za_ScreenSpacePosition = _camera.WorldToScreenPoint(zoomAnchor.transform.position);
                        //    if (Vector3.Distance(za_ScreenSpacePosition, _lastPanPosition) <= _zoomAnchorRadius)
                        //        transform.position = Vector3.MoveTowards(transform.position, Vector3.Project(transform.position, ))
                        //}
                        
                        ZoomCamera(offset, _zoomSpeedTouch);

                        _lastZoomPositions = newPositions;
                    }

                    break;
                }

            default:
                {
                    _wasZoomingLastFrame = false;
                    break;
                }
        }
    }

    private void HandleMouse()
    {
        //On mouse down, capture it's position.
        //Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(0))
        {
            _lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 newPosition = Input.mousePosition;
            Vector3 delta = newPosition - _lastPanPosition;

            if (delta.x < 0 && delta.sqrMagnitude > _swipeForBackSpeed * _swipeForBackSpeed)
            {
                Back();
                _lastPanPosition = newPosition;
            }
            else
                PanCamera(Input.mousePosition);
        }

        //Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, _zoomSpeedMouse);
    }

    private void PanCamera(Vector3 newPanPosition)
    {
        //Determine how much to move the camera
        Vector3 offset = _camera.ScreenToViewportPoint(_lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * _panSpeed, 0, offset.y * _panSpeed);

        //Perform the movement
        transform.Translate(move, Space.World);

        //Ensure the camera remains within bounds.
        Vector3 pos = transform.position;
        if (!_freezeX) pos.x = Mathf.Clamp(transform.position.x, _lowPanBound.x, _highPanBound.x);
        if (!_freezeY) pos.y = Mathf.Clamp(transform.position.y, _lowPanBound.y, _highPanBound.y);
        if (!_freezeZ) pos.z = Mathf.Clamp(transform.position.z, _lowPanBound.z, _highPanBound.z);
        transform.position = pos;

        //Cache the position
        _lastPanPosition = newPanPosition;
    }

    private void ZoomCamera(float offset, float speed)
    {
        if (Math.Abs(offset) < Mathf.Epsilon) return;

        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - offset * speed, _lowZoomBound, _highZoomBound);
    }

    private void Back()
    {
        onBack.Invoke();
        Debug.Log("BACK");
    }
}
