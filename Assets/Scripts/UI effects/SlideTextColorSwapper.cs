using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SlideSnap))]
public class SlideTextColorSwapper : MonoBehaviour
{
    [SerializeField] private Color _color1 = Color.blue;
    [SerializeField] private Color _color2 = Color.gray;

    [SerializeField] private Text[] _Texts;
#pragma warning disable 0414
    private Slider _slider = null;
#pragma warning restore 0414
    private float _oldValue = 0;

    private SlideSnap _slideSnap;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        _slideSnap = GetComponent<SlideSnap>();
        _slideSnap.OnToggle += Toggle;
        _oldValue = 0.5f;
    }

    public void Toggle()
    {
        if (Math.Abs(_oldValue - _slideSnap.TargetValue) > Mathf.Epsilon)
        {
            _oldValue = _slideSnap.TargetValue;

            foreach (Text _Text in _Texts)
            {
                if (_Text.color == _color1)
                    _Text.color = _color2; //Color.Lerp(_color1, _color2, Time.deltaTime);
                else if (_Text.color == _color2)
                    _Text.color = _color1; //Color.Lerp(_color2, _color1, Time.deltaTime);
            }
        }
    }
}
