using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SolarPanelDirtWorldSpaceUI : MonoBehaviour
{
    private float _height = 0;
    private float _currentDirt = 0;
    private float _targetDirt = 0;
    private Tweener _tweener = null;

    [SerializeField] private RectTransform _filler = null;

    [SerializeField] private float _maxWidth = 100;

    [SerializeField] private Text _text = null;

    [SerializeField] private TweenElement _tweening = new TweenElement(1, Ease.Linear);


    // Use this for initialization
    private void Start()
    {
        Debug.Assert(_filler != null, "Assign Rect Transform filler");
        Debug.Assert(_text != null, "Assign Text");

        _height = _filler.sizeDelta.y;
    }

    private void OnEnable()
    {
        SolarPanel.onDirtChange += OnDirtChangeHandler;
    }

    private void OnDisable()
    {
        if (SolarPanel.onDirtChange != null)
            SolarPanel.onDirtChange -= OnDirtChangeHandler;
    }

    private void OnDirtChangeHandler(float dirt)
    {
        _targetDirt = dirt;

        if (_tweener == null)
        {
            _tweener = DOTween.To(() => _currentDirt, x => _currentDirt = x, _targetDirt, _tweening.duration)
                .SetEase(_tweening.ease).SetAutoKill(false);
            return;
        }

        _tweener.ChangeEndValue(_targetDirt, _tweening.duration, true).SetEase(_tweening.ease).Restart();
    }

    private void LateUpdate()
    {
        UpdateBar();
        UpdateText();
    }

    private void UpdateText()
    {
        if (_text == null) return;
        int final = (int)(_currentDirt * 100);
        _text.text = final + "%";
    }

    private void UpdateBar()
    {
        if (_filler == null) return;
        _filler.sizeDelta = new Vector2(_currentDirt * _maxWidth, _height);
    }
}
