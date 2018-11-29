using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideInsideButtonsGrayingOut : MonoBehaviour
{
    [SerializeField] private GameObject _insideOn = null;
    [SerializeField] private GameObject _insideOff = null;
    [SerializeField] private GameObject _outsideOn = null;
    [SerializeField] private GameObject _outsideOff = null;

    private void OnEnable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.onDomeChange += UpdateButtons;
    }

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(_insideOn != null, "Assign Inside Enabled Button");
        Debug.Assert(_insideOff != null, "Assign Inside Disabled Button");
        Debug.Assert(_outsideOn != null, "Assign Outside Enabled Button");
        Debug.Assert(_outsideOff != null, "Assign Outside Disabled Button");

        if (GameManager.Instance != null) UpdateButtons(GameManager.Instance.InDome);
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null || GameManager.Instance.onDomeChange == null) return;
        GameManager.Instance.onDomeChange -= UpdateButtons;
    }


    // Update is called once per frame
    private void UpdateButtons(bool inside)
    {
        _insideOn.SetActive(!inside);
        _outsideOn.SetActive(inside);

        _insideOff.SetActive(inside);
        _outsideOff.SetActive(!inside);
    }
}
