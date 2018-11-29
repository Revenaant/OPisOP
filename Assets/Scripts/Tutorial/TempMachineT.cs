using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempMachineT : MonoBehaviour
{
    [SerializeField] private Transform _camMountGenerator;
    [SerializeField] private GameObject _canvasWithArrowButton;
    [SerializeField] private GameObject _canvasWithArrowButtonCactus;
    [Space]
    [SerializeField] private GameObject _cactusMachine;

    private CameraControls _camController;
    private FridayTutorial _fridayTutorial;
    private bool _wasAtMachine;

    private void Start()
    {
        _fridayTutorial = FindObjectOfType<FridayTutorial>();
        _camController = FindObjectOfType<CameraControls>();
        _canvasWithArrowButton.SetActive(false);
        _canvasWithArrowButtonCactus.SetActive(false);

        _cactusMachine.SetActive(false);
    }

    private bool _startedStorm = false;
    private void Update()
    {
        if (GameManager.Instance.GameRules.Days == 2)
        {
            if (!_startedStorm)
            {
                GameManager.Instance.Storms.StartStorm();
                _startedStorm = true;
            }

            if (_wasAtMachine)
            {
                GoToDefaultView();
            }
        }
    }

    public void GoToDefaultView()
    {
        GameManager.Instance.Storms.StopStorm();

        _cactusMachine.SetActive(true);
        _cactusMachine.GetComponentInChildren<MaterializeDissolve>().Materialize();

        _camController.SetZoom(60);
        if (_canvasWithArrowButtonCactus != null) _canvasWithArrowButtonCactus.SetActive(true);
        _fridayTutorial.Next();
        Destroy(gameObject);
    }

    public void GoToTemperatureMachine()
    {
        if (GameManager.Instance.GameRules.Days == 2) _wasAtMachine = true;
    }
}

