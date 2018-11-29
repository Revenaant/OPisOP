using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticFunctions : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.StaticFunctions = this;
    }

    public void ApplicationQuit()
    {
        StartCoroutine(MyCoroutines.WaitForEndOfFrame(GameManager.ApplicationQuit));
        //GameManager.Instance.ApplicationQuit();
        //Application.Quit();
    }

    public void Pause()
    {
        GameManager.Instance.Pause();
    }

    public void Unpause()
    {
        GameManager.Instance.Unpause();
    }

    public void TogglePause()
    {
        GameManager.Instance.TogglePause();
    }

    public void RestartGame()
    {
        StartCoroutine(MyCoroutines.WaitForEndOfFrame(GameManager.RestartGame));
    }

    public void EndGame()
    {
        StartCoroutine(MyCoroutines.WaitForEndOfFrame(GameManager.Instance.OnEnd));
    }

    public void GoToScene(string scene)
    {
        GameManager.SceneManager.LoadSceneSingleAsync(scene);
    }
}
