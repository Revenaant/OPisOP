using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TouchScript.Gestures;

public class FoodMachineTutorialNew : MonoBehaviour {

    [SerializeField] private Transform _camMountFM;
    [SerializeField] private Transform _camMountSP;
    [SerializeField] private GameObject _canvasWithArrowButton;
    [SerializeField] private GameObject _canvasWithArrowButtonSP;

    private float _oldEnergyNeed = 0;
    private SolarPanel _solarPanel;
    private CameraControls _camController;
    private bool _zoomIn;
    private bool _wentToSp;
    private FridayTutorial _fridayTutorial;

    private void Start()
    {
        _fridayTutorial = FindObjectOfType<FridayTutorial>();
        _camController = FindObjectOfType<CameraControls>();
        _solarPanel = GameManager.Instance.SolarPanel;

        _solarPanel.gameObject.SetActive(false);

        _canvasWithArrowButton.SetActive(false);
        _canvasWithArrowButtonSP.SetActive(false);

        _oldEnergyNeed = GameManager.Instance.NewFoodMachine.EnergyNeed;
        GameManager.Instance.NewFoodMachine.EnergyNeed = 1;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null || GameManager.Instance.NewFoodMachine == null) return;
        GameManager.Instance.NewFoodMachine.EnergyNeed = _oldEnergyNeed;
    }

    private void Update()
    {
        if (GameManager.Instance.Pet.FedInTutorial == 0)
        {
            Destroy(gameObject);
        }
    }

    public void GoToFoodMachine()
    {
        if (_canvasWithArrowButton.activeSelf) _canvasWithArrowButton.SetActive(false);
    }

    public void GoToSolarPanel()
    {
        if (!_wentToSp)
        {
            _fridayTutorial.Next();
            _camController.MoveFocus(_camMountSP);

            _solarPanel.gameObject.SetActive(true);
            _solarPanel.gameObject.GetComponentInChildren<MaterializeDissolve>().Materialize();

            _canvasWithArrowButtonSP.SetActive(true);
            _wentToSp = true;
        }
    }
}
