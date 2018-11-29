using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayDosNewTutorial : MonoBehaviour {

    [SerializeField] private Transform _camMountGenerator;
    [SerializeField] private GameObject _canvasWithArrowButton;
    [SerializeField] private GameObject _canvasWithArrowButtonTemp;
    [Space]
    [SerializeField] private GameObject _tempMachine;
    [SerializeField] private GameObject _barForTempMachine;

    private CameraControls _camController;
    private GasTank _generator;
    private bool _isAtOil;
    private FridayTutorial _fridayTutorial;

    private void Start ()
    {
        _camController = FindObjectOfType<CameraControls>();
        _canvasWithArrowButtonTemp.SetActive(false);
        _canvasWithArrowButton.SetActive(false);
        _generator = GameManager.Instance.GasTank;
        _fridayTutorial = FindObjectOfType<FridayTutorial>();
        _generator.gameObject.SetActive(false);

        _tempMachine.GetComponentInParent<Collider>().enabled = false;
        _tempMachine.SetActive(false);
        _barForTempMachine.SetActive(false);
    }

    private bool _startedStorm = false;
    private void Update()
    {
        if (GameManager.Instance.GameRules.Days == 1)
        {
            GameManager.Instance.PollutionManager.Pollution = 0;
            GameManager.Instance.Temperature = 50;
        }
        else if (GameManager.Instance.GameRules.Days == 2)
        {
            if (!_startedStorm)
            {
                GameManager.Instance.Storms.IsStorming = true;
                _startedStorm = true;
            }

            if (_generator.GetComponent<GasTank>().IsOn) GoToDefaultView();
            if (!_isAtOil)
            {
                _generator.gameObject.SetActive(true);
                _generator.gameObject.GetComponentInChildren<MaterializeDissolve>().Materialize();

                _canvasWithArrowButton.SetActive(true);
                _camController.MoveFocus(_camMountGenerator);
                _camController.SetZoom(40);
                _isAtOil = true;

                if (!_fridayTutorial.gameObject.activeSelf) _fridayTutorial.gameObject.SetActive(true);
                _fridayTutorial.Next();
            }
        }
    }

    public void GoToDefaultView()
    {

        _barForTempMachine.SetActive(true);
        _tempMachine.SetActive(true);
        _tempMachine.GetComponentInParent<Collider>().enabled = true;
        _tempMachine.GetComponentInChildren<MaterializeDissolve>().Materialize();

        _camController.SetZoom(60);
        _canvasWithArrowButtonTemp.SetActive(true);
        _fridayTutorial.Next();
        if(gameObject != null) Destroy(gameObject);
    }

}
