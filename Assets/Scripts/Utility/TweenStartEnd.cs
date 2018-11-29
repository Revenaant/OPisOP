using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TweenStartEnd : MonoBehaviour
{
    private enum OnStart { DoNothing, GoToStart, GoToEnd }

    private Tweener _move = null;

    [SerializeField] private OnStart _onStart = OnStart.GoToStart;
    [SerializeField, ReadOnly] private bool _isActivated = false;

    [SerializeField] private Transform _start = null;
    [SerializeField] private Transform _end = null;

    //[SerializeField] private float _duration = 1;
    //[SerializeField] private Ease _easing = Ease.Linear;

    [SerializeField] private TweenElement _activation = new TweenElement(0.5f, Ease.Linear);
    [SerializeField] private TweenElement _deactivation = new TweenElement(0.1f, Ease.Linear);

    public bool IsActivated
    {
        get { return _isActivated; }
        set
        {
            if (_isActivated != value)
            {
                if (_move == null)
                    _move = transform.DOLocalMove(value ? _end.localPosition : _start.localPosition, _activation.duration, true)
                        .SetEase(value ? _activation.ease : _deactivation.ease).SetAutoKill(false);
                else
                    _move.ChangeEndValue(value ? _end.localPosition : _start.localPosition, _deactivation.duration, true)
                        .SetEase(value ? _activation.ease : _deactivation.ease).Restart();
            }

            _isActivated = value;
        }
    }

    // Use this for initialization
    private void Start()
    {
        switch (_onStart)
        {
            default:
            case OnStart.DoNothing:
                {
                    IsActivated = _isActivated;
                    break;
                }
            case OnStart.GoToStart:
                {
                    IsActivated = true;
                    IsActivated = false;
                    break;
                }
            case OnStart.GoToEnd:
                {
                    IsActivated = false;
                    IsActivated = true;
                    break;
                }
        }
    }

    public void Toggle()
    {
        IsActivated = !IsActivated;
    }

    public void Activate()
    {
        IsActivated = true;
    }

    public void Deactivate()
    {
        IsActivated = false;
    }
}
