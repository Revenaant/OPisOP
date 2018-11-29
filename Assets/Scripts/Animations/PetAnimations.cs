using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetAnimations : MonoBehaviour {

    private Animator _anime;
    private float _rnd;
    private float _idleChangeTimer;
    private Pet _pet;

    private void Start ()
    {
        _pet = GetComponentInParent<Pet>();
    }

    private void Update ()
    {
        if(_anime == null) _anime = GetComponentInChildren<Animator>();

        IdleAnimation();
        SadAnimation();
        SickAnimation();
        TiredAnimation();
        TooCold();
        TooHot();
        Full();
    }

    private void IdleAnimation()
    {
        if (_anime.GetBool("Sad") || _anime.GetBool("Sick")
            || _anime.GetBool("Tired")) return;

        _idleChangeTimer += Time.deltaTime;

        if (_idleChangeTimer > Random.Range(10, 21))
        {
            ResetTriggers();

            if (_pet.Happiness > 70) _rnd = Random.Range(1, 5);
            else if (_pet.Hunger > 50)
            {
                _rnd = Random.Range(1, 6);
                if (_rnd == 4) _rnd = 5;
            }
            else _rnd = Random.Range(1, 4);

            _idleChangeTimer = 0;
            _anime.SetTrigger("IdleAction" + _rnd);
        }
    }

    private void SadAnimation()
    {
        if (_pet.Happiness < 40) _anime.SetBool("Sad", true);
        else _anime.SetBool("Sad", false);

        if (!_anime.GetBool("Sad")) return;

        _idleChangeTimer += Time.deltaTime;

        if (_idleChangeTimer > Random.Range(10, 21))
        {
            ResetTriggers();

            _idleChangeTimer = 0;
            _rnd = Random.Range(1, 4);

            if (_pet.Hunger > 50 && _rnd > 1)
            {
                _anime.SetTrigger("IdleAction5");
                return;
            }

            _anime.SetTrigger("SadIdleAction");
        }
    }

    private void SickAnimation()
    {
        if (GameManager.Instance.PollutionManager != null)
        {
            if (GameManager.Instance.PollutionManager.PollutionDamage > 0.7f) _anime.SetBool("Sick", true);
            else _anime.SetBool("Sick", false);
        }

        if (!_anime.GetBool("Sick")) return;

        _idleChangeTimer += Time.deltaTime;

        if (_idleChangeTimer > Random.Range(10, 21))
        {
            ResetTriggers();

            _idleChangeTimer = 0;

            _anime.SetTrigger("IdleAction5");
        }
    }

    private void TiredAnimation()
    {
        if (_pet.StateOfPet == PetState.Sleep) _anime.SetBool("Tired", true);
        else _anime.SetBool("Tired", false);

        if (!_anime.GetBool("Tired")) return;
    }

    private void TooCold()
    {
        if (GameManager.Instance.Temperature < _pet.TooColdTemperature) _anime.SetBool("TooCold", true);
        else _anime.SetBool("TooCold", false);
    }

    private void TooHot()
    {
        if (GameManager.Instance.Temperature > _pet.TooHotTemperature) _anime.SetBool("TooHot", true);
        else _anime.SetBool("TooHot", false);
    }

    private void Full()
    {
        if (_pet == null) return;

        if (_pet.Hunger < 20) _anime.SetBool("Full", true);
        else _anime.SetBool("Full", false);
    }

    public void DomeEntered()
    {
        ResetTriggers();
        if (!GameManager.Instance.InDome) _anime.SetTrigger("Wave");
    }

    private void ResetTriggers()
    {
        _anime.ResetTrigger("IdleAction1");
        _anime.ResetTrigger("IdleAction2");
        _anime.ResetTrigger("IdleAction3");
        _anime.ResetTrigger("IdleAction4");
        _anime.ResetTrigger("SadIdleAction");
        _anime.ResetTrigger("IdleAction5");
        _anime.ResetTrigger("Wave");
    }
}
