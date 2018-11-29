using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetForWhaking : MonoBehaviour {

    private bool _hit;
    private int _score;
    private float _timer;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    private void Update()
    {
        //if (_hit)
        //{
        //    _timer += Time.deltaTime;
        //    if(_timer)
        //}
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Suppliment")
        {
            _hit = true;
        }
    }

    public bool Hit
    {
        get { return _hit; }
        set { _hit = value; }
    }
}
