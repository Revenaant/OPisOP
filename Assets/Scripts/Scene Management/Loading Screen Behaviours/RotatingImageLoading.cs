using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotatingImageLoading : LoadingScreenBehaviour
{
    public float rotationSpeed = 100;
    public Image image = null;

    // Update is called once per frame
    private void Update()
    {
        LoadingUpdate();
    }

    public override void LoadingUpdate()
    {
        if (image != null)
            image.rectTransform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    public override void Requirements()
    {
        Canvas canvas = image.canvas.rootCanvas;
        DontDestroyOnLoad(canvas.gameObject);
    }
}
