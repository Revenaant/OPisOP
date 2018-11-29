using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillMiniGame : MonoBehaviour {

    [SerializeField] private float _treadmillSpeed;
    private bool _isOn;
    [SerializeField] private GameObject _pet;
    private Rigidbody _rb;
    private Vector3 _treadmillMove;
    private float timer;
    private int _strikes;
    private float _outTimer;
    private bool _isOut;
    [Space]
    [SerializeField] private GameObject _treadmillSpawn;
    [SerializeField] private GameObject _defaultSpawn;
    private int _layer;
    private float _defaultSpeed;

    private void Start ()
    {
        _layer = LayerMask.NameToLayer("Treadmill");
        _rb = _pet.GetComponent<Rigidbody>();
        _defaultSpeed = _treadmillSpeed;
	}

    private void Update()
    {
        if (!_isOn) return;

        RaycastDown();

        timer += Time.deltaTime;
        if (timer >= 1)
        {
            _treadmillSpeed += 0.5f;
            timer = 0;
        }

        if (_isOut)
        {
            _outTimer += Time.deltaTime;
            if (_outTimer > 1)
            {
                _pet.transform.position = _treadmillSpawn.transform.position;
                _pet.transform.rotation = _treadmillSpawn.transform.rotation;
                _outTimer = 0;
                _treadmillSpeed = _defaultSpeed;
                _strikes++;
                _isOut = false;
            }
        }

        if (_strikes > 2)
        {
            _rb.constraints = RigidbodyConstraints.None;
            _pet.transform.position = _defaultSpawn.transform.position;
            _pet.transform.rotation = _defaultSpawn.transform.rotation;
            Camera.main.GetComponent<CamSwitchForMiniGames>().SetMount(Camera.main.GetComponent<CamSwitchForMiniGames>().DefaultMount);
            _strikes = 0;
            _isOn = false;
            _isOut = false;
        }
    }

    public void StartMiniGame()
    {
        _isOn = true;
        _treadmillSpeed = _defaultSpeed;
        _rb.constraints = RigidbodyConstraints.None;
        _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;

        _pet.transform.position = _treadmillSpawn.transform.position;
        _pet.transform.rotation = _treadmillSpawn.transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
       // GameManager.Instance.Storage.Energy += 10;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Pet")
        {
            if (_isOn)
            {
                _rb.constraints = RigidbodyConstraints.None;
                _rb.constraints = RigidbodyConstraints.FreezeRotation;
                if (_rb.velocity.z < _treadmillSpeed)
                {
                    _rb.AddForce(_pet.transform.forward * _treadmillSpeed, ForceMode.Acceleration);
                   // _rb.velocity = _petVel;
                }
            }
            //else _strikes++;
        }
    }

    private void RaycastDown()
    {
        RaycastHit _hit;

        // Raycast
        if (Physics.Raycast(_pet.transform.position, Vector3.down, out _hit, 10))
        {
            if (_hit.transform.gameObject.layer != _layer)
            {
                _isOut = true;
            }
        }
    }
}
