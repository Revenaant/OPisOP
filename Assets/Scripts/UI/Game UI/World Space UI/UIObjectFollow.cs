using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIObjectFollow : MonoBehaviour
{

    [Header("Object to follow")]
    [SerializeField] private Transform _target;

    [Tooltip("Offset of the UI Element from the target object")]
    [SerializeField] private Vector3 _offset = Vector3.up;

    [SerializeField, Range(0, 0.05f)]
    private float _zoomFactor = 0.01f;

    [SerializeField, Range(0, 100f)]
    private float _lerpFactor = 35f;

    private Camera _cam;
    private CameraControls _controller;
    //private Vector3 _startPosition = Vector3.zero;
    //private Vector3 _forward = Vector3.zero;
    private List<Graphic> _graphics = new List<Graphic>();

    // Use this for initialization
    void Start()
    {
        _cam = FindObjectOfType<Camera>();
        _controller = FindObjectOfType<CameraControls>();
        _graphics = GetComponentsInChildren<Graphic>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
        {
            Debug.Log("No target UIObjectFollow");
            return;
        }

        float dot = Vector3.Dot(_cam.transform.forward, (_target.position - _cam.transform.position).normalized);
        bool graphicEnabled = dot >= 0;
        foreach (Graphic graphic in _graphics)
            graphic.enabled = graphicEnabled;

        transform.position = Vector3.Lerp(transform.position,
        _cam.WorldToScreenPoint((_offset) + _target.transform.position), _lerpFactor * Time.deltaTime);
    }

    private float AdjustedZoom
    {
        get { return _controller.GetZoom() * -_zoomFactor; }
    }
}
