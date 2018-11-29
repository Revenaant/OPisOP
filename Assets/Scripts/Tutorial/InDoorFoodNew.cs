using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InDoorFoodNew : MonoBehaviour
{

    private Pet _pet;
    private SolarPanel _solarPanel;
    private CameraControls _camController;

    [SerializeField] private Transform _camMountFM;
    [SerializeField] private GameObject _foodMachineArrow;
    [SerializeField] private GameObject _goOutIndicator;
    [SerializeField] private EventTrigger _goOutEventTrigger;
    [Space]
    [SerializeField] private GameObject _foodMachineModel;

    private FridayTutorial _fridayTutorial;
    private int _textN;
    private float _textTimer;
    private bool _endOfPart1;
    private bool _endOfPart2;
    private bool _isBuggy;

    private void Start()
    {
        _goOutIndicator.SetActive(false);
        _foodMachineArrow.SetActive(false);
        _camController = FindObjectOfType<CameraControls>();
        _fridayTutorial = FindObjectOfType<FridayTutorial>();

        _goOutEventTrigger.enabled = false;
        _foodMachineModel.SetActive(false);
        //_foodMachineModel.GetComponent<MaterializeDissolve>().Dematerialize();

        _solarPanel = GameManager.Instance.SolarPanel;
        _pet = GameManager.Instance.Pet;
        _pet.Hunger = 20;
        _pet.HungerPerSecond = 0;
        _fridayTutorial.Next();
        StartCoroutine(delay());
    }

    private void Update()
    {
        if (_pet.FedInTutorial == 3 && !_endOfPart1)
        {
            _fridayTutorial.Next();
            _endOfPart1 = true;
        }
        else if (_pet.FedInTutorial == 2 && !_endOfPart2)
        {
            _fridayTutorial.Next();
            _endOfPart2 = true;
            _goOutIndicator.SetActive(true);
            _goOutEventTrigger.enabled = true;
        }
    }

    public void GoToDefault()
    {
        if (_foodMachineArrow) _foodMachineArrow.SetActive(true);
        _solarPanel.Dirt = 1;
        GameManager.Instance.Storage.Energy = 0;

        _foodMachineModel.SetActive(true);
        _foodMachineModel.GetComponent<MaterializeDissolve>().Materialize();

        _camController.MoveFocus(_camMountFM);
        _camController.SetZoom(40);
        _goOutIndicator.SetActive(false);

        Destroy(gameObject);
    }

    private IEnumerator delay()
    {
        yield return new WaitForSeconds(3);
        _fridayTutorial.Next();
    }
}
