using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarPanelTutorial3 : MonoBehaviour {

    [SerializeField] private Transform _camMountSP;
    private SolarPanel _solarPanel;
    [SerializeField] private GameObject _canvasWithArrowButton;
    [SerializeField] private GameObject _canvasWithArrowButtonSP;
    private CameraControls _camController;
    private bool _zoomIn;
    private FridayTutorial _fridayTutorial;
    private bool _isBuggy;
    private bool _waitedForDelay;

    private void Start()
    {
        _solarPanel = GameManager.Instance.SolarPanel;
        _fridayTutorial = FindObjectOfType<FridayTutorial>();
        _camController = FindObjectOfType<CameraControls>();
        StartCoroutine(delay());
    }

    private void Update()
    {
        _solarPanel.ForceEvents();
        if(_waitedForDelay) GameManager.Instance.Storage.Energy = 2;

        if (_solarPanel.Dirt <= 0.1f)
        {
            GoToDefaultView();
        }
    }

    public void GoToDefaultView()
    {
        if (!_isBuggy)
        {
            _isBuggy = true;
            _fridayTutorial.Next();
            _camController.SetZoom(60);
            _canvasWithArrowButton.SetActive(true);
            _solarPanel.Dirt = 0.0f;
            Destroy(gameObject);
        }
    }

    public void GoToSolarPannel()
    {
        _canvasWithArrowButtonSP.SetActive(false);
    }

    private IEnumerator delay()
    {
        yield return new WaitForSeconds(1);
        _waitedForDelay = true;
    }
}
