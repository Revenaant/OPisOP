using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchParticleManager : MonoBehaviour
{
    private Camera cam;

    public UnityEvent OnTap;

	// Use this for initialization
	void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        Spawn();
    }

    public void Spawn()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Ended) return;

            if(OnTap != null) OnTap.Invoke();

            RaycastHit hit = GetRayHit(new Vector3(touch.position.x, touch.position.y));
            if (hit.collider == null) return;
            FeedbackParticles data = hit.collider.GetComponent<FeedbackParticles>();

            if (data != null && data.Particle != null)
                Instantiate(data.Particle, hit.point, Quaternion.identity);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit = GetRayHit(Input.mousePosition);
            if (hit.collider == null) return;
            FeedbackParticles data = hit.collider.GetComponent<FeedbackParticles>();

            if (data != null && data.Particle != null)
                Instantiate(data.Particle, hit.point, Quaternion.identity);
        }
    }

    private RaycastHit GetRayHit(Vector3 screenPos)
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(screenPos);
        Physics.Raycast(ray, out hit);

        return hit;
    }
}
