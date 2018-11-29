using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeMiniGame : MonoBehaviour {

    private GameObject _pet;
    private GameObject _ropeCenter;
    [Space]
    [SerializeField] private GameObject _handCanvas;
    //[SerializeField] private GameObject _defaultSpawn;

    private Rigidbody _rb;
    private float timer = 0;

    private bool _minigamePlaying = false;

    private Vector3 _petStartPos;
    [SerializeField] private float _ropeSpeed;
    private GroundAnimation _GroundAmination;
    private int _strikes;
    private bool _isOut;
    private int _layer;
    private float _outTimer;
    private float _defaultSpeed;
    private float _startAgainTime;
    private bool _waitForRestart;

    private bool _fuckThisShit;
    private bool _over;
    private GameObject _plate;

    private CameraInterior _camInterior;

    private void Start()
    {
        _GroundAmination = FindObjectOfType<GroundAnimation>();
        _pet = GameManager.Instance.Pet.gameObject;
        _rb = _pet.GetComponent<Rigidbody>();
        _petStartPos = _pet.transform.position;
        _defaultSpeed = _ropeSpeed;
        _ropeCenter = gameObject;
        _plate = FindObjectOfType<JumpRopePlate>().gameObject.transform.parent.gameObject;
        _handCanvas.SetActive(false);
        //StartCoroutine(WaitForAnimation());
    }

    private void Update()
    {
        if (!_minigamePlaying) return;

        if (_camInterior == null) _camInterior = FindObjectOfType<CameraInterior>();

        if (_ropeSpeed > _ropeSpeed + _defaultSpeed * 2) _handCanvas.SetActive(false);

        RotateRope();

        if (_strikes > 2)
        {
            if(_plate != null) Destroy(_plate);

            if (_pet.GetComponent<Pet>().OnTheGround && !_over)
            {
                _pet.GetComponentInChildren<Animator>().SetTrigger("MiniGameEnd");
                _rb.constraints = RigidbodyConstraints.None;
                _rb.freezeRotation = true;
                _rb.AddForce(new Vector3(0, 7f, 7.2f), ForceMode.VelocityChange);
                _over = true;
                StartCoroutine(GoBackDelay());
                if (_camInterior != null) _camInterior._state = State.Default;
            }
        }

        if (_waitForRestart)
        {
            _startAgainTime += Time.deltaTime;
            if (_startAgainTime > 2)
            {
                _ropeSpeed = _defaultSpeed;
                _startAgainTime = 0;
                _waitForRestart = false;
            }
        }
    }

    private void StartMiniGame()
    {
        _ropeSpeed = _defaultSpeed;

        _minigamePlaying = true;

        _rb.isKinematic = false;
        _rb.useGravity = true;

        _rb.constraints = RigidbodyConstraints.None;
        _rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        _rb.freezeRotation = true;

        _over = false;
        _handCanvas.SetActive(true);

        //_pet.transform.position = _ropeSpawn.transform.position;
        //_pet.transform.rotation = _ropeSpawn.transform.rotation;
        _pet.GetComponent<Pet>().JumpForce *= 2f;

    }

    private void RotateRope()
    {
        _ropeCenter.transform.Rotate(new Vector3(0, 0, _ropeSpeed * -1));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Pet")
        {
            _strikes++;
            _ropeCenter.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
            _ropeSpeed = 0;
            _waitForRestart = true;
        }
    }

    private void EndGame()
    {
        _rb.constraints = RigidbodyConstraints.None;

        _pet.GetComponent<Pet>().JumpForce /= 2f;

        Camera.main.fieldOfView = 20;
        _strikes = 0;
        _minigamePlaying = false;
        _GroundAmination.CloseHatch();
        _rb.isKinematic = true;
        _rb.useGravity = false;
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public float RopeSpeed
    {
        get { return _ropeSpeed; }
        set { _ropeSpeed = value; }
    }

    public IEnumerator WaitForAnimation()
    {
        while (true)
        {
            if (_fuckThisShit) yield break;
            yield return new WaitForSeconds(4);
            StartMiniGame();
            _fuckThisShit = true;
        }
    }

    private IEnumerator GoBackDelay()
    {
        yield return new WaitForSeconds(1);
        EndGame();
    }
}