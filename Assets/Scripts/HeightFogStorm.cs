using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HeightFogStorm : MonoBehaviour
{
    private enum StartState { DoNothing, GoToMinAlpha, GoToMaxAlpha }

    private Material _heightFogMaterial = null;

    [SerializeField] private StartState _onStart = StartState.GoToMaxAlpha;
    [SerializeField, Range(0, 1)] private float _alpha = 1;

    [SerializeField, Range(0, 1)] private float _maxAlpha = 1;
    [SerializeField] private TweenElement _fadeIn = new TweenElement(0.5f, Ease.Linear);
    [SerializeField, Range(0, 1)] private float _minAlpha = 0;
    [SerializeField] private TweenElement _fadeOut = new TweenElement(0.5f, Ease.Linear);

    private void OnValidate()
    {
        if (_heightFogMaterial == null)
            _heightFogMaterial = GetComponent<Renderer>().sharedMaterial;
        UpdateMaterialAlpha();
    }

    // Use this for initialization
    private void Awake()
    {
        _heightFogMaterial = GetComponent<Renderer>().sharedMaterial;
    }

    private void Start()
    {
        if (_onStart == StartState.GoToMinAlpha)
        {
            _alpha = _minAlpha;
            UpdateMaterialAlpha();
        }

        if (_onStart == StartState.GoToMaxAlpha)
        {
            _alpha = _maxAlpha;
            UpdateMaterialAlpha();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(MyCoroutines.WaitOneFrame(() =>
            {
                if (GameManager.Instance != null && GameManager.Instance.Storms != null)
                {
                    //Debug.Log("SO");
                    GameManager.Instance.Storms.onStormStart.AddListener(StormStart);
                    GameManager.Instance.Storms.onStormEnd.AddListener(StormEnd);
                }
            }
        ));
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.Storms != null)
        {
            //Debug.Log("GAY");
            GameManager.Instance.Storms.onStormStart.RemoveListener(StormStart);
            GameManager.Instance.Storms.onStormEnd.RemoveListener(StormEnd);
        }
    }

    private void StormStart()
    {
        //Debug.Log("Start");
        DOTween.To(() => _alpha, x => _alpha = x, _maxAlpha, _fadeIn.duration).SetEase(_fadeIn.ease).SetAutoKill(true).SetRecyclable();
    }

    private void StormEnd()
    {
        //Debug.Log("End");
        DOTween.To(() => _alpha, x => _alpha = x, _minAlpha, _fadeOut.duration).SetEase(_fadeOut.ease).SetAutoKill(true).SetRecyclable();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateMaterialAlpha();
    }

    private void UpdateMaterialAlpha()
    {
        if (_heightFogMaterial != null)
            _heightFogMaterial.SetFloat("_Alpha", _alpha);
    }
}
