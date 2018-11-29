using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject loadScreen;

    private VideoPlayer player;
    private SceneManager manager;

    // Use this for initialization
    void Start()
    {
        player = GetComponent<VideoPlayer>();
        manager = GameManager.SceneManager;

        manager.ReadyToLoad = false;
        manager.LoadSceneSingleAsync(sceneToLoad);


        player.prepareCompleted += (o) => loadScreen.SetActive(false);
        player.loopPointReached += (o) => manager.ReadyToLoad = true;
    }

    public void Skip()
    {
        player.frame = (long)player.frameCount;
    }
}
