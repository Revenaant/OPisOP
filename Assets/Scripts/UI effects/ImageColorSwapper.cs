using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageColorSwapper : MonoBehaviour
{
    private enum OnStart { DoNothing, ChangeColorToColor1, ChangeColorToColor2 }

    [SerializeField] private OnStart _onStart = OnStart.ChangeColorToColor1;

    [SerializeField] private Color _color1 = Color.blue;
    [SerializeField] private Color _color2 = Color.gray;

    [SerializeField] private Image[] _images;

    private void Start()
    {
        switch (_onStart)
        {
            case OnStart.ChangeColorToColor1:
                foreach (Image image in _images)
                    image.color = _color1;
                break;
            case OnStart.ChangeColorToColor2:
                foreach (Image image in _images)
                    image.color = _color2;
                break;
            case OnStart.DoNothing:
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        for (int i = _images.Length - 1; i >= 0; i--)
        {
            _images[i] = null;
        }
    }

    public void Toggle()
    {
        foreach (Image image in _images)
        {
            if (image != null)
            {
                if (image.color == _color1)
                    image.color = _color2; //Color.Lerp(_color1, _color2, Time.deltaTime);
                else if (image.color == _color2)
                    image.color = _color1; //Color.Lerp(_color2, _color1, Time.deltaTime);
            }
        }
    }

    public void Color1()
    {
        foreach (Image image in _images)
            image.color = _color1;
    }

    public void Color2()
    {
        foreach (Image image in _images)
            image.color = _color2;
    }
}
