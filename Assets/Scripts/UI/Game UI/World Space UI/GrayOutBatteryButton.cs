using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayOutBatteryButton : MonoBehaviour
{
    [SerializeField] private Storage _storage = null;
    [SerializeField] private GameObject _button = null;
    [SerializeField] private GameObject _disabledButton = null;

    // Use this for initialization
    private void Start()
    {
        if (_storage == null && GameManager.Instance != null) _storage = GameManager.Instance.Storage;
        if (_storage == null) _storage = FindObjectOfType<Storage>();

        Debug.Assert(_storage != null, "Storage is not assigned or found.");
        //_plantBed.OnMake
    }

    // Update is called once per frame
    private void Update()
    {
        if (_button == null || _disabledButton == null || _storage == null) return;

        bool overcharging = _storage.Overcharging;
        _button.SetActive(overcharging);
        _disabledButton.SetActive(!overcharging);
    }
}
