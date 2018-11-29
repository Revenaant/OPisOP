using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TouchScript;

[System.Serializable]
public class TweenElement
{
    public float duration;
    public Ease ease;

    public TweenElement(float duration, Ease ease)
    {
        this.duration = duration;
        this.ease = ease;
    }
}

public class ActionPanel : MonoBehaviour
{

    [SerializeField, ReadOnly] private bool _activated = false;

    [SerializeField] private TargetFocus _targetFocus = null;
    [SerializeField] private Transform _fadeInPanel = null;
#pragma warning disable 0414
    [SerializeField] private Transform _transform = null;

    //[SerializeField] private float _offsetY = 3;
    [SerializeField] private TweenElement _moveActivate = new TweenElement(0.7f, Ease.Linear);
    [SerializeField] private TweenElement _moveDeactivate = new TweenElement(0.7f, Ease.Linear);
#pragma warning restore 0414
    [SerializeField] private TweenElement _fadeActivate = new TweenElement(0.7f, Ease.Linear);
    [SerializeField] private TweenElement _fadeDeactivate = new TweenElement(0.7f, Ease.Linear);

    //[SerializeField] private Collider _collider = null;

    private Image[] _images;
    private Text[] _texts;
    //private Sequence _sequence = null;
    private Sequence _sequenceForward = null;
    private Sequence _sequenceBackward = null;

    //private CameraControls _cameraControls = null;

    private EventSystem _eventSystem;
    private GraphicRaycaster _graphicRaycaster;

    public Action onMinimize;
    public UnityEvent OnMinimize;

    //public Action onTriggerValueChanged;
    //public UnityEvent OnTriggerValueChanged;

    public bool Activated
    {
        get { return _activated; }
        set
        {
            if (value != _activated/* && _sequence != null*/)
            {
                if (value)
                {
                    gameObject.SetActive(true);

                    if (_sequenceForward != null)
                    {
                        _sequenceForward.Play();
                        _sequenceForward.Restart();
                    }

                    if (_targetFocus != null)
                    {
                        _targetFocus.Focus();
                        //_targetFocus.ActivateActionPanel();
                    }
                    //if (_collider != null) _collider.enabled = true;
                    //_sequence.PlayForward();
                }
                else
                {
                    //_sequence.PlayBackwards();
                    if (_sequenceBackward != null)
                    {
                        _sequenceBackward.Play();
                        _sequenceBackward.Restart();
                    }

                    if (_targetFocus != null)
                    {
                        //_targetFocus.DeactivateActionPanel();
                        _targetFocus.Unfocus();
                    }

                    //if (_collider != null) _collider.enabled = false;
                }
            }

            _activated = value;
        }
    }

    private void OnEnable()
    {
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersPressed += PressedHandler;
            //TouchManager.Instance.PointersReleased += PressedHandler;
            //TouchManager.Instance.PointersReleased += PressedHandler2;
            //TouchManager.Instance.PointersPressed += PressedHandler2;
        }

