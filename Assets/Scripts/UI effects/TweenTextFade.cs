using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TweenTextFade : MonoBehaviour
{
    private bool _fading = true;

    [SerializeField]
    private Text _text = null;

    [SerializeField, Range(0, 1)]
    private float _maxTransparency = 1;
    [SerializeField, Range(0, 1)]
    private float _minTransparency = 0;

    [SerializeField]
    private float _fadeDuration = 1;

    [SerializeField]
    private bool _ignoreTimeScale = false;

    private void Start()
    {
        Debug.Assert(_text != null);
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        while (true)
        {
            //Debug.Log("Hello");
            if (_fading) _text.CrossFadeAlpha(_minTransparency, _fadeDuration, _ignoreTimeScale);
            else _text.CrossFadeAlpha(_maxTransparency, _fadeDuration, _ignoreTimeScale);
            yield return new WaitForSeconds(_fadeDuration);
            _fading = !_fading;
        }
    }
}
