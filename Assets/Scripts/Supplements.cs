using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Behaviors;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;

public class Supplements : MonoBehaviour
{
    private TransformGesture gesture;
    private Transformer transformer;
    private Rigidbody rb;

    [SerializeField]
    private GameObject _particle;

    private Animator _anime;

    public Rigidbody Rb
    {
        get { return rb; }
        set { rb = value; }
    }

    public Transformer Transformer
    {
        get { return transformer; }
        set { transformer = value; }
    }

    private void OnEnable()
    {
        // The gesture
        gesture = GetComponent<TransformGesture>();
        // Transformer component actually MOVES the object
        Transformer = GetComponent<Transformer>();
        rb = GetComponent<Rigidbody>();

        Transformer.enabled = false;
        rb.isKinematic = false;
        // Subscribe to gesture events
        gesture.TransformStarted += TransformStartedHandler;
        gesture.TransformCompleted += TransformCompletedHandler;
    }

    private void getAnime()
    {
        if (GameManager.Instance.Pet != null)
            _anime = GameManager.Instance.Pet.gameObject.GetComponentInChildren<Animator>();
    }

    private void OnDisable()
    {
        // Unsubscribe from gesture events
        gesture.TransformStarted -= TransformStartedHandler;
        gesture.TransformCompleted -= TransformCompletedHandler;
    }

    private void TransformStartedHandler(object sender, EventArgs e)
    {
        // When movement starts we need to tell physics that now WE are moving this object manually
        getAnime();
        if (_anime != null && GameManager.Instance.Pet.StateOfPet != PetState.Sleep)_anime.SetTrigger("HoldingFood");
        rb.isKinematic = true;
        Transformer.enabled = true;
    }

    private void TransformCompletedHandler(object sender, EventArgs e)
    {
        //print("BUTCH");
        Transformer.enabled = false;
        rb.isKinematic = false;
        getAnime();
        if (_anime != null && GameManager.Instance.Pet.StateOfPet != PetState.Sleep) _anime.SetTrigger("DroppedFood");
        rb.WakeUp();
    }
    
    public void SpawnParticles()
    {
        Instantiate(_particle, transform.position, Quaternion.identity);
    }
}
