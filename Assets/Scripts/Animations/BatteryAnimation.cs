using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryAnimation : MonoBehaviour {

    private Animator _anime;

    private void Start ()
    {
        _anime = GetComponent<Animator>();	
	}

    private void Update ()
    {
        if (_anime == null || GameManager.Instance == null || GameManager.Instance.Storage == null) return;

        _anime.SetBool("Overheating", GameManager.Instance.Storage.Overcharging);

        if (GameManager.Instance.Storage.CurrentUpgradeLevel == 1) _anime.SetBool("UpgradedOnce", true);
        if (GameManager.Instance.Storage.CurrentUpgradeLevel == 2) _anime.SetBool("UpgradedTwice", true);
    }

    public void BatteryTap()
    {
        _anime.SetTrigger("BatteryTap");
    }
}