        if (GameManager.Instance.CheckpointManager != null)
            GameManager.Instance.CheckpointManager.OnCheckpointStart.AddListener(Minimize);
    }

    //private EventHandler< PointerEventArgs> lambda = null;
    // Use this for initialization
    private void Start()
    {
        _images = _fadeInPanel.GetComponentsInChildren<Image>();
        _texts = _fadeInPanel.GetComponentsInChildren<Text>();
        //if (_collider == null) _collider = GetComponent<Collider>();

        // Shit for making it go away clicking somewhere else
        _eventSystem = FindObjectOfType<EventSystem>();
        _graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        //if (lambda == null)
        //    lambda = (object sender, PointerEventArgs E) => CheckRay(E.Pointers[0].Position);
        //TouchManager.Instance.PointersPressed += PressedHandler;

        //_sequence = DOTween.Sequence()
        //    .Append(_transform.DOLocalMoveY(_offsetY, _moveActivate.duration).SetRelative().SetEase(_moveActivate.ease))
        //    .Insert(0, FallAllToSequence(1, _fadeActivate.duration, _fadeActivate.ease))
        //    .Pause().SetAutoKill(false);

        //_sequence.PlayBackwards();

        _sequenceForward = DOTween.Sequence();
        _sequenceForward
            //.Append(_transform.DOLocalMoveY(_offsetY, _moveActivate.duration).SetRelative().SetEase(_moveActivate.ease))
            .AppendInterval(_fadeActivate.duration)
            .InsertCallback(0, () => FadeAllTo(1, _fadeActivate.duration, _fadeActivate.ease));
        _sequenceForward.Pause().SetAutoKill(false);

        _sequenceBackward = DOTween.Sequence();
        _sequenceBackward
            .AppendCallback(() => FadeAllTo(0, _fadeDeactivate.duration, _fadeDeactivate.ease))
            //.Append(_transform.DOLocalMoveY(-_offsetY, _moveDeactivate.duration).SetRelative().SetEase(_moveDeactivate.ease)
            .AppendInterval(_fadeDeactivate.duration);
        _sequenceBackward.Pause().SetAutoKill(false);
        _sequenceBackward.onComplete += () => { gameObject.SetActive(false); };

        _sequenceBackward.Play();
        _sequenceBackward.Restart();

        //if (_collider != null) _collider.enabled = false;

        //StartCoroutine(MyCoroutines.WaitOneFrame(() =>
        //{
        //    _cameraControls = GameManager.Instance.CameraControls;

        //    Debug.Assert(_cameraControls != null, "Can't find Camera Controls.");
        //}));
    }

    private void PressedHandler(object sender, PointerEventArgs pointerEventArgs)
    {
        if (!isActiveAndEnabled) return;

        var point = pointerEventArgs.Pointers[0].Position;
        StartCoroutine(MyCoroutines.Wait(0.01f, () =>
            CheckRay(point)));
        //Debug.Log("Pos: " + pointer);
        //CheckRay(point);
    }

    private void PressedHandler2(object sender, PointerEventArgs pointerEventArgs)
    {
        CheckRay(pointerEventArgs.Pointers[0].Position);
    }

    private void OnDisable()
    {
        //StopAllCoroutines();
        if (TouchManager.Instance != null)
        {
            TouchManager.Instance.PointersPressed -= PressedHandler;
            TouchManager.Instance.PointersPressed -= PressedHandler2;
        }
        //if (TouchManager.Instance != null && lambda != null)
        //    TouchManager.Instance.PointersPressed -= lambda;

        if (GameManager.Instance.CheckpointManager != null)
            GameManager.Instance.CheckpointManager.OnCheckpointStart.RemoveListener(Minimize);
    }

    private void OnDestroy()
    {
        //StopAllCoroutines();
        //if (TouchManager.Instance != null)
        //    TouchManager.Instance.PointersPressed -= PressedHandler;
        //if (TouchManager.Instance != null && lambda != null)
        //    TouchManager.Instance.PointersPressed -= lambda;
    }

    private void FadeAllTo(float alpha, float duration, Ease ease)
    {

        foreach (Image image in _images)
        {
            image.DOFade(alpha, duration).SetEase(ease).SetAutoKill(true).SetRecyclable();
        }

        foreach (Text text in _texts)
        {
            text.DOFade(alpha, duration).SetEase(ease).SetAutoKill(true).SetRecyclable();
        }
    }

    private Sequence FallAllToSequence(float alpha, float duration, Ease ease)
    {
        Sequence sequence = DOTween.Sequence();
        foreach (Image image in _images)
        {
            sequence.Insert(0, image.DOFade(alpha, duration).SetEase(ease));
        }

        foreach (Text text in _texts)
        {
            sequence.Insert(0, text.DOFade(alpha, duration).SetEase(ease));
        }

        return sequence;
    }

    //bool did = false;
    private void CheckRay(Vector3 position)
    {
        // So it doesn't immediately deactivate
        if (!isActiveAndEnabled) return;
        //if (did == false)
        //{
        //    did = true;
        //    return;
        //}

        // Check if it hits outside itself
        if (!GetUIRayHit(position))
        {
            Minimize();
        }

        //StartCoroutine(MyCoroutines.WaitOneFrame(() => { }));
    }

    private Coroutine _activation;
    private Coroutine _deactivation;

    public void Minimize()
    {
        //Debug.Log("Something minimized");
        Activated = false;
        //did = false;

        OnMinimizeInvoke();
    }

    public void ToggleActivation()
    {
        Activated = !Activated;
    }

    public void Activate()
    {
        gameObject.SetActive(true);

        if (_activation != null)
            StopCoroutine(_activation);
        if (_deactivation != null)
            StopCoroutine(_deactivation);

        _activation = StartCoroutine(MyCoroutines.WaitXFrames(2, () => Activated = true));
        //StartCoroutine(MyCoroutines.Wait(0.03f, () => Activated = true));
        //Activated = true;
        //_sequence.Restart();
    }

    public void Deactivate()
    {
        _deactivation = StartCoroutine(MyCoroutines.WaitXFrames(2, () => Activated = false));
        //StartCoroutine(MyCoroutines.Wait(0.03f, () => Activated = false));
        //Activated = false;
        //_sequence.Restart();
        //_sequence.PlayBackwards();
    }

    public void OnMinimizeInvoke()
    {
        if (onMinimize != null)
            onMinimize();

        OnMinimize.Invoke();
        //Debug.Log("Minimize");
    }

    public void TriggerValueChange()
    {
        //if (onTriggerValueChanged != null)
        //    onTriggerValueChanged();

        //OnTriggerValueChanged.Invoke();
    }

    private bool GetUIRayHit(Vector3 pos)
    {
        if (_graphicRaycaster == null)
            _graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        if (_graphicRaycaster == null)
        {
            Debug.LogWarning("Graphics Raycaster is null.");
            return false;
        }

        PointerEventData pData = new PointerEventData(_eventSystem) { position = pos };
        //pData.position = pos;

        // Do some raycasting shiz
        List<RaycastResult> results = new List<RaycastResult>();
        _graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        _graphicRaycaster.Raycast(pData, results);
        //bool debug = true;
        bool debug = false;
        if (debug) print("Waow");
        if (results.Count > 0)
        {
            if (debug) print("Waow > 0");
            // Get all children of this object
            Transform[] allChild = gameObject.GetComponentsInChildren<Transform>();

            // Loop through all the rayHit results
            foreach (var result in results)
            {
                //LayerMask i = LayerMask.NameToLayer("Untouchable UI");
                //if (result.gameObject.layer == i) continue;
                // If it's this gameobject don't deactivate
                if (result.gameObject == this.gameObject)
                    return true;

                if (debug) print("Waow Same");
                // If it's any of the childs, don't deactivate
                foreach (var child in allChild)
                {
                    if (result.gameObject == child.gameObject)
                    {
                        if (debug) print("Waow True");
                        return true;
                    }
                }

                //// Else deactivate
                if (debug) print("fail");
                //return false;
            }
        }
        else
        {
            if (debug) print("Waow None");
            // If there are no hits to the UI, deactivate
            return false;
        }

        return false;
    }
}
