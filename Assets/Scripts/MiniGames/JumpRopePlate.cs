using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpRopePlate : MonoBehaviour {

    //[SerializeField] private Storage _battery;
    private RopeMiniGame _ropeGame;
    // Use this for initialization
    void Start()
    {
        _ropeGame = FindObjectOfType<RopeMiniGame>();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Pet")
        {
            if (GameManager.Instance.Storage != null) GameManager.Instance.Storage.Energy += 10f;
            _ropeGame.RopeSpeed += 0.2f;
        }
    }
}
