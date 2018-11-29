using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFoodMachineInternal : MonoBehaviour
{
    private static int _FOOD = 0;
    private static bool _DO_NOT_OVERWRITE = true;
    public static int Food
    {
        get { return _FOOD; }
        set
        {
            _FOOD = value;
            _DO_NOT_OVERWRITE = true;
        }
    }

    private float _timer = 0;

    [SerializeField] private int _food = 0;
    [SerializeField] private float _interval = 0.3f;

    [SerializeField] private GameObject _foodPrefab = null;

    private void OnValidate()
    {
        if (!_DO_NOT_OVERWRITE)
            _FOOD = _food;
        _DO_NOT_OVERWRITE = false;
        _timer = _interval;
    }

    // Update is called once per frame
    private void Update()
    {
        //if (GameManager.Instance == null || !GameManager.Instance.InDome) return;

        //if (Input.GetKeyDown(KeyCode.Question)) _DO_NOT_OVERWRITE = false;
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Debug.Log("FOOD:" + _FOOD);
        //    Debug.Log("Food:" + _food);
        //}
        
        _food = _FOOD;
        DispenseFood();
    }

    private void DispenseFood()
    {
        if (_food <= 0 || _foodPrefab == null) return;

        _timer -= Time.deltaTime;

        if (_timer > 0) return;

        _timer = _interval;
        _FOOD--;

        GameObject go = Instantiate(_foodPrefab);
        go.transform.position = transform.position;
        //go.transform.rotation = transform.rotation;
    }
}
