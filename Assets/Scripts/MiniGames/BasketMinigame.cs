using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class BasketMinigame : MonoBehaviour, IMinigame
{
    private class Food
    {
        public GameObject go = null;
        public float timer = 7;

        public Food(GameObject go, float timer)
        {
            this.go = go;
            this.timer = timer;
        }
    }

    private List<Food> _leftoverFood = new List<Food>();

    [SerializeField]
    private TransformGesture _transformGesture = null;

    [Header("Positioning, Clamps, Speed and Direction")]
    [SerializeField] private float _leftLimit = -11;
    [SerializeField] private float _rightLimit = 11;
    [SerializeField] private float _pos = 0;

    [Space]

    [SerializeField, ReadOnly] private int _direction = 1;
    [SerializeField] private float _speed = 0.1f;


    [Header("Time (seconds)")]
    [SerializeField] private float _timer = 20;
    [SerializeField] private float _time = 20;

    [SerializeField] private float _dropTime = 3;
    [SerializeField] private float _dropInterval = 3;

    [SerializeField] private float _foodDeathTimer = 7;

    [Header("Minigame")]
    [SerializeField, ReadOnly] private bool _started = false;
    [SerializeField] private string _tag = "Suppliment";

    [Header("Objects")]

    [SerializeField] private Basket _basket = null;

    [SerializeField] private GameObject _food = null;

    [SerializeField] private Transform _line = null;
    [SerializeField] private Transform _dispenser = null;

    private void OnDrawGizmosSelected()
    {
        GizmosHelper.DrawGizmoLineArrow(_line.position, _line.parent.TransformPoint(_line.localPosition + new Vector3(_leftLimit, 0, 0)), Color.yellow);
        GizmosHelper.DrawGizmoLineArrow(_line.position, _line.parent.TransformPoint(_line.localPosition + new Vector3(_rightLimit, 0, 0)), Color.magenta);
    }

    private void OnValidate()
    {
        UpdateDispenserPosition();

        _time = _timer;
        _dropTime = _dropInterval;
    }

    private void OnEnable()
    {
        //_transformGesture = GetComponent<TransformGesture>();

        if (_transformGesture == null) return;
        _transformGesture.Transformed += TransformGestureHandler;
    }

    private void OnDisable()
    {
        if (_transformGesture == null) return;
        _transformGesture.Transformed -= TransformGestureHandler;

    }

    private void TransformGestureHandler(object sender, EventArgs e)
    {
        //Debug.Log("L: " + _transformGesture.LocalDeltaPosition);
        //Debug.Log("W: " + _transformGesture.DeltaPosition);
        if (!_started) return;

        Vector3 delta = _transformGesture.LocalDeltaPosition;
        Vector3 pos = _basket.transform.localPosition;

        pos.x += delta.x;
        _basket.transform.localPosition = pos;
        pos = transform.localPosition;

        pos.x += delta.x;
        transform.localPosition = pos;
    }

    // Use this for initialization
    private void Start()
    {
        Debug.Assert(_food != null, "Food Prefab is not assigned.");
        Debug.Assert(_basket != null, "Basket isn't assigned.");
        Debug.Assert(_line != null, "Line isn't assigned");
        Debug.Assert(_dispenser != null, "Dispenser isn't assigned");

        this.gameObject.SetActive(false);

        _basket.Tag = _tag;
        _basket.onStick += OnStick;
        _basket.onUnstick += OnUnstick;
    }

    private void OnStick(GameObject go)
    {
        if (_leftoverFood.Count == 0) return;

        for (int i = _leftoverFood.Count - 1; i >= 0; i--)
        {
            Food food = _leftoverFood[i];
            if (food.go == go)
                _leftoverFood.RemoveAt(i);
        }
    }

    private void OnUnstick(GameObject go)
    {
        if (_leftoverFood.Count == 0) return;

        bool found = false;

        for (int i = _leftoverFood.Count - 1; i >= 0; i--)
        {
            Food food = _leftoverFood[i];
            found = food.go == go;
            if (found) break;
        }

        if (found) _leftoverFood.Add(new Food(go, _foodDeathTimer));
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_started) return;

        KillFood();
        UpdateMovement();
        UpdateFoodDrop();
        UpdateTimer();
    }

    private void LateUpdate()
    {
        //var pos = transform.localPosition;
        //pos.y = 0;
        //pos.z = 0;
        //transform.localPosition = pos;
    }

    #region Movement
    private void UpdateMovement()
    {
        if (_pos >= _rightLimit || _pos <= _leftLimit) _direction *= -1;

        _pos += _direction * _speed;
        UpdateDispenserPosition();
    }

    private void UpdateDispenserPosition()
    {
        if (_dispenser.parent == null) return;

        _pos = Mathf.Clamp(_pos, _leftLimit, _rightLimit);
        _dispenser.localPosition = new Vector3(_pos, 0, 0);
    }
    #endregion

    #region Food

    private void KillFood()
    {
        if (_leftoverFood.Count == 0) return;

        for (int i = _leftoverFood.Count - 1; i >= 0; i--)
        {
            Food food = _leftoverFood[i];
            food.timer -= Time.deltaTime;

            if (!(food.timer <= 0)) continue;

            _leftoverFood.RemoveAt(i);
            Destroy(food.go);
        }
    }

    private void UpdateFoodDrop()
    {
        _dropTime -= Time.deltaTime;

        if (_dropTime > 0) return;

        Drop();
        _dropTime = _dropInterval;
    }

    private void Drop()
    {
        GameObject food = Instantiate(_food, _dispenser.position, Quaternion.identity);

        _leftoverFood.Add(new Food(food, _foodDeathTimer));

        Rigidbody rb = food.GetComponent<Rigidbody>();

        if (rb != null) rb.useGravity = true;
    }
    #endregion

    #region Game Timer
    private void UpdateTimer()
    {
        _time -= Time.deltaTime;

        if (_time > 0) return;

        EndMiniGame();
        _time = _timer;
    }

    private IEnumerator MoveBackToStart()
    {
        while (Math.Abs(_pos) > Mathf.Epsilon)
        {
            _pos = Mathf.MoveTowards(_pos, 0, _speed);
            UpdateDispenserPosition();
            yield return null;
        }

        yield return null;
    }
    #endregion

    public void StartMiniGame()
    {
        _started = true;

        _basket.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
        print(this.gameObject.activeSelf);
    }

    public void EndMiniGame()
    {
        _started = false;
        StartCoroutine(MoveBackToStart());

        Debug.Log(_basket.Result);
        _basket.Reset();

        foreach (Food food in _leftoverFood)
            Destroy(food.go);

        _leftoverFood.Clear();

        _basket.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        Camera.main.GetComponent<CamSwitchForMiniGames>().SetMount(Camera.main.GetComponent<CamSwitchForMiniGames>().DefaultMount);
    }
}
