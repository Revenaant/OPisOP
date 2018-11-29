using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFillBar : MonoBehaviour
{
    [Tooltip("Image used as the foreground of the fill bar, depletes")]
    [SerializeField] protected Image _fill;
    [Tooltip("Image used as the background of the fill bar")]
    [SerializeField] protected Image _back;

    private bool _isBlinking;
    private const float _blinkingTime = 0.3f;
    private const float _blinkFadeAmmount = 0.4f;

    /// <summary>
    /// Sets and gets the fill ammount the bar is currently at
    /// </summary>
    public float Value
    {
        get { return _fill.fillAmount; }
        set { _fill.fillAmount = Mathf.Max(0, Mathf.Min(1, value)); }
    }

    public bool shouldBlink
    {
        get { return _isBlinking; }
        set
        {
            StopAllCoroutines();
            if (_back == null) return;

            if (value == true)
            {
                StartCoroutine(blink());
            }
            else
            {
                Color color = _back.color;
                color.a = 1f;
                _back.color = color;
            }

            _isBlinking = value;
        }
    }

    public IEnumerator lerpBar(float newValue)
    {
        float currentVal = Value;

        // Lerp the bar fill ammount
        while (currentVal > newValue + 0.01f || currentVal < newValue - 0.01f)
        {
            currentVal = Mathf.Lerp(currentVal, newValue, Time.deltaTime * 5);
            Value = currentVal;
            yield return null;
        }

        // Disable the bar after the lerping is finished
        if (newValue == 0) disableBar();
    }

    /// <summary>
    ///  Switches the alpha of the image on intervals
    /// </summary>
    /// <returns></returns>
    private IEnumerator blink()
    {
        bool blink = false;

        while (true)
        {
            Color color = _back.color;

            color.a = blink ? _blinkFadeAmmount : 1f;
            blink = !blink;

            _back.color = color;
            yield return new WaitForSeconds(_blinkingTime);
        }
    }

    protected virtual void disableBar()
    {
        gameObject.SetActive(false);
    }
}
