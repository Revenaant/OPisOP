using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappinessBarUI : MonoBehaviour
{
    private float _value = 1;
    private float _target = 1;

    [SerializeField] private float _speed = 0.01f;

    [SerializeField] private Image _fillBar = null;
    [SerializeField] private float _imageWidthFullPoint = 200;

    // Use this for initialization
    private void OnEnable()
    {
        Pet.onHappinessChanged += SetTarget;
        StartCoroutine(MyCoroutines.Wait(1, () =>
        {
            Debug.Log("Yo");
            if (GameManager.Instance != null && GameManager.Instance.Pet != null)
                GameManager.Instance.Pet.ForceEvents();
        }));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (Pet.onHappinessChanged != null)
            Pet.onHappinessChanged -= SetTarget;
    }

    private void Start()
    {
        StartCoroutine(MyCoroutines.Wait(1, () =>
        {
            //Debug.Log("Yo");
            GameManager.Instance.Pet.ForceEvents();
        }));
    }

    // Update is called once per frame
    private void UpdateFillBar(float value)
    {
        _fillBar.rectTransform.sizeDelta = new Vector2(_imageWidthFullPoint * value, _fillBar.rectTransform.sizeDelta.y);
    }

    public void SetTarget(float value)
    {
        StopAllCoroutines();

        _target = value;
        StartCoroutine(LerpBar());
    }

    private IEnumerator LerpBar()
    {
        while (Mathf.Abs(_target - _value) > Mathf.Epsilon)
        {
            //_value = Mathf.Lerp()
            _value = Mathf.MoveTowards(_value, _target, _speed);
            UpdateFillBar(_value);
            yield return null;
        }

        yield return null;
    }
}
