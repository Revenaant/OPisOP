using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResolutionCheckpointUI : MonoBehaviour
{
    private Dictionary<ImageColorSwapper, bool> _images = new Dictionary<ImageColorSwapper, bool>();

    [SerializeField] private float _checkpointTimeDelay = 0.5f;

    [SerializeField] private ImageColorSwapper _image1 = null;
    [SerializeField] private ImageColorSwapper _image2 = null;
    [SerializeField] private ImageColorSwapper _image3 = null;

    // Use this for initialization
    private void Start()
    {
        _images[_image1] = false;
        _images[_image2] = false;
        _images[_image3] = false;

        UpdateCheckpoints();
    }

    private void UpdateCheckpoints()
    {
        var cpm = GameManager.Instance.CheckpointManager;
        if (cpm == null) return;

        for (int i = 0; i < cpm.CheckpointsPassed; i++)
            StartCoroutine(MyCoroutines.Wait(_checkpointTimeDelay, NextCheckpoint));
            //NextCheckpoint();
    }

    private void NextCheckpoint()
    {
        foreach (var image in _images.Keys.ToList())
        {
            if (_images[image]) continue;

            image.Toggle();
            _images[image] = true;
            break;
        }
    }
}
