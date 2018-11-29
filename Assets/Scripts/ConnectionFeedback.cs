using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ConnectionFeedback : MonoBehaviour
{
    private Connection _connection = null;
    private MeshRenderer _meshRenderer = null;

    private Tweener _colorTween = null;
    private Tweener _moveSpeedTween = null;

    [SerializeField, ReadOnly] private Color _connectionColor = Color.cyan;
    [SerializeField, ReadOnly] private float _move = 1;

    [SerializeField] private TweenElement _tween = new TweenElement(1, Ease.Linear);

    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField, Range(0, 1)] private float _normalMoveSpeed = 1;
    [SerializeField] private Color _lowPowerColor = Color.black;
    [SerializeField, Range(0, 1)] private float _lowPowerMoveSpeed = 0.25f;

    // Use this for initialization
    private void Start()
    {
        //_connection = GetComponent<Connection>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        if (_connection == null)
            _connection = GetComponent<Connection>();

        _connection.OnStartTransfer += StartTransferHandler;
    }

    private void OnDisable()
    {
        if (_connection == null) return;

        if (_connection.OnStartTransfer != null)
            _connection.OnStartTransfer -= StartTransferHandler;
    }

    private void Update()
    {
        UpdateConnection();
    }

    private void UpdateConnection()
    {
        if (_meshRenderer == null) return;

        _meshRenderer.material.color = _connectionColor;
        _meshRenderer.material.SetFloat("_uvMove", _move);
    }

    // Update is called once per frame
    private void StartTransferHandler(float amount)
    {
        if (_colorTween == null)
            _colorTween = DOTween.To(() => _connectionColor, x => _connectionColor = x,
                _normalColor, _tween.duration).SetEase(_tween.ease).SetAutoKill(false);

        if (_moveSpeedTween == null)
            _moveSpeedTween = DOTween.To(() => _move, x => _move = x,
                _normalMoveSpeed, _tween.duration).SetEase(_tween.ease).SetAutoKill(false);

        if (amount > 1)
        {
            _colorTween.ChangeEndValue(_normalColor, true).Restart();
            _moveSpeedTween.ChangeEndValue(_normalMoveSpeed, true).Restart();
        }
        else
        {
            _colorTween.ChangeEndValue(_lowPowerColor, true).Restart();
            _moveSpeedTween.ChangeEndValue(_lowPowerMoveSpeed, true).Restart();
        }
    }
}
