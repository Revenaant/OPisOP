using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackAndForth : MonoBehaviour
{
    [System.Serializable]
    protected class Element
    {
        public bool follow = true;
        public float speed = 25;
    }

    [System.Serializable]
    protected class ElementVector : Element
    {
        public bool overwrite = false;
        public Vector3 overwriteValue = Vector3.zero;
    }

    [System.Serializable]
    protected class ElementQuaternion : Element
    {
        public bool overwrite = false;
        public Quaternion overwriteValue = Quaternion.identity;
    }

    [SerializeField]
    protected int isDoneCount = 0;
    [SerializeField]
    protected int maxIsDoneCount = 0;

    protected Coroutine translationCoroutine = null;
    protected Coroutine rotationCoroutine = null;
    protected Coroutine scalingCoroutine = null;

    [SerializeField, ReadOnly] protected bool triggered = false;
    [SerializeField] protected bool interpolation = false;

    [Header("Thing to move")]
    [SerializeField] protected Transform target = null;
    [SerializeField] protected Element translation = new Element();
    [SerializeField] protected Element rotation = new Element();
    [SerializeField] protected Element scale = new Element();

    [Header("Positions to move to")]
    [SerializeField] protected Transform start = null;
    [SerializeField] protected Transform end = null;

    public virtual bool Triggered
    {
        get { return triggered; }
        set
        {
            triggered = value;
            if (onTrigger != null)
                onTrigger(value);

            if (interpolation)
            {
                if (value) Lerp(end, onStartMoveTowards, onFinishMoveTowards);
                else Lerp(start, onStartMoveBack, onFinishMoveBack);
            }
            else
            {
                if (value) Transform(end, onStartMoveTowards, onFinishMoveTowards);
                else Transform(start, onStartMoveBack, onFinishMoveBack);
            }
        }
    }

    public Action<bool> onTrigger;
    public Action onStartMoveTowards;
    public Action onStartMoveBack;
    public Action onFinishMoveTowards;
    public Action onFinishMoveBack;

    protected virtual void OnValidate()
    {
        if (!interpolation) return;

        translation.speed = Mathf.Clamp01(translation.speed);
        rotation.speed = Mathf.Clamp01(rotation.speed);
        scale.speed = Mathf.Clamp01(scale.speed);
    }

    protected virtual void Start()
    {
        GoToStart();
        //onFinishMoveTowards += () => Debug.Log("I STOP");
    }

    protected virtual void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Z)) Transform(end, onStartMoveTowards, onFinishMoveTowards);
        //if (Input.GetKeyDown(KeyCode.V)) Transform(start, onStartMoveBack, onFinishMoveBack);
    }

    public virtual void ToggleSelect()
    {
        Triggered = !Triggered;
    }

    public virtual void Select()
    {
        Triggered = true;
    }


    public virtual void Deselect()
    {
        Triggered = false;
    }

    protected void GoToStart()
    {
        if (start == null) return;
        if (translation.follow) transform.position = start.position;
        if (rotation.follow) transform.rotation = start.rotation;
        if (scale.follow) transform.localScale = start.lossyScale;
    }

    protected void GoToEnd()
    {
        if (end == null) return;
        if (translation.follow) transform.position = end.position;
        if (rotation.follow) transform.rotation = end.rotation;
        if (scale.follow) transform.localScale = end.lossyScale;
    }

    public void Transform(Transform transformTarget, Action onStart, Action onEnd)
    {
        if (transformTarget == null) return;

        if (onStart != null)
            onStart();


        ResetCoroutines();
        //Vector3 targetScale = transform.parent.InverseTransformDirection(transformTarget.lossyScale);

        maxIsDoneCount = 0;

        Vector3 targetScale = transformTarget == end ? Vector3.one : Vector3.zero;

        if (translation.follow)
        {
            maxIsDoneCount++;
            translationCoroutine = StartCoroutine(MyCoroutines.DoWhile(
                () => transform.position != transformTarget.position,
                () => transform.position = Vector3.MoveTowards(transform.position, transformTarget.position,
                    translation.speed * Time.deltaTime),
                () => isDoneCount++
            ));
        }

        if (rotation.follow)
        {
            maxIsDoneCount++;
            rotationCoroutine = StartCoroutine(MyCoroutines.DoWhile(
                () => transform.rotation != transformTarget.rotation,
                () => transform.rotation = Quaternion.RotateTowards(transform.rotation, transformTarget.rotation, rotation.speed * Time.deltaTime),
                () => isDoneCount++
            ));
        }

        if (scale.follow)
        {
            maxIsDoneCount++;
            scalingCoroutine = StartCoroutine(MyCoroutines.DoWhile(
                () => transform.localScale != targetScale,
                () => transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, scale.speed * Time.deltaTime),
                () => isDoneCount++
            ));
        }

        StartCoroutine(MyCoroutines.DoUntil(
            () => isDoneCount == maxIsDoneCount, null, () =>
            {
                if (onEnd != null)
                    onEnd();
                maxIsDoneCount = 0;
            }));

        //transformationCoroutine = StartCoroutine(TransformCoroutine(transformTarget, onEnd));

        //transformationCoroutine = StartCoroutine(MyCoroutines.DoUntil(
        //    () =>
        //            //(translation.follow && transform.position == transformTarget.position)
        //    //&&
        //    //(rotation.follow && transform.rotation == transformTarget.rotation)
        //    //&&
        //    (!scale.follow || transform.localScale == targetScale)
        //    ,
        //    () =>
        //    {
        //        if (translation.follow)
        //            transform.position = Vector3.MoveTowards(
        //                transform.position, transformTarget.position, translation.speed * Time.deltaTime);
        //        //if (rotation.follow)
        //        //    transform.rotation = Quaternion.RotateTowards(
        //        //        transform.rotation, transformTarget.rotation, rotation.speed * Time.deltaTime);
        //        if (scale.follow)
        //            transform.localScale = Vector3.MoveTowards(
        //                transform.localScale, targetScale, scale.speed * Time.deltaTime);
        //    },
        //    onEnd));
    }

    protected void ResetCoroutines()
    {
        if (translationCoroutine != null)
        {
            StopCoroutine(translationCoroutine);
            translationCoroutine = null;
        }

        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }

        if (scalingCoroutine != null)
        {
            StopCoroutine(scalingCoroutine);
            scalingCoroutine = null;
        }
    }

    private IEnumerator TransformCoroutine(Transform transformTarget, Action onEnd)
    {
        //Vector3 targetScale = transform.parent.InverseTransformDirection(transformTarget.lossyScale);
        Vector3 targetScale = transformTarget == end ? Vector3.one : Vector3.zero;

        Debug.Log("DO NOT USE");
        while (
               (!translation.follow || transform.position != transformTarget.position) ||
               (!rotation.follow || transform.rotation != transformTarget.rotation) ||
               (!scale.follow || transform.lossyScale != transformTarget.lossyScale))
        {
            //Debug.Log("Hello");
            if (translation.follow)
                transform.position = Vector3.MoveTowards(
                    transform.position, transformTarget.position, translation.speed * Time.deltaTime);
            if (rotation.follow)
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, transformTarget.rotation, rotation.speed * Time.deltaTime);
            if (scale.follow)
                transform.localScale = Vector3.MoveTowards(
                    transform.localScale, targetScale, scale.speed * Time.deltaTime);

            yield return null;
        }

        if (onEnd != null) onEnd.Invoke();

        yield return null;
    }

    public void Lerp(Transform transformTarget, Action onStart, Action onEnd, bool invert = false)
    {
        Debug.LogWarning("DO NOT USE! NOT UPDATED");
        if (transformTarget == null) return;

        if (onStart != null)
            onStart();

        StartCoroutine(MyCoroutines.DoUntil(
            () => (!translation.follow || transform.position != transformTarget.position) &&
                  (!rotation.follow || transform.rotation != transformTarget.rotation) &&
                  (!scale.follow || transform.localScale != transformTarget.lossyScale),
            () =>
            {
                if (translation.follow)
                {
                    float speed = invert ? 1 - translation.speed : translation.speed;
                    transform.position = Lerp(
                        transform.position, transformTarget.position, speed * Time.deltaTime);
                }

                if (rotation.follow)
                {
                    float speed = invert ? 1 - rotation.speed : rotation.speed;
                    transform.rotation = Lerp(
                        transform.rotation, transformTarget.rotation, speed * Time.deltaTime);
                }

                if (scale.follow)
                {
                    float speed = invert ? 1 - scale.speed : scale.speed;
                    transform.localScale = Lerp(
                        transform.localScale, transformTarget.lossyScale, speed * Time.deltaTime);
                }
            },
            onEnd));
    }

    protected static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        a = Vector3.Slerp(a, b, t);

        if ((a - b).sqrMagnitude <= Mathf.Epsilon) a = b;

        return a;
    }

    protected static Quaternion Lerp(Quaternion a, Quaternion b, float t)
    {
        a = Quaternion.Slerp(a, b, t);

        if (Quaternion.Angle(a, b) <= 1) a = b;

        return a;
    }
}
