using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAnimation : MonoBehaviour {

    private Animator _anime;
    [SerializeField] private GameObject RopeMachine;
    [SerializeField] private GameObject WhackAMole;
    [Space]
    [SerializeField] private GameObject _jumpRopeSpawn;
    [Space]
    [SerializeField] private GameObject _buttonOne;
    [SerializeField] private GameObject _buttonTwo;

    private GameObject _pet;
    private Animator _petAnime;
    private CameraShake _camShake;
    private bool _jumpGame;

    private bool _fallTroughGround;
    private float _fallTimer;
    private bool _playedToday;

    private CameraInterior _camInterior;

    private void Start()
    {
        _anime = GetComponent<Animator>();
        RopeMachine.SetActive(false);
        WhackAMole.SetActive(false);
        _buttonOne.SetActive(false);
        _buttonTwo.SetActive(false);
    }

    private void Update()
    {
        if (_fallTroughGround)
        {
            _pet.transform.position = _jumpRopeSpawn.transform.position;
            _pet.transform.rotation = _jumpRopeSpawn.transform.rotation;
            print(_pet.transform.position);
            _fallTimer += Time.deltaTime;
            if (_fallTimer > 1.5f) _fallTroughGround = false;
        }
        if (GameManager.Instance != null && GameManager.Instance.DayNightCycle != null && 
            Math.Abs(GameManager.Instance.DayNightCycle.TimeNormalized() - 0.3f) < Mathf.Epsilon) _playedToday = false;

        //if (GameManager.Instance.GameRules.Days > 2 && (!_buttonOne.activeSelf || !_buttonTwo.activeSelf))
        //{
        //    _buttonOne.SetActive(true);
        //    _buttonTwo.SetActive(true);
        //}
    }

    public void StartJumpRope()
    {
        if (!_playedToday && GameManager.Instance.GameRules.Days > 2)
        {
            if (GameManager.Instance.Pet != null) _pet = GameManager.Instance.Pet.gameObject;
            if (_camInterior == null) _camInterior = FindObjectOfType<CameraInterior>();
            StartCoroutine(JumpDelay());

            _jumpGame = true;
            _playedToday = true;

            _anime.SetBool("IsOpen", true);

            StartCoroutine(EnableDelayRope());

            StartCoroutine(MyCoroutines.WaitOneFrame(() => ShakeCamera()));
        }
    }

    public void StartGuacamole()
    {
        if (!_playedToday && GameManager.Instance.GameRules.Days > 2)
        {
            if (GameManager.Instance.Pet != null) _pet = GameManager.Instance.Pet.gameObject;

            _jumpGame = false;
            _playedToday = true;

            if (_camInterior == null) _camInterior = FindObjectOfType<CameraInterior>();

            StartCoroutine(JumpDelay());

            _anime.SetBool("IsOpen", true);

            StartCoroutine(EnableDelayGuac());

            StartCoroutine(MyCoroutines.WaitOneFrame(() => ShakeCamera()));
        }
    }

    private void ShakeCamera()
    {
        if (_camShake == null) _camShake = FindObjectOfType<CameraShake>();

        //_camShake.Duration = _anime.GetCurrentAnimatorStateInfo(0).length;
        _camShake.ShakeScale = Vector3.one;
        _camShake.Shake(7, 1);
    }

    public void CloseHatch()
    {
        if (_anime == null) return;
        _anime.SetBool("IsOpen", false);
    }

    private IEnumerator GravityCD()
    {
        while (true)
        {
            yield return new WaitForSeconds(_petAnime.GetCurrentAnimatorStateInfo(0).length);

            //GameManager.Instance.Pet.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            _pet.GetComponent<Rigidbody>().useGravity = false;
            _pet.GetComponent<Rigidbody>().freezeRotation = true;

            if (_jumpGame) _fallTroughGround = true;
            else _pet.transform.position = Vector3.down * 420;

            if (_jumpGame) StartCoroutine(RopeMachine.GetComponentInChildren<RopeMiniGame>().WaitForAnimation());
            else StartCoroutine(WhackAMole.GetComponentInChildren<WhackAMole>().WaitForAnimation());

            break;
        }
    }

    private IEnumerator JumpDelay()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            if (_pet != null)
            {
                _petAnime = _pet.GetComponentInChildren<Animator>();

                if (_petAnime != null) _petAnime.SetTrigger("MiniGameStart");
                if (_camInterior != null) _camInterior._state = State.Minigame;
                _pet.GetComponent<Rigidbody>().isKinematic = false;
                _pet.GetComponent<Rigidbody>().useGravity = true;
                _pet.GetComponent<Rigidbody>().AddForce(new Vector3(0, 6, -6.5f), ForceMode.VelocityChange);
                StartCoroutine(GravityCD());
            }
            break;
        }
    }

    private IEnumerator EnableDelayRope()
    {
        yield return new WaitForSeconds(1f);
        WhackAMole.SetActive(false);
        RopeMachine.SetActive(true);
    }

    private IEnumerator EnableDelayGuac()
    {
        yield return new WaitForSeconds(1f);
        RopeMachine.SetActive(false);
        WhackAMole.SetActive(true);
    }
}
