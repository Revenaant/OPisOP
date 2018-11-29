using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform cam;

    public bool followCamera = true;

    public bool lookAtX = true;
    public bool lookAtY = true;
    public bool lookAtZ = true;

    private void Awake()
    {
        cam = Camera.main.transform;
        //cam = GameObject.FindObjectOfType<Camera>().transform;
    }

    private void Update()
    {
        Vector3 lookDirection = followCamera ? (cam.position - transform.position).normalized : -cam.forward;
        //Vector3 lookPos = cam.position - transform.position;
        //Vector3 lookPos = -cam.forward;

        if (!lookAtX) lookDirection.x = 0;
        if (!lookAtY) lookDirection.y = 0;
        if (!lookAtZ) lookDirection.z = 0;

        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);
    }
}
