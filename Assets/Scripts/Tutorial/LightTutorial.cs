using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightTutorial : MonoBehaviour {

    [SerializeField] private Transform _camMountLight;
    private LightSwitch _lights;
    [SerializeField] private GameObject _canvasWithArrowButton;
    private CameraControls _camController;
    private bool _isAtLights;
    private FridayTutorial _fridayTutorial;

    private void Start()
    {
        _fridayTutorial = FindObjectOfType<FridayTutorial>();
        _camController = FindObjectOfType<CameraControls>();
        _lights = GameManager.Instance.LightSwitch;
        _canvasWithArrowButton.SetActive(false);
        _lights.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.DayNightCycle.IsNight() && !_isAtLights && !GameManager.Instance.InDome)
        {
            if (!_fridayTutorial.gameObject.activeSelf)
            {
                _fridayTutorial.gameObject.SetActive(true);
                _fridayTutorial.Next();
            }

            _lights.gameObject.SetActive(true);
            _lights.gameObject.GetComponentInChildren<MaterializeDissolve>().Materialize();

            _camController.MoveFocus(_camMountLight);
            _camController.SetZoom(40);
            _isAtLights = true;
            _canvasWithArrowButton.SetActive(true);
        }

        if (_lights.IsON && _isAtLights)
        {
            if (_fridayTutorial.gameObject.activeSelf) _fridayTutorial.gameObject.SetActive(false);
            _camController.SetZoom(60);
            Destroy(gameObject);
        }
    }
}
