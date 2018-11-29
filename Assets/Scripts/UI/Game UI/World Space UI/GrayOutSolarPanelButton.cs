using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayOutSolarPanelButton : MonoBehaviour
{
    [SerializeField] private SolarPanel _solarPanel = null;
    [SerializeField] private GameObject _button = null;
    [SerializeField] private GameObject _disabledButton = null;

    // Use this for initialization
    private void Start()
    {
        if (_solarPanel == null && GameManager.Instance != null) _solarPanel = GameManager.Instance.SolarPanel;
        if (_solarPanel == null) _solarPanel = FindObjectOfType<SolarPanel>();

        Debug.Assert(_solarPanel != null, "Plant Bed is not assigned or found.");
        //_plantBed.OnMake
    }

    // Update is called once per frame
    private void Update()
    {
        if (_button == null || _disabledButton == null || _solarPanel == null) return;

        bool clean = !_solarPanel.IsDirty();
        _button.SetActive(!clean);
        _disabledButton.SetActive(clean);
    }
}
