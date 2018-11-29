using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour {

    [SerializeField] private float _duration;
    [SerializeField, Range(0, 1)] private float _trauma;
    [SerializeField] private Vector3 _shakeScale = new Vector3(5, 5, 5);
    Camera _camera;

    // Use this for initialization
    void Start () {
        _camera = FindObjectOfType<Camera>();
	}

    public void Shake(float duration = 1, float trauma = 1)
    {
        _camera.DOShakeRotation(duration, new Vector3(_shakeScale.x, _shakeScale.y, _shakeScale.z) * trauma, 10, 45, true).SetEase(Ease.InCubic);
    }

    public void Shake()
    {
        _camera.DOShakeRotation(_duration, new Vector3(_shakeScale.x, _shakeScale.y, _shakeScale.z) * _trauma, 10, 45, true).SetEase(Ease.InCubic);
    }

    public float Duration
    {
        get { return _duration; }
        set { _duration = value; }
    }

    public float Trauma
    {
        get { return _trauma; }
        set { _trauma = Mathf.Clamp01(value); }
    }

    public Vector3 ShakeScale
    {
        get { return _shakeScale; }
        set { _shakeScale = value; }
    }
}
