using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menupetanim : MonoBehaviour {

    private Animator _anime;
    private float _rnd;
    private float _idleChangeTimer;
    public GameObject _Reye;
    public GameObject _Leye;

    void Start () {

        _anime = GetComponent<Animator>();
    }

    private void Update()
    {
        _idleChangeTimer += Time.deltaTime;

        if (_idleChangeTimer > Random.Range(10, 21))
        {
            _rnd = Random.Range(1, 4);

            _idleChangeTimer = 0;
            _anime.SetTrigger("IdleAction" + _rnd);
        }
    }

    public void Interacted()
    {
        if (!_anime.GetCurrentAnimatorStateInfo(0).IsName("Idle 1")) return;

        _rnd = Random.Range(1, 4);
        _idleChangeTimer = 0;
        _anime.SetTrigger("IdleAction" + _rnd);
    }

    public void WakeUp()
    {
        _anime.SetTrigger("tired");
        _Reye.SetActive(false);
        _Leye.SetActive(false);
    }

}
