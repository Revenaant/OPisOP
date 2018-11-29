using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    private bool _isOn = false;
    [SerializeField] private float _time;
    [SerializeField] private float _OnIntensity;
    Light[] lights;

    // Use this for initialization
    void Start() {
        lights = GetComponentsInChildren<Light>();
    }

    public void ToggleOnOff(bool value)
    {
        StopAllCoroutines();

        _isOn = true;
        if (value)
            PowerOn();
        else
            PowerOff();
    }

    private void PowerOn()
    {
        foreach (Light light in lights)
            StartCoroutine(lerpIntensity(light, _OnIntensity));
    }

    private void PowerOff()
    {
        foreach (Light light in lights)
            StartCoroutine(lerpIntensity(light, 0.01f));
    }

    public bool isOn
    {
        get { return _isOn; }
        set { _isOn = value; }
    }

    private IEnumerator lerpIntensity(Light light, float targetValue)
    {
        while (light.intensity != targetValue)
        {
            light.intensity = Mathf.Lerp(light.intensity, targetValue, _time * Time.deltaTime);
            yield return null;
        }
    }
        
}
