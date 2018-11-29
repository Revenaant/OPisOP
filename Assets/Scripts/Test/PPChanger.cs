using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PPChanger : MonoBehaviour
{
    public float saturation = 5f;
        ColorGrading grading = null;

	// Use this for initialization
	void Start () {
        PostProcessVolume volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out grading);


	}
	
	// Update is called once per frame
	void Update () {
		//if(Input.GetKey(KeyCode.O))
        {
            grading.saturation.value = saturation;
        }
	}
}
