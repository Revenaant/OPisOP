using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryTutorialNew : MonoBehaviour {

    [SerializeField] private GameObject BatteryArrow;
    [SerializeField] private Transform _batteryFocus;
    private CameraControls _camController;
    private bool _forcedOvercharge;
    private bool _wasAtBattery;
    private FridayTutorial _fridayTutorial;

    private void Start ()
    {
        BatteryArrow.SetActive(false);
        _camController = FindObjectOfType<CameraControls>();
        _fridayTutorial = FindObjectOfType<FridayTutorial>();
    }

    private void Update()
    {
        if (!_forcedOvercharge)
        {
            GameManager.Instance.DayNightCycle.SetTimeOfDay(0.3f);
            if (GameManager.Instance.Pet.FedInTutorial <= 0)
            {
                GameManager.Instance.Storage.Energy = GameManager.Instance.Storage.Capacity;
                BatteryArrow.SetActive(true);
                _forcedOvercharge = true;
                _fridayTutorial.Next();
            }
        }
        else
        {
            if (!GameManager.Instance.Storage.OverThreshold)
            {
                _camController.SetZoom(60);
                Destroy(gameObject);
            }
        }
    }

    public void ClickedOnBattery()
    {
        if (!_wasAtBattery)
        {
            _fridayTutorial.gameObject.SetActive(false);
            _camController.MoveFocus(_batteryFocus);
            _camController.SetZoom(50);
            BatteryArrow.SetActive(false);
            _wasAtBattery = true;
        }
    }
}
