using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RestartCanvas : MonoBehaviour
{
#pragma warning disable 0414
    [SerializeField] private EventSystem _eventSystem = null;
#pragma warning restore 0414

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (_eventSystem != null)
        //{
        //    if (Input.GetKeyDown(KeyCode.RightShift)) _eventSystem.gameObject.SetActive(false);
        //    if (Input.GetKeyDown(KeyCode.RightControl)) _eventSystem.gameObject.SetActive(true);
        //    if (Input.GetKeyDown(KeyCode.RightArrow)) _eventSystem.UpdateModules();
        //}
    }
}
