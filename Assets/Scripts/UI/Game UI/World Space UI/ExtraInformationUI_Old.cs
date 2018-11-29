using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraInformationUI_Old : MoveBackAndForth
{
    [Header("Parents")]
    [SerializeField] private Transform _targetToParent = null;

    [SerializeField] private bool _keepWorldTransformOnStart = false;
    [SerializeField] private Transform _parentOnStart = null;

    [SerializeField] private bool _keepWorldTransformOnEnd = false;
    [SerializeField] private Transform _parentOnEnd = null;

    public override bool Triggered
    {
        get { return base.Triggered; }
        set
        {
            base.Triggered = value;

            //Debug.Log("Hi?");
            //if (value)
            //{
            //    _targetToParent.SetParent(_parentOnStart, _keepWorldTransformOnStart);
            //    _targetToParent.SetAsFirstSibling();
            //    //_targetToParent.Rotate(0, 180, 0);
            //}
            //else
            //{
            //    _targetToParent.SetParent(_parentOnEnd, _keepWorldTransformOnEnd);
            //    _targetToParent.SetAsFirstSibling();
            //    //_targetToParent.Rotate(0, -180, 0);
            //}
        }
    }

    public override void ToggleSelect()
    {
        Triggered = !Triggered;
    }

    public void Unparent()
    {
        _targetToParent.SetParent(_parentOnStart, _keepWorldTransformOnStart);
        _targetToParent.SetAsFirstSibling();
        _targetToParent.Rotate(0, -180, 0);
    }

    public void Parent()
    {
        _targetToParent.SetParent(_parentOnEnd, _keepWorldTransformOnEnd);
        _targetToParent.SetAsFirstSibling();
        _targetToParent.Rotate(0, 180, 0);
    }
}
