using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRenderers : MonoBehaviour
{
    public void Toggle(bool value)
    {
        MeshRenderer myRend = GetComponent<MeshRenderer>();
        if (myRend != null) myRend.enabled = value;

        if (transform.childCount <= 0) return;
        MeshRenderer[] rends = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].enabled = value;
        }
    }
}
