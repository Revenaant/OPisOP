using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPanelUI_Old : MoveBackAndForth
{
    private LineRenderer _lineRenderer = null;

    [Header("Action Panel")]
    [SerializeField] private GameObject _gameObjectToDisable = null;
    [SerializeField] private ExtraInformationUI_Old _extraInformationUI = null;

    [SerializeField] private Transform _scaleAnchor = null;

    public override bool Triggered
    {
        get { return base.Triggered; }
        set
        {
            if (_extraInformationUI != null)
                _extraInformationUI.Triggered = value;
            else
                base.Triggered = value;

        }
    }

    protected override void Start()
    {
        //base.Start();

        if (_extraInformationUI != null)
        {
            Debug.Log("Subscribing");
            _scaleAnchor.localScale = new Vector3(1, 0, 1);
            _extraInformationUI.onStartMoveTowards += () =>
            {
                transform.localScale = Vector3.one;
                Debug.Log("Started");
                StartCoroutine(MyCoroutines.DoWhile(
                    () => Math.Abs(_scaleAnchor.localScale.y - 1) > Mathf.Epsilon,
                    () =>
                    {
                        Debug.Log("Doing");
                        Vector3 localScale = _scaleAnchor.localScale;
                        localScale.y = Mathf.MoveTowards(localScale.y, 1, 25 * Time.deltaTime);
                        _scaleAnchor.localScale = localScale;
                    }
                    //,
                    //() =>
                    //{
                    //    _extraInformationUI.Parent();
                    //    base.Triggered = true;
                    //}
                    ));
            };
        }

        if (_gameObjectToDisable != null)
        {
            onStartMoveTowards += () => _gameObjectToDisable.SetActive(true);
            onFinishMoveBack += () => _gameObjectToDisable.SetActive(false);
        }

        _lineRenderer = GetComponentInParent<LineRenderer>();

        //gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (_lineRenderer == null) return;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.SetPosition(0, start.position);
        _lineRenderer.SetPosition(1, transform.position);
    }
}
