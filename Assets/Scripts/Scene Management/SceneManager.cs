using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TouchScript.Layers.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using SM = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : MonoBehaviour
{
    //[SerializeField, ReadOnly]
    private bool _canLoad = true;
    private bool _canContinue = true;
    private bool _readyToLoad = true;
    private string _sceneNameBeingLoaded;

    //[SerializeField]
    public bool _useLoadingScreen = true;

    //[SerializeField]
    public float _waitTime = 5;

    //[SerializeField]
    public List<LoadingScreenBehaviour> _loading = new List<LoadingScreenBehaviour>();

    public static Action<Scene, LoadSceneMode> onSceneLoaded;
    public static Action<Scene> onSceneUnloaded;

    public float WaitTime
    {
        get { return _waitTime; }
        set { _waitTime = value; }
    }

    public bool UseLoadingScreen
    {
        get { return _useLoadingScreen; }
        set { _useLoadingScreen = value; }
    }

    public bool ReadyToLoad
    {
        get { return _readyToLoad; }
        set { _readyToLoad = value; }
    }

    // Use this for initialization
    private void Start()
    {
        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
            GameManager.SceneManager = this));
        //GameManager.SceneManager = this;
        //DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);

        //SM.sceneLoaded += (scene, mode) => _canLoad = true;
        SM.sceneLoaded += OnSceneLoadedDelegate;
        SM.sceneUnloaded += OnSceneUnloadedDelegate;
        SM.sceneLoaded += CleanupPlaying;
        Application.quitting += CleanupDestroy;
        //Application.wantsToQuit += CleanupDestroyB;
    }

    private void OnDisable()
    {

        SM.sceneLoaded -= OnSceneLoadedDelegate;
        SM.sceneUnloaded -= OnSceneUnloadedDelegate;
        SM.sceneLoaded -= CleanupPlaying;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            //if (GameManager.Instance != null)
            GameManager.Instance.Unpause();

            for (int i = _loading.Count - 1; i >= 0; i--)
                if (_loading[i] != null)
                    Destroy(_loading[i].transform.root.gameObject);
            _loading.Clear();
        }

        SM.sceneLoaded -= OnSceneLoadedDelegate;
        SM.sceneUnloaded -= OnSceneUnloadedDelegate;
        SM.sceneLoaded -= CleanupPlaying;
    }

    private void CleanupDestroy()
    {
        if (GameManager.Instance != null)
        {
            //if (GameManager.Instance != null)
            GameManager.Instance.Unpause();

            for (int i = _loading.Count - 1; i >= 0; i--)
                if (_loading[i] != null)
                    Destroy(_loading[i].transform.root.gameObject);
            _loading.Clear();
        }
    }

    private void CleanupPlaying(Scene scene, LoadSceneMode mode)
    {
        var whatever = FindObjectsOfType<SceneManager>();
        //if (whatever.Length >= 2 && gameObject.scene.name == "DontDestroyOnLoad") Destroy(gameObject);

        if (whatever.Length >= 2)
            CleanupDestroy();

        for (int i = whatever.Length - 1; i >= 0; i--)
        {
            SceneManager newSceneManager = whatever[i];

            if (newSceneManager != this)
            {
                _loading = newSceneManager._loading;
                _waitTime = newSceneManager._waitTime;
                _useLoadingScreen = newSceneManager._useLoadingScreen;
                Destroy(newSceneManager.gameObject);
            }
        }

        GameManager.SceneManager = this;
    }

    private static void OnSceneLoadedDelegate(Scene scene, LoadSceneMode mode)
    {
        if (onSceneLoaded != null)
            onSceneLoaded(scene, mode);
    }

    private static void OnSceneUnloadedDelegate(Scene scene)
    {
        if (onSceneUnloaded != null)
            onSceneUnloaded(scene);
    }

    #region Loading Screens
    public void AddLoadingScreen(LoadingScreenBehaviour lsb)
    {
        _loading.Add(lsb);
        lsb.gameObject.SetActive(false);
    }

    public bool RemoveLoadingScreen(LoadingScreenBehaviour lsb)
    {
        return _loading.Remove(lsb);
    }

    public void ClearLoadingScreens()
    {
        _loading.Clear();
    }
    #endregion

    #region Loading
    #region Loading Wrappers
    public void LoadSceneSingle(int buildIndex)
    {
        LoadScene(buildIndex, LoadSceneMode.Single);
    }

    public void LoadSceneSingle(string sceneName)
    {
        LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void LoadSceneAdditive(int buildIndex)
    {
        LoadScene(buildIndex, LoadSceneMode.Additive);
    }

    public void LoadSceneAdditive(string sceneName)
    {
        LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void LoadSceneSingleAsync(int buildIndex)
    {
        LoadSceneAsync(buildIndex, LoadSceneMode.Single);
    }

    public void LoadSceneSingleAsync(string sceneName)
    {
        LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    public void LoadSceneAdditiveAsync(int buildIndex)
    {
        LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
    }

    public void LoadSceneAdditiveAsync(string sceneName)
    {
        LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    public void LoadSceneSingleS(int buildIndex)
    {
        LoadScene(buildIndex, LoadSceneMode.Single);
    }

    public void LoadSceneSingleS(string sceneName, out Scene scene)
    {
        scene = LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void LoadSceneAdditiveS(int buildIndex, out Scene scene)
    {
        scene = LoadScene(buildIndex, LoadSceneMode.Additive);
    }

    public void LoadSceneAdditiveS(string sceneName, out Scene scene)
    {
        scene = LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void LoadSceneSingleAsyncS(int buildIndex, out Scene scene)
    {
        scene = LoadSceneAsync(buildIndex, LoadSceneMode.Single);
    }

    public void LoadSceneSingleAsyncS(string sceneName, out Scene scene)
    {
        scene = LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    public void LoadSceneAdditiveAsyncS(int buildIndex, out Scene scene)
    {
        scene = LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
    }

    public void LoadSceneAdditiveAsyncS(string sceneName, out Scene scene)
    {
        scene = LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
    #endregion

    public Scene LoadScene(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (!_canLoad) return default(Scene);
        _canLoad = false;

        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
        {
            GameManager.Instance.Pause();
            SM.LoadScene(buildIndex, mode);
            GameManager.Instance.Pause();

            if (UseLoadingScreen)
            {
                foreach (var lsb in _loading)
                {
                    lsb.gameObject.SetActive(true);
                    lsb.transform.root.gameObject.SetActive(true);

                    lsb.Requirements();
                    var lsb1 = lsb;
                    StartCoroutine(MyCoroutines.Wait(_waitTime, () =>
                    {
                        _canLoad = true;
                        lsb1.gameObject.SetActive(false);
                        GameManager.Instance.Unpause();
                    }));
                }
            }
            GameManager.Instance.Unpause();
        }));

        return SM.GetSceneByBuildIndex(buildIndex);
    }

    public Scene LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (!_canLoad) return default(Scene);
        _canLoad = false;

        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
        {
            GameManager.Instance.Pause();
            SM.LoadScene(sceneName, mode);
            GameManager.Instance.Pause();

            if (UseLoadingScreen)
            {
                foreach (var lsb in _loading)
                {
                    lsb.gameObject.SetActive(true);
                    lsb.transform.root.gameObject.SetActive(true);
                    lsb.Requirements();
                    var lsb1 = lsb;
                    StartCoroutine(MyCoroutines.Wait(_waitTime, () =>
                    {
                        _canLoad = true;
                        lsb1.gameObject.SetActive(false);
                        GameManager.Instance.Unpause();
                    }));
                }
            }
            GameManager.Instance.Unpause();
        }));

        return SM.GetSceneByName(sceneName);
    }

    public Scene LoadSceneAsync(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (!_canLoad) return default(Scene);
        _canLoad = false;

        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
        {
            GameManager.Instance.Pause();
            _canContinue = false;
            StartCoroutine(MyCoroutines.Wait(_waitTime, () =>
            {
                _canContinue = true;
                _canLoad = true;
            }));
            StartCoroutine(LoadSceneAsyncCoroutine(buildIndex, mode));
        }));

        return SM.GetSceneByBuildIndex(buildIndex);
    }

    public Scene LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (!_canLoad)
        {
            Debug.Log("Already loading another scene: " + _sceneNameBeingLoaded);
            return default(Scene);
        }
        _canLoad = false;
        _sceneNameBeingLoaded = sceneName;

        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
        {
            GameManager.Instance.Pause();
            _canContinue = false;
            StartCoroutine(MyCoroutines.Wait(_waitTime, () =>
            {
                _canContinue = true;
                _canLoad = true;
                _sceneNameBeingLoaded = "";
            }));
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName, mode));
        }));

        return SM.GetSceneByName(sceneName);
    }

    private IEnumerator LoadSceneAsyncCoroutine(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation asyncOperation = SM.LoadSceneAsync(buildIndex, mode);
        GameManager.Instance.Pause();
        asyncOperation.allowSceneActivation = false;

        //Debug.Log("Use: " + UseLoadingScreen);

        if (UseLoadingScreen && _loading.Count != 0)
        {
            foreach (var lsb in _loading)
            {
                //if (lsb == null) continue;
                lsb.gameObject.SetActive(true);
                lsb.transform.root.gameObject.SetActive(true);
                lsb.Requirements();
            }
        }

        while (!asyncOperation.isDone)
        {
            if (UseLoadingScreen && _loading.Count != 0)
                _loading.ForEach(lsb => lsb.LoadingUpdate());

            if (asyncOperation.progress >= 0.9f && _canContinue && _readyToLoad)
            {
                asyncOperation.allowSceneActivation = true;
                GameManager.Instance.Unpause();
            }
            yield return null;
        }

        if (asyncOperation.isDone && _readyToLoad)
        {
            asyncOperation.allowSceneActivation = true;
            GameManager.Instance.Unpause();
        }

        //asyncOperation.allowSceneActivation = true;

        //if (_loading.Count != 0)
        //    _loading.ForEach(lsb => lsb.gameObject.SetActive(false));

        yield return null;
    }


    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation asyncOperation = SM.LoadSceneAsync(sceneName, mode);
        GameManager.Instance.Pause();
        asyncOperation.allowSceneActivation = false;

        //Debug.Log("Use: " + _useLoadingScreen);
        //Debug.Log("Loading Screen: " + _rotatingImageLoading.gameObject);
        if (UseLoadingScreen && _loading.Count != 0)
        {
            foreach (var lsb in _loading)
            {
                //if (lsb == null) continue;

                lsb.gameObject.SetActive(true);
                lsb.transform.root.gameObject.SetActive(true);
                lsb.Requirements();
            }
        }

        while (!asyncOperation.isDone)
        {
            if (UseLoadingScreen && _loading.Count != 0)
                _loading.ForEach(lsb => lsb.LoadingUpdate());

            if (asyncOperation.progress >= 0.9f && _canContinue && _readyToLoad)
            {
                asyncOperation.allowSceneActivation = true;
                GameManager.Instance.Unpause();
            }
            yield return null;
        }

        if (asyncOperation.isDone && _readyToLoad)
        {
            asyncOperation.allowSceneActivation = true;
            GameManager.Instance.Unpause();
        }


        //asyncOperation.allowSceneActivation = true;

        //StartCoroutine(MyCoroutines.WaitOneFrame(()=>{))
        //for (int i = _loading.Count - 1; i >= 0; i--)
        //    Destroy(_loading[i].gameObject);

        yield return null;
    }
    #endregion
}
