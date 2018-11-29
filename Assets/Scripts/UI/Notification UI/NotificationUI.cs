using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    protected Sequence jiggle = null;
    protected Tweener move = null;

    protected float jiggleInterval = 10;
    protected bool rotatingLeft = true;
    protected float moveSpeed = 0;
    protected Quaternion startRotation = Quaternion.identity;
    protected Quaternion leftRotation = Quaternion.identity;
    protected Quaternion rightRotation = Quaternion.identity;

    public Action onActivation;
    public Action onDismissal;
    public Notification notification = null;

    protected float distanceMagnitudeSqr = 1;
    //protected Vector3 startPosition = Vector3.zero;
    //protected Vector3 activatedPosition = Vector3.zero;

    protected NotificationPanelUI panel = null;
    protected NotificationUIManager notificationManager = null;

    /*[SerializeField, ReadOnly]*/
    //protected Transform focus = null;
    protected TargetFocus focus = null;

    [Header("In View and getting there")]
    [SerializeField, ReadOnly] protected bool inView = false;
    /*[SerializeField, ReadOnly]*/
    protected Transform startSpot = null;
    /*[SerializeField, ReadOnly]*/
    protected Transform inViewSpot = null;
    [SerializeField] protected float moveTime = 1;

    [Header("Jiggle")]
    [SerializeField] protected float jiggleCooldown = 5;
    [SerializeField, ReadOnly] protected bool jiggling = false;
    [SerializeField] protected float jiggleRange = 30;
    [SerializeField] protected int movesPerJiggle = 5;
    [SerializeField, ReadOnly] protected float moveDuration = 0.1f;
    [SerializeField] protected float jiggleDuration = 0.5f;

    //[Header("Images")]
    //[ReadOnly, SerializeField]
    protected List<Image> images = new List<Image>();

    public NotificationPanelUI Panel
    {
        get { return panel; }
        set
        {
            panel = value;

            UpdatePositioning();
        }
    }

    private void UpdatePositioning()
    {
        if (panel == null) return;

        //startPosition = panel.start.localPosition;
        startSpot = panel.start;
        inViewSpot = panel.end;
        //activatedPosition = inViewSpot.localPosition;

        distanceMagnitudeSqr = (inViewSpot.localPosition - startSpot.localPosition).sqrMagnitude;
    }

    public NotificationUIManager NotificationManager
    {
        get { return notificationManager; }
        set { notificationManager = value; }
    }

    public bool InView
    {
        get { return inView; }
        set { inView = value; }
    }

    public Transform InViewSpot
    {
        get { return inViewSpot; }
        set { inViewSpot = value; }
    }

    public TargetFocus Focus
    {
        get { return focus; }
        set { focus = value; }
    }

    public Action onFinishMoveTowards;
    public Action onFinishMoveBack;

    protected virtual void Start()
    {
        //if (startSpot == null)
        //{
        //    //startSpot = new RectTransform
        //    //{
        //    //    localPosition = transform.localPosition,
        //    //    localRotation = transform.localRotation,
        //    //    localScale = transform.localScale
        //    //};
        //}
        //startPosition = transform.localPosition;
        //if (inViewSpot == null)
        //{
        //    activatedPosition = startPosition;
        //    activatedPosition.x += -100;
        //}
        //else activatedPosition = inViewSpot.localPosition;

        //distanceMagnitudeSqr = (activatedPosition - startPosition).sqrMagnitude;

        jiggleInterval = jiggleCooldown;

        startRotation = transform.localRotation;

        UpdateJiggleInfo();

        images = GetComponentsInChildren<Image>().ToList();
        //Debug.Log(images.Count);
    }

    protected void OnEnable()
    {
        onFinishMoveTowards += Jiggle;
    }

    protected void OnDisable()
    {
        if (jiggle != null)
            jiggle.Complete(true);

        if (move != null)
            move.Complete(true);

        if (onFinishMoveTowards != null)
            onFinishMoveTowards -= Jiggle;
    }

    protected void OnDestroy()
    {
        if (notification == null) return;

        if (notification.onTrigger != null)
            notification.onTrigger -= onActivation;
        if (notification.onEnd != null)
            notification.onEnd -= onDismissal;
    }

    //[SerializeField, ReadOnly] private float amount = 1;
    protected virtual void Update()
    {
        float amount = 1;
        if (startSpot != null && Math.Abs(distanceMagnitudeSqr) > Mathf.Epsilon)
            amount = (transform.localPosition - startSpot.localPosition).sqrMagnitude / distanceMagnitudeSqr;

        foreach (Image image in images)
            if (image != null)
                image.DOFade(amount, 0.01f);

        jiggleInterval -= Time.deltaTime;
        if (jiggleInterval > 0) return;
        Jiggle();
        jiggleInterval = jiggleCooldown;
        //if ()
        //if (Input.GetKeyDown(KeyCode.B)) Jiggle();
        //if (Input.GetKeyDown(KeyCode.Z)) MoveTowards();
        //if (Input.GetKeyDown(KeyCode.V)) MoveBack();
    }

    protected void UpdateJiggleInfo()
    {
        //1 Move is equal to the halves needed for start and end (going to one extreme and then going back to the start)
        //Check Jiggle() for visualization
        moveDuration = jiggleDuration / (movesPerJiggle + 1);
        moveSpeed = jiggleRange / moveDuration;

        leftRotation = Quaternion.AngleAxis(-jiggleRange, transform.forward);
        rightRotation = Quaternion.AngleAxis(jiggleRange, transform.forward);
    }

    public void GoToStart()
    {
        transform.localPosition = startSpot.localPosition;
    }

    //public void GoToFocus()
    //{
    //    if (GameManager.Instance == null || GameManager.Instance.MainCamera == null) return;

    //    CameraControls cc = GameManager.Instance.MainCamera.GetComponent<CameraControls>();
    //    if (cc != null) cc.MoveFocus(focus);
    //}

    public void Activate()
    {
        notificationManager.Activate(this);
        if (onActivation != null)
            onActivation();
    }

    public void Trigger()
    {
        notificationManager.Trigger(this);
    }

    public void Dismiss()
    {
        notificationManager.Dismiss(this);
        if (onDismissal != null)
            onDismissal();
    }

    public void MoveTowards()
    {
        move = transform.DOLocalMove(inViewSpot.localPosition, moveTime).OnComplete(() =>
        {
            if (onFinishMoveTowards != null)
                onFinishMoveTowards();
            move = null;
        });

        //StartCoroutine(MyCoroutines.DoUntil(
        //    () => transform.localPosition != activatedPosition,
        //    () =>
        //    {
        //        transform.localPosition = Vector3.MoveTowards(
        //            transform.localPosition, activatedPosition, speed * Time.deltaTime);
        //    },
        //    onFinishMoveTowards));
    }

    public void MoveBack()
    {
        move = transform.DOLocalMove(startSpot.localPosition, moveTime).OnComplete(() =>
        {
            if (onFinishMoveBack != null)
                onFinishMoveBack();
            move = null;
        });

        //StartCoroutine(MyCoroutines.DoUntil(
        //    () => transform.localPosition != startPosition,
        //    () =>
        //    {
        //        transform.localPosition = Vector3.MoveTowards(
        //            transform.localPosition, startPosition, speed * Time.deltaTime);
        //    },
        //    onFinishMoveBack));
    }

    public void Jiggle()
    {
        if (jiggling || jiggle != null) return;
        UpdateJiggleInfo();
        jiggling = true;

        jiggle = DOTween.Sequence().Append(transform.DORotateQuaternion(leftRotation, moveDuration / 2));
        for (int i = 0; i <= movesPerJiggle; i++)
        {
            Quaternion rotation = i % 2 == 0 ? rightRotation : leftRotation;
            jiggle.Append(transform.DORotateQuaternion(rotation, moveDuration));
        }

        jiggle.Append(transform.DORotateQuaternion(startRotation, moveDuration / 2));
        jiggle.onComplete += () =>
        {
            jiggling = false;
            jiggle = null;
        };
        //DOTween.
        //StartCoroutine(Move(0, rotatingLeft ? leftRotation : rightRotation, moveSpeed, movesPerJiggle));
    }

    //private IEnumerator Move(float startTime, Quaternion targetRotation, float maxDelta, int movesLeft, bool final = false)
    //{
    //    //JigglePosition target = GetJiggleTarget(targetRotation);
    //    yield return new WaitForSeconds(0);

    //    while (transform.localRotation != targetRotation)
    //    {
    //        transform.localRotation =
    //            Quaternion.RotateTowards(transform.localRotation, targetRotation, maxDelta * Time.deltaTime);
    //        yield return null;
    //    }

    //    if (!final)
    //    {
    //        movesLeft -= 2;
    //        if (movesLeft <= 0)
    //            StartCoroutine(Move(moveDuration, startRotation, maxDelta, movesLeft, true));
    //        else
    //            StartCoroutine(Move(moveDuration * 2, movesLeft % 4 == 0 ? leftRotation : rightRotation, maxDelta,
    //                movesLeft));

    //        //Debug.Log("Finished move");
    //        yield return null;
    //    }
    //    else
    //    {
    //        jiggling = false;
    //    }
    //}
}
