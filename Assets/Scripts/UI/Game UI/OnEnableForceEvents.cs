using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableForceEvents : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.ForceEvents();
    }
}
