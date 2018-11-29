using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayMoveTo : MonoBehaviour
{
    [SerializeField, ReadOnly] protected bool isSelected = false;
    [SerializeField] protected bool extrapolated = false;

    [Header("Thing to move")]
    [SerializeField] protected Transform target = null;
    [SerializeField] protected float translationSpeed = 25;

    [Header("Positions to move to")]
    [SerializeField] protected Transform endRectTransform = null;
    [SerializeField] protected Transform startRectTransform = null;
    
    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            
            if (extrapolated)
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

    protected void OnValidate()
    {
        if (!extrapolated) return;

        translationSpeed = Mathf.Clamp01(translationSpeed);
    }

    protected void Start()
    {
        GoToStart();
    }

    protected void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Z)) TransformTowards();
        //if (Input.GetKeyDown(KeyCode.V)) TransformBack();
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

    protected void GoToStart()
    {
        if (startRectTransform == null) return;
        transform.position = startRectTransform.position;
    }

    protected void GoToEnd()
    {
        if (endRectTransform == null) return;
        transform.position = endRectTransform.position;
    }

    public void TransformTowards()
    {
        if (endRectTransform == null) return;

        if (onStartMoveTowards != null)
            onStartMoveTowards();

        StartCoroutine(MyCoroutines.DoUntil(
            () => transform.position != endRectTransform.position,
            () =>
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, endRectTransform.position, translationSpeed * Time.deltaTime);
            },
            onFinishMoveTowards));
    }

    public void TransformBack()
    {
        if (startRectTransform == null) return;

        if (onStartMoveBack != null)
            onStartMoveBack();

        StartCoroutine(MyCoroutines.DoUntil(
            () => transform.position != startRectTransform.position,
            () =>
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, startRectTransform.position, translationSpeed * Time.deltaTime);
            },
            onFinishMoveBack));
    }

    public void LerpTowards()
    {
        if (endRectTransform == null) return;

        if (onStartMoveTowards != null)
            onStartMoveTowards();

        StartCoroutine(MyCoroutines.DoUntil(
            () => transform.position != endRectTransform.position,
            () =>
            {
                transform.position = Lerp(
                    transform.position, endRectTransform.position, translationSpeed * Time.deltaTime);
            },
            onFinishMoveTowards));
    }

    public void LerpBack()
    {
        if (startRectTransform == null) return;

        if (onStartMoveBack != null)
            onStartMoveBack();

        StartCoroutine(MyCoroutines.DoUntil(
            () => transform.position != startRectTransform.position,
            () =>
            {
                transform.position = Lerp(
                    transform.position, startRectTransform.position, translationSpeed * Time.deltaTime);
            },
            onFinishMoveBack));
    }

    protected static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        a = Vector3.Slerp(a, b, t);

        if ((a - b).sqrMagnitude <= Mathf.Epsilon) a = b;

        return a;
    }
}
