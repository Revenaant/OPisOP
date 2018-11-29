using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayOutCactusButton : MonoBehaviour
{
    [SerializeField] private PlantBed _plantBed = null;
    [SerializeField] private GameObject _button = null;
    [SerializeField] private GameObject _disabledButton = null;

    // Use this for initialization
    private void Start()
    {
        if (_plantBed == null && GameManager.Instance != null) _plantBed = GameManager.Instance.PlantBed;
        if (_plantBed == null) _plantBed = FindObjectOfType<PlantBed>();

        Debug.Assert(_plantBed != null, "Plant Bed is not assigned or found.");
        //_plantBed.OnMake
    }

    // Update is called once per frame
    private void Update()
    {
        if (_button == null || _disabledButton == null || _plantBed == null) return;

        bool atMax = _plantBed.CurrentUpgradeLevel == _plantBed.Cacti;
        _button.SetActive(!atMax);
        _disabledButton.SetActive(atMax);
    }
}
