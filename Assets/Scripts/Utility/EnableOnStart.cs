using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnStart : MonoBehaviour
{
    [SerializeField] private bool _waitOneFrame = false;
    [SerializeField] private GameObject _gameObject = null;

    // Use this for initialization
    private void Start()
    {
        _gameObject.SetActive(false);

        if (_waitOneFrame)
        {
            StartCoroutine(MyCoroutines.WaitOneFrame(() => _gameObject.SetActive(true)));
            return;
        }

        _gameObject.SetActive(true);
    }
}
