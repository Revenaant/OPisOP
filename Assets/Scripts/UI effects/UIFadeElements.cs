using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeElements : MonoBehaviour {

    private Image _img;
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float fadeOutTime = 0.5f;
    [SerializeField] private bool disableAtEnd = false;

    private void Awake()
    {
        _img = GetComponent<Image>();
        _img.color = Color.black;
    }

    public static void CrossFadeIn(Image image, float time, float min = 0f, float max = 1f)
    {
        //image.CrossFadeAlpha(max, time, )
    }

    public void FadeIn()
    {
        _img.CrossFadeAlpha(1, fadeInTime, false);

        if(disableAtEnd)
            StartCoroutine(MyCoroutines.Wait(fadeInTime, () => gameObject.SetActive(false)));
    }

    public void FadeOut()
    {
        _img.CrossFadeAlpha(0.01f, fadeOutTime, false);

        if (disableAtEnd)
            StartCoroutine(MyCoroutines.Wait(fadeOutTime, () => gameObject.SetActive(false)));
    }
}
