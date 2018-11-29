using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAtStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<UIFadeElements>().FadeOut();
	}
	
}
