using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPanelUI_Old_Old : MonoBehaviour
{
    private bool _isSelected = false;
    private LineRenderer _lineRenderer = null;

    [Header("If a Line Renderer is present, will use it draw a line")]
    [Header("Optional")]
    [SerializeField] private bool _followRotation = false;
    [SerializeField] private float _rotationSpeed = 25;

    [Header("Speed at which it'll go to the end.")]
    [SerializeField] private float _translationSpeed = 25;
    [SerializeField] private float _scaleSpeed = 25;

    [Header("Where it'll spawn from and be connected to")]
    [SerializeField]
    private bool _extrapolated = true;
    [SerializeField]
    private Transform _start = null;
    [Header("Where it'll go")]
    [SerializeField]
    private Transform _end = null;

    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;

            //gameObject.SetActive(value);
            if (_extrapolated)
            {
                if (value) LerpTowards();
                else LerpBack();
            }
            else
            {
                if (value) TransformTowards();
                else TransformBack();
            }
        }
    }

    public Action onStartMoveTowards;
    public Action onStartMoveBack;
    public Action onFinishMoveTowards;
    public Action onFinishMoveBack;

    private void OnValidate()
    {
        if (!_extrapolated) return;

        _translationSpeed = Mathf.Clamp01(_translationSpeed);
        _rotationSpeed = Mathf.Clamp01(_rotationSpeed);
        _scaleSpeed = Mathf.Clamp01(_scaleSpeed);
    }

    private void Start()
    {
        GoToStart();

        onStartMoveTowards += () => gameObject.SetActive(true);
        onFinishMoveBack += () => gameObject.SetActive(false);

        _lineRenderer = GetComponent<LineRenderer>();

        //if (_lineRenderer != null)
        //{
        //    _lineRenderer.useWorldSpace = true;
        //    _lineRenderer.SetPosition(0, _start.position);
        //    _lineRenderer.SetPosition(1, transform.position);
        //}
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Z)) TransformTowards();
        //if (Input.GetKeyDown(KeyCode.V)) TransformBack();

        if (_lineRenderer == null) return;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.SetPosition(0, _start.position);
        _lineRenderer.SetPosition(1, transform.position);
    }

    public void ToggleSelect()
    {
        IsSelected = !IsSelected;
    }

    public void Select()
    {
        IsSelected = true;
    }


    public void Deselect()
    {
        IsSelected = false;
    }

    private void GoToStart()
    {
        if (_start == null) return;
        transform.position = _start.position;
        transform.rotation = _start.rotation;
        transform.localScale = _start.lossyScale;
    }

    private void GoToEnd()
    {
        if (_end == null) return;
        transform.position = _end.position;
        transform.rotation = _end.rotation;
        transform.localScale = _end.lossyScale;
    }

    public void TransformTowards()
    {
        if (_end == null) return;

        if (onStartMoveTowards != null)
            onStartMoveTowards();

        StartCoroutine(MyCoroutines.DoUntil(
            () => transform.position != _end.position &&
                  (!_followRotation || transform.rotation != _end.rotation) &&
                  transform.localScale != _end.lossyScale,
            () =>
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, _end.position, _translationSpeed * Time.deltaTime);
                if (_followRotation)
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation, _end.rotation, _rotationSpeed * Time.deltaTime);
                transform.localScale = Vector3.MoveTowards(
                    transform.localScale, _end.lossyScale, _scaleSpeed * Time.deltaTime);
            },
            onFinishMoveTowards));
    }

    public void TransformBack()
    {
        if (_start == null) return;

        if (onStartMoveBack != null)
            onStartMoveBack();

        StartCoroutine(MyCoroutines.DoUntil(
            () => transform.position != _start.position &&
                  (!_followRotation || transform.rotation != _start.rotation) &&
                  transform.localScale != _start.lossyScale,
            () =>
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, _start.position, _translationSpeed * Time.deltaTime);
                if (_followRotation)
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation, _start.rotation, _rotationSpeed * Time.deltaTime);
                transform.localScale = Vector3.MoveTowards(
                    transform.localScale, _start.lossyScale, _scaleSpeed * Time.deltaTime);
            },
            onFinishMoveBack));
    }

    public void LerpTowards()
    {
        if (_end == null) return;

        if (onStartMoveTowards != null)
            onStartMoveTowards();

        StartCoroutine(MyCoroutines.DoUntil(
            () => transform.position != _end.position &&
                  (!_followRotation || transform.rotation != _end.rotation) &&
                  transform.localScale != _end.lossyScale,
            () =>
            {
                transform.position = Lerp(
                    transform.position, _end.position, _translationSpeed * Time.deltaTime);
                if (_followRotation)
                    transform.rotation = Lerp(
                        transform.rotation, _end.rotation, _rotationSpeed * Time.deltaTime);
                transform.localScale = Lerp(
                    transform.localScale, _end.lossyScale, _scaleSpeed * Time.deltaTime);
            },
            onFinishMoveTowards));
    }

    public void LerpBack()
    {
        if (_start == null) return;

        if (onStartMoveBack != null)
            onStartMoveBack();

        StartCoroutine(MyCoroutines.DoUntil(
            () => transform.position != _start.position &&
                  (!_followRotation || transform.rotation != _start.rotation) &&
                  transform.localScale != _start.lossyScale,
            () =>
            {
                transform.position = Lerp(
                    transform.position, _start.position, _translationSpeed * Time.deltaTime);
                if (_followRotation)
                    transform.rotation = Lerp(
                        transform.rotation, _start.rotation, _rotationSpeed * Time.deltaTime);
                transform.localScale = Lerp(
                    transform.localScale, _start.lossyScale, _scaleSpeed * Time.deltaTime);
            },
            onFinishMoveBack));
    }

    private static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        a = Vector3.Slerp(a, b, t);

        if ((a - b).sqrMagnitude <= Mathf.Epsilon) a = b;

        return a;
    }

    private static Quaternion Lerp(Quaternion a, Quaternion b, float t)
    {
        a = Quaternion.Slerp(a, b, t);

        if (Quaternion.Angle(a, b) <= 1) a = b;

        return a;
    }
}
