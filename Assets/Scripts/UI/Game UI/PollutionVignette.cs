using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PollutionVignette : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float _lerpSpeed = 0.35f;
    [SerializeField, Range(1, 255)] private int _maxAlpha = 255;

    private Image _image;
    private Transform[] childs;

    // Use this for initialization
    private void Start()
    {
        _image = GetComponent<Image>();
        _image.canvasRenderer.SetAlpha(0.01f);
        Debug.Assert(_image != null, "Could not find Image in " + gameObject.name);

        childs = transform.GetComponentsInChildren<Transform>();
    }

    private void OnEnable()
    {
        PollutionManager.onPollutionDamageChange += UpdateAlphaValue;
    }

    private void OnDisable()
    {
        if (PollutionManager.onPollutionDamageChange != null)
            PollutionManager.onPollutionDamageChange -= UpdateAlphaValue;
    }

    // Update is called once per frame
    private void UpdateAlphaValue(float value)
    {
        float newValue = value * _maxAlpha;
        _image.CrossFadeAlpha(0.01f + newValue, _lerpSpeed, false);

        if (newValue > 0.5f)
        {
            for(int i = 0; i < childs.Length; i++)
                childs[i].gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < childs.Length; i++)
                childs[i].gameObject.SetActive(false);
        }
    }
}
