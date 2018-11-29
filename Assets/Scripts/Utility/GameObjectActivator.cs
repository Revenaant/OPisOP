using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActivator : MonoBehaviour
{
    private enum StartState { DoNothing, Activate, Deactivate }
    private bool _isActivated = false;

    [SerializeField] private StartState _doAtStart = StartState.Deactivate;
    [SerializeField] private GameObject _toDeactivate = null;
    [SerializeField] private GameObject _toActivate = null;

    public bool IsActivated
    {
        get { return _isActivated; }
        set
        {
            if (_isActivated != value)
            {
                if (_toDeactivate != null)
                    _toDeactivate.SetActive(!value);
                if (_toActivate != null)
                    _toActivate.SetActive(value);
            }

            _isActivated = value;
        }
    }

    private void Start()
    {
        if (_doAtStart == StartState.DoNothing)
            IsActivated = _isActivated;

        if (_doAtStart == StartState.Deactivate)
        {
            IsActivated = true;
            IsActivated = false;
        }

        if (_doAtStart == StartState.Activate)
        {
            IsActivated = false;
            IsActivated = true;
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
