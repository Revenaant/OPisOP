﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckpointUI : MonoBehaviour
{
    private Dictionary<ImageColorSwapper, bool> _images = new Dictionary<ImageColorSwapper, bool>();

    [SerializeField] private ImageColorSwapper _image1 = null;
    [SerializeField] private ImageColorSwapper _image2 = null;
    [SerializeField] private ImageColorSwapper _image3 = null;

    // Use this for initialization
    private void Start()
    {
        _images[_image1] = false;
        _images[_image2] = false;
        _images[_image3] = false;

        CheckpointManager.onCheckpointStart += NextCheckpoint;
    }

    private void NextCheckpoint()
    {
        //int counter = 0;
        //foreach (KeyValuePair<ImageColorSwapper, bool> kvp in _images)
        //{
        //    Debug.Log(++counter + ": " + kvp.Value);
        //}

        foreach (var image in _images.Keys.ToList())
        {
            if (_images[image]) continue;

            image.Toggle();
            _images[image] = true;
            break;
        }
    }
}