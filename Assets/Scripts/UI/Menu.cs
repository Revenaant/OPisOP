using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    [SerializeField] private string _levelToLoad = null;

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject CreditsMenu;
    [SerializeField] private GameObject LeaderboardMenu;
    [SerializeField] private GameObject FeedbackMenu;

    //[SerializeField] private RotatingImageLoading _loadingScreen = null;

    public void Start()
    {
        if (LeaderboardMenu != null)
            LeaderboardMenu.SetActive(false);
        if (FeedbackMenu != null)
            FeedbackMenu.SetActive(false);
        if (CreditsMenu != null)
            CreditsMenu.SetActive(false);
        if (MainMenu != null)
            MainMenu.SetActive(true);
    }

    public void StartGame()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene(_levelToLoad);
        //GameManager.SceneManager.AddLoadingScreen(_loadingScreen);
        //GameManager.SceneManager.UseLoadingScreen = true;
        GameManager.SceneManager.LoadSceneAsync(_levelToLoad);

        FindObjectOfType<Canvas>().gameObject.SetActive(false);
        //Destroy(gameObject);
        Destroy(transform.root.gameObject);
    }

    public void Credits()
    {
        if (MainMenu != null)
            MainMenu.SetActive(false);
        if (FeedbackMenu != null)
            FeedbackMenu.SetActive(false);
        if (LeaderboardMenu != null)
            LeaderboardMenu.SetActive(false);
        if (CreditsMenu != null)
            CreditsMenu.SetActive(true);
    }

    public void LeaderBoards()
    {
        if (MainMenu != null)
            MainMenu.SetActive(false);
        if (FeedbackMenu != null)
            FeedbackMenu.SetActive(false);
        if (CreditsMenu != null)
            CreditsMenu.SetActive(false);
        if (LeaderboardMenu != null)
            LeaderboardMenu.SetActive(true);
    }

    public void Back()
    {
        if (LeaderboardMenu != null)
            LeaderboardMenu.SetActive(false);
        if (FeedbackMenu != null)
            FeedbackMenu.SetActive(false);
        if (CreditsMenu != null)
            CreditsMenu.SetActive(false);
        if (MainMenu != null)
            MainMenu.SetActive(true);
    }

    public void Feedback()
    {
        if (LeaderboardMenu != null)
            LeaderboardMenu.SetActive(false);
        if (FeedbackMenu != null)
            FeedbackMenu.SetActive(true);
        if (CreditsMenu != null)
            CreditsMenu.SetActive(false);
        if (MainMenu != null)
            MainMenu.SetActive(false);
    }
}
