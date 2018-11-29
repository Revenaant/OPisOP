using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionProbeRefresh : MonoBehaviour
{
    private float _timer = 0;
    //private bool _finished = false;
    private int _renderID = int.MinValue;

    [SerializeField]
    private ReflectionProbe _reflectionProbe = null;
    [SerializeField, Range(0, 10)]
    private float _refreshRate = 1;

    //private IEnumerator Start()
    //{
    //    if (_reflectionProbe == null)
    //        _reflectionProbe = GetComponent<ReflectionProbe>();

    //    while (true)
    //    {
    //        yield return new WaitForSeconds(_refreshRate);

    //        _reflectionProbe.RenderProbe();

    //        yield return null;
    //    }
    //}

    private void OnValidate()
    {
        if (Application.isPlaying)
            _timer = _refreshRate;
    }

    //Use this for initialization
    private void Start()
    {
        if (_reflectionProbe == null)
            _reflectionProbe = GetComponent<ReflectionProbe>();
        _timer = _refreshRate;

        _renderID = _reflectionProbe.RenderProbe();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_reflectionProbe.IsFinishedRendering(_renderID)) return;

        _timer -= Time.deltaTime;

        if (_timer > 0) return;

        _renderID = _reflectionProbe.RenderProbe();
        _timer = _refreshRate;
    }
}
