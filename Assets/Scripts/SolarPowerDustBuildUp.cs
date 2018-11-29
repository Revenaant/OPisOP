using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SolarPowerDustBuildUp : MonoBehaviour
{
    private Renderer _renderer = null;
    private Tweener _tweener = null;
    private float _target = 0;
    private float _dust = 0;
    [SerializeField] private float _duration = 1;

    private float Dust
    {
        get { return _dust; }
        set
        {
            _dust = value;
            UpdateDust(_dust);
        }
    }

    // Use this for initialization
    private void OnEnable()
    {
        SolarPanel.onDirtChange += SetTarget;
    }

    private void OnDisable()
    {
        if (SolarPanel.onDirtChange != null)
            SolarPanel.onDirtChange -= SetTarget;
    }

    private void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        UpdateDust(_target);
    }

    private void SetTarget(float dust)
    {
        _target = dust;

        if (_tweener == null) _tweener = DOTween.To(() => Dust, dirt => Dust = dirt, _target, _duration).SetAutoKill(false);
        else _tweener.ChangeEndValue(_target, _duration, true).Restart();
    }

    // Update is called once per frame
    private void UpdateDust(float dust)
    {
        _renderer.material.SetFloat("_SnowSize", dust * (-4) + 2);
    }
}
