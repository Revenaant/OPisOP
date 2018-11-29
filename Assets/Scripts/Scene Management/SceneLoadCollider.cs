using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadCollider : MonoBehaviour
{
    private CameraControls _cameraExterior;
    private CameraInterior _cameraInterior;

#pragma warning disable 0414
    private Scene _scene;
#pragma warning restore 0414

    [SerializeField] private string _sceneName = "Interior Load Test";
    [SerializeField] List<ToggleRenderers> _rendToggles;

    private void Start()
    {
        _cameraExterior = FindObjectOfType<CameraControls>();
        Debug.Assert(_cameraExterior != null, "Could not find CameraControls in " + gameObject.name);
        _cameraInterior = _cameraExterior.GetComponent<CameraInterior>();
        Debug.Assert(_cameraInterior != null, "Could not find CameraInterior in " + gameObject.name);

        _rendToggles.Add(GetComponent<ToggleRenderers>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Camera>() != null)
        {
            Load();
            //_cameraExterior.enabled = false;
            //_cameraInterior.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Camera>() != null)
        {
            Unload();
            //_cameraInterior.enabled = false;
            //_cameraExterior.enabled = true;
            //_cameraExterior.ResetRotation();
        }
    }

    private void Load()
    {
        GameManager.SceneManager.LoadSceneAdditiveAsyncS(_sceneName, out _scene);
        GameManager.Instance.MainCamera.fieldOfView = 20;

        //float fov = GameManager.Instance.MainCamera.fieldOfView;

        //StartCoroutine(MyCoroutines.DoUntil(() => Math.Abs(fov - 20) < Mathf.Epsilon, () =>
        //{
        //    GameManager.Instance.MainCamera.fieldOfView = Mathf.Lerp(fov, 20, 0.75f * Time.deltaTime);

        //    // Snap when close enough to prevent annoying slerp behavior
        //    if ((20 - fov) <= float.Epsilon)
        //    {
        //        GameManager.Instance.MainCamera.fieldOfView = 20;
        //        fov = 20;
        //    }

        //}));

        GameManager.Instance.InDome = true;

        if (_rendToggles.Count > 0)
        {
            for (int i = 0; i < _rendToggles.Count; i++)
                _rendToggles[i].Toggle(false);
        }
    }

    private void Unload()
    {
        //UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Interior Load Test");
        //if (_scene.IsValid())
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_sceneName);
        GameManager.Instance.InDome = false;
        GameManager.Instance.MainCamera.fieldOfView = 53;

        //float fov = GameManager.Instance.MainCamera.fieldOfView;

        //StartCoroutine(MyCoroutines.DoUntil(() => Math.Abs(fov - 53) < Mathf.Epsilon, () =>
        //{
        //    GameManager.Instance.MainCamera.fieldOfView = Mathf.Lerp(fov, 53, 0.75f * Time.deltaTime);

        //    // Snap when close enough to prevent annoying slerp behavior
        //    if ((53 - fov) <= float.Epsilon)
        //    {
        //        GameManager.Instance.MainCamera.fieldOfView = 53;
        //        fov = 53;
        //    }

        //}));

        if (_rendToggles.Count > 0)
        {
            for (int i = 0; i < _rendToggles.Count; i++)
                _rendToggles[i].Toggle(true);
        }
    }
}
