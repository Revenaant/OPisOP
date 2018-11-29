using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CactusTutorial : MonoBehaviour
{
    [SerializeField] private Transform _camMountGenerator;
    //[SerializeField] private GameObject _canvas;
    [SerializeField] private GameObject _canvasWithArrowButton;
    private CameraControls _camController;
    private bool _atTempMachine;
    private FridayTutorial _fridayTutorial;

    private void Start()
    {
        _camController = FindObjectOfType<CameraControls>();
        _canvasWithArrowButton.SetActive(false);
        _fridayTutorial = FindObjectOfType<FridayTutorial>();
        //_canvas.SetActive(false);
    }

    private void Update()
    {
        if (GameManager.Instance.PlantBed.Cacti > 0) GoToDefaultView();
    }

    public void GoToDefaultView()
    {
        _fridayTutorial.gameObject.SetActive(false);
        _canvasWithArrowButton.SetActive(false);
        Destroy(gameObject);
    }

    public void GoToCactuses()
    {
        _canvasWithArrowButton.SetActive(false);
        Camera.main.orthographic = false;
        _atTempMachine = true;
    }
}
