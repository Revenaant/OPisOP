using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionScreenEnable : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objectsToDisable = null;
    [SerializeField] private GameObject _screen = null;
    
    private void OnEnable()
    {
        GameRules.onEnd += Switch;
    }

    private void OnDisable()
    {
        if (GameRules.onEnd != null)
            GameRules.onEnd -= Switch;
    }

    private void Switch()
    {
        //Debug.Log("YO");
        foreach (GameObject go in _objectsToDisable)
            go.SetActive(false);
        _screen.SetActive(true);
    }
}
