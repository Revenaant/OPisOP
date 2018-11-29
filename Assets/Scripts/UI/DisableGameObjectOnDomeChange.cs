using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameObjectOnDomeChange : MonoBehaviour
{
    public GameObject gameObjectToDisable = null;

    private void OnEnable()
    {
        GameManager.Instance.onDomeChange += Handler;
    }

    private void OnDisable()
    {
        if (GameManager.Instance.onDomeChange != null)
            GameManager.Instance.onDomeChange -= Handler;
    }

    private void Handler(bool value)
    {
        if (gameObjectToDisable != null)
            gameObjectToDisable.SetActive(!value);
    }
}
