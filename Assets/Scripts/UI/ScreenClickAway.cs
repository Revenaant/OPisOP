using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript;

public class ScreenClickAway : MonoBehaviour
{
    private ActionPanel[] _actionPanels;

	// Use this for initialization
	private void Start ()
    {
        _actionPanels = FindObjectsOfType<ActionPanel>();

        StartCoroutine(MyCoroutines.WaitForEndOfFrame(() => { gameObject.SetActive(false); }));
    }

    public void DeactivatePanels()
    {
        for(int i = 0; i < _actionPanels.Length; i++)
        {
            if (_actionPanels[i].gameObject.activeInHierarchy)
                _actionPanels[i].Activated = false;
        }
        gameObject.SetActive(false);
    }
}
