using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhackAMole : MonoBehaviour
{

    [SerializeField] private GameObject _spawnPointHolder;
    [SerializeField] private GameObject _handHolder;
    [Space]
    [SerializeField] private float _spawnSpeed;
    [SerializeField] private float _foodSpeed;
    [SerializeField] private GameObject _whakCam;
    [Space]
    [SerializeField] private GameObject _food;
    [SerializeField] private GameObject _defaultSpawn;

    private System.Random _rnd;
    private float timer;

    private List<Transform> _spawnPoints;
    private List<Transform> _handPoints;

    private bool _isOn;
    private int _strikes = 0;
    private Vector3 _mousePos;
    private PetForWhaking _theOtherScriptForThis;
    private float _defaultSpeedFood, _defaultSpeedSpawn;
    private GameObject _pet;
    private Animator _anime;

    private GroundAnimation _GroundAmination;
    private CameraInterior _camInterior;

    private int _nShot;

    private void Start()
    {
        _GroundAmination = FindObjectOfType<GroundAnimation>();
        _defaultSpeedFood = _foodSpeed;
        _defaultSpeedSpawn = _spawnSpeed;
        _pet = GameManager.Instance.Pet.gameObject;
        _theOtherScriptForThis = _pet.GetComponent<PetForWhaking>();
        //_pet.transform.position = Vector3.zero;

        _spawnPoints = new List<Transform>();
        _handPoints = new List<Transform>();

        _rnd = new System.Random();
        foreach (Transform child in _spawnPointHolder.transform)
        {
            if (!_spawnPoints.Contains(child)) _spawnPoints.Add(child);
        }
        //StartCoroutine(WaitForAnimation());
        foreach (Transform child in _handHolder.transform)
        {
            if (!_handPoints.Contains(child)) _handPoints.Add(child);
            child.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!_isOn) return;

        _mousePos = Input.mousePosition;

        if (_camInterior == null) _camInterior = FindObjectOfType<CameraInterior>();

        WhakedInMiniGame();

        timer += Time.deltaTime;
        if (timer >= _spawnSpeed)
        {
            if (_nShot < 4)
            {
                foreach (Transform child in _handHolder.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }

            int t = _rnd.Next(_spawnPoints.Count);
            _pet.transform.position = _spawnPoints[t].position;
            _pet.transform.rotation = _spawnPoints[t].localRotation;

            if (_nShot < 3) _handPoints[t].gameObject.SetActive(true);
            
            JumpRetardJump(t);
            _strikes++;
            timer = 0;
        }

        if (_strikes > 2)
        {
            if (_anime == null) _anime = _pet.GetComponentInChildren<Animator>();
            _anime.SetTrigger("MiniGameEnd");
            _pet.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            _pet.GetComponent<Rigidbody>().freezeRotation = true;
            _pet.GetComponent<Rigidbody>().AddForce(new Vector3(0, 7f, 70.2f), ForceMode.VelocityChange);
            StartCoroutine(GoBackDelay());
            //Camera.main.GetComponent<CamSwitchForMiniGames>().SetMount(Camera.main.GetComponent<CamSwitchForMiniGames>().DefaultMount);
            if (_camInterior != null) _camInterior._state = State.Default;
            _strikes = 0;
            _foodSpeed = _defaultSpeedFood;
            _spawnSpeed = _defaultSpeedSpawn;
            _isOn = false;
        }
    }

    public void StartMiniGame()
    {
        _isOn = true;
        _nShot = 0;
        //Camera.main.gameObject.transform.position = _whakCam.transform.position;
        //Camera.main.gameObject.transform.rotation = _whakCam.transform.rotation;
    }

    private void EndGame()
    {
        _pet.transform.position = _defaultSpawn.transform.position;
        _pet.transform.rotation = _defaultSpawn.transform.rotation;
        _GroundAmination.CloseHatch();
        _pet.GetComponent<Rigidbody>().isKinematic = true;
        _pet.GetComponent<Rigidbody>().useGravity = false;
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void ShootFood(Transform pTransform)
    {
        GameObject p = Instantiate(_food, Camera.main.gameObject.transform.position, Quaternion.identity);

        p.transform.LookAt(pTransform);

        p.GetComponent<Rigidbody>().AddForce(p.transform.forward * _foodSpeed, ForceMode.VelocityChange);

        _nShot++;
    }

    private void WhakedInMiniGame()
    {
        if (_theOtherScriptForThis.Hit)
        {
            if (_nShot < 4)
            {
                foreach (Transform child in _handHolder.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }

            int t = _rnd.Next(_spawnPoints.Count);
            _pet.transform.position = _spawnPoints[t].position;
            _pet.transform.rotation = _spawnPoints[t].localRotation;

            if (_nShot < 3) _handPoints[t].gameObject.SetActive(true);

            JumpRetardJump(t);
            timer = 0;
            _spawnSpeed -= 0.1f;
            _foodSpeed += 0.8f;
            _theOtherScriptForThis.Hit = false;
        }
    }

    private void JumpRetardJump(int pPlace)
    {
        _pet.GetComponent<Rigidbody>().isKinematic = false;
        _pet.GetComponent<Rigidbody>().useGravity = true;
        if(pPlace < 3)_pet.GetComponent<Rigidbody>().AddForce(Vector3.up * 7f, ForceMode.VelocityChange);
        else _pet.GetComponent<Rigidbody>().AddForce(Vector3.up * 8.3f, ForceMode.VelocityChange);
        StartCoroutine(JumpDelay());
    }

    public IEnumerator WaitForAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            StartMiniGame();
            break;
        }
    }

    private IEnumerator JumpDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.6f);
            _pet.GetComponent<Rigidbody>().isKinematic = true;
            _pet.GetComponent<Rigidbody>().useGravity = false;
            break;
        }
    }

    private IEnumerator GoBackDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            EndGame();
            break;
        }
    }
}
