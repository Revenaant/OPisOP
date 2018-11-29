using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitchForMiniGames : MonoBehaviour
{

    public float speedFactor = 0.1f;
    public float zoomFactor = 1.0f;
    private Transform CurrentMount;
    public Transform DefaultMount;
    private Vector3 lastPosition;
    private Camera cameraComp;

    private void Awake()
    {
        CurrentMount = DefaultMount;
        lastPosition = transform.position;
        cameraComp = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, CurrentMount.position, speedFactor);
        transform.rotation = Quaternion.Slerp(transform.rotation, CurrentMount.rotation, speedFactor);
        float velocity = Vector3.Magnitude(transform.position - lastPosition);
        //cameraComp.fieldOfView = 60 + velocity * zoomFactor;
        lastPosition = transform.position;
    }

    public void SetMount(Transform newMount)
    {
        CurrentMount = newMount;
    }

    public Transform GetCM
    {
        get { return CurrentMount; }
    }
}