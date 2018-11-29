using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlideSnap : MonoBehaviour
{
    //private bool _invokedMaxMinEvents = false;
    private const float EPSILON_F = 0.01f;

    private Slider slider;
    private Image image;
    private Material OnMat;

    [SerializeField, ReadOnly]
    private float targetValue;
    private bool isHeld;
    private float oldValue;

    private EventTrigger eventer;

    [SerializeField] private float lerpSpeed = 10;

    [SerializeField] private Material OffMat;

    public System.Action OnToggle;

    public UnityEvent OnMaxSliderValue;
    public UnityEvent OnMinSliderValue;

    public Action onMaxSliderValue;
    public Action onMinSliderValue;

    // Use this for initialization
    private void Start()
    {
        #region SetupEvents
        eventer = GetComponent<EventTrigger>();

        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        //EventTrigger.Entry onDragEntry = new EventTrigger.Entry();
        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        EventTrigger.Entry clickEntry = new EventTrigger.Entry();

        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        //onDragEntry.eventID = EventTriggerType.Drag;
        endDragEntry.eventID = EventTriggerType.EndDrag;
        clickEntry.eventID = EventTriggerType.PointerClick;

        beginDragEntry.callback.AddListener((data) => { OnBeginDrag((PointerEventData)data); });
        //onDragEntry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        endDragEntry.callback.AddListener((data) => { OnEndDrag((PointerEventData)data); });
        clickEntry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });

        eventer.triggers.Add(beginDragEntry);
        eventer.triggers.Add(endDragEntry);
        eventer.triggers.Add(clickEntry);

        #endregion

        image = GetComponentInChildren<Image>();
        OnMat = image.material;
        image.material = OffMat;

        slider = GetComponent<Slider>();
        targetValue = slider.value;

        slider.onValueChanged.AddListener(value =>
        {
            if (Mathf.Abs(value - slider.maxValue) <= EPSILON_F)
            {
                if (onMaxSliderValue != null)
                    onMaxSliderValue.Invoke();
                OnMaxSliderValue.Invoke();
                image.material = OnMat;
            }

            if (Mathf.Abs(value - slider.minValue) <= EPSILON_F)
            {
                if (onMinSliderValue != null)
                    onMinSliderValue.Invoke();
                OnMinSliderValue.Invoke();
                image.material = OffMat;
            }
        });
    }

    private void Update()
    {
        if (!IsHeld)
        {
            slider.value = Mathf.Lerp(slider.value, targetValue, lerpSpeed * Time.deltaTime);
            //slider.value = Mathf.MoveTowards(slider.value, targetValue, lerpSpeed);
        }
        else targetValue = slider.value;
    }

    public void OnPointerClick(PointerEventData data)
    {
        Click();
    }

    public void Click()
    {
        //Debug.Log("Yo, work!");
        Toggle();
        OnToggle.Invoke();

        if (Math.Abs(oldValue - TargetValue) > EPSILON_F)
        {
            oldValue = TargetValue;

            SwapMaterials();
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        isHeld = true;
        //Debug.Log("Hmmmm");
    }

    //public void OnDrag(PointerEventData data)
    //{
    //    oldValue = TargetValue = slider.value;
    //}

    public void OnEndDrag(PointerEventData data)
    {
        isHeld = false;
        Snap();
        OnToggle.Invoke();

        if (Math.Abs(oldValue - TargetValue) > EPSILON_F)
        {
            oldValue = TargetValue;

            SwapMaterials();
        }
    }

    private void SwapMaterials()
    {
        if (image.material == OnMat)
            image.material = OffMat;
        else if (image.material == OffMat)
            image.material = OnMat;
    }

    public void Snap()
    {
        targetValue = Mathf.Round(slider.value);
    }

    public void Toggle()
    {
        //Debug.Log("Target Value: " + targetValue);
        //Debug.Log("Current Value: " + slider.value);
        if (Math.Abs(slider.value - slider.minValue) < EPSILON_F)
            targetValue = slider.maxValue;
        if (Math.Abs(slider.value - slider.maxValue) < EPSILON_F)
            targetValue = slider.minValue;
    }

    public bool IsHeld
    {
        get { return isHeld; }
        set { isHeld = value; }
    }

    public float TargetValue
    {
        get { return targetValue; }
        set { targetValue = value; }
    }
}
