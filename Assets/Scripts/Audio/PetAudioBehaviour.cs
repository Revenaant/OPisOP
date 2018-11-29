using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pet), typeof(PlayAfterDead))]
public class PetAudioBehaviour : MonoBehaviour
{
    private Pet pet;
    private PlayAfterDead source;

    [SerializeField] private AudioClip jumpUpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip eatClip;

    // Use this for initialization
    void Awake()
    {
        pet = GetComponent<Pet>();
        source = GetComponent<PlayAfterDead>();
    }

    private void OnEnable()
    {
        pet.OnJump += PlayJump;
        pet.OnLand += PlayLand;
        pet.OnStartEat += PlayEat;
    }

    private void OnDisable()
    {
        pet.OnJump -= PlayJump;
        pet.OnLand -= PlayLand;
        pet.OnStartEat -= PlayEat;
    }

    private void PlayJump()
    {
        source.PlayClip(jumpUpClip);
    }

    private void PlayLand()
    {
        source.PlayClip(landClip);
    }

    private void PlayEat()
    {
        print("eating sound");
        source.PlayClip(eatClip);
    }
}
