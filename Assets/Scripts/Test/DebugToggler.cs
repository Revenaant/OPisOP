using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugToggler : MonoBehaviour {

    private void Start()
    {
        GameManager.Instance.DisplayDebug = false;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.D))
            GameManager.Instance.DisplayDebug = !GameManager.Instance.DisplayDebug;

    }
}
