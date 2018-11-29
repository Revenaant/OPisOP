using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DayNightCycle))]
public sealed class DayNightAutoTuning : MonoBehaviour
{
    #region Y I DO DIS
#pragma warning disable 0649
    [System.Serializable]
    private class Element
    {
        public bool edit;

        public Element(bool edit = true)
        {
            this.edit = edit;
        }
    }

    [System.Serializable]
    private class MinMaxCurveElement : Element
    {
        public AnimationCurve curve;
        public float min;
        public float max;

        public MinMaxCurveElement(float min = 0, float max = 1, AnimationCurve curve = default(AnimationCurve), bool edit = true)
        : base(edit)
        {
            this.min = min;
            this.max = max;
            this.curve = curve;
        }
    }

    [System.Serializable]
    private class ScaleCurveElement : Element
    {
        public AnimationCurve curve;
        public float scale;

        public ScaleCurveElement(float scale = 1, AnimationCurve curve = default(AnimationCurve), bool edit = true)
            : base(edit)
        {
            this.scale = scale;
            this.curve = curve;
        }
    }

    [System.Serializable]
    private class GradientElement : Element
    {
        public Gradient colorGradient;

        public GradientElement(Gradient gradient = default(Gradient), bool edit = true) : base(edit)
        {
            colorGradient = gradient;
        }
    }

    [System.Serializable]
    private class Group
    {
        public bool editGroup;
    }

    [System.Serializable]
    private class MinMaxCurveGroup : Group
    {
        public MinMaxCurveElement intensity;
    }

    [System.Serializable]
    private class GradientGroup : Group
    {
        public GradientElement color;
    }

    [System.Serializable]
    private class Ambient : Group
    {
        public MinMaxCurveElement intensity;
        public bool colorEqualsLightSource = true;
    }

    [System.Serializable]
    private class GradientMinMaxCurveGroup : Group
    {
        public GradientElement color;
        public MinMaxCurveElement intensity;
    }

    [System.Serializable]
    private class FogGroup : Group
    {
        public GradientElement color;
        public ScaleCurveElement density;
    }

    [System.Serializable]
    private class SkyboxGroup : Group
    {
        public GradientElement skyTintColor;
        public MinMaxCurveElement sunSize;
        public MinMaxCurveElement sunSizeConvergence;
        public MinMaxCurveElement atmosphereThickness;
        public MinMaxCurveElement exposure;
    }
#pragma warning restore 0649
    #endregion

    [SerializeField, HideInInspector]
    private DayNightCycle _dayNightCycle = null;
    [SerializeField, HideInInspector]
    private Material _skyboxMaterial = null;

    #region Old
    //[Header("Render/Lighting Settings")]
    //[SerializeField] private bool _editAmbience = true;
    //[SerializeField] private AnimationCurve _ambientIntensityCurve;
    //[SerializeField] private float _ambientMinIntensity = -0.2f;
    //[SerializeField] private float _ambientMaxIntensity = 1.2f;

    //[Header("Fog")]
    //[SerializeField] private bool _editFog = true;
    //[SerializeField] private Gradient _dayNightFogColor;
    //[SerializeField] private AnimationCurve _fogDensity;
    //[SerializeField] private float _fogScale = 1f;

    //[Header("Skybox")]
    //[SerializeField] private bool _editSkybox = true;
    //[SerializeField] private Gradient _dayNightSkyTintColor;
    //[SerializeField] private AnimationCurve _atmosphereThicknessCurve;
    //[SerializeField] private float _dayAtmosphereThickness = 0.4f;
    //[SerializeField] private float _nightAtmosphereThickness = 0.87f;

    //[Header("Sun")]
    //[SerializeField] private bool _editSun = true;
    //[SerializeField] private Gradient _sunColor;
    //[SerializeField] private AnimationCurve _sunIntensityCurve;
    //[SerializeField] private float _sunMinIntensity = 0;
    //[SerializeField] private float _sunMaxIntensity = 3;

    //[Header("Moon")]
    //[SerializeField] private bool _editMoon = true;
    //[SerializeField] private Gradient _moonColor;
    //[SerializeField] private AnimationCurve _moonIntensityCurve;
    //[SerializeField] private float _moonMinIntensity = 0;
    //[SerializeField] private float _moonMaxIntensity = 3;

    //private void Fog()
    //{
    //    if (!_editFog) return;

    //    float factor = _dayNightCycle.TimeNormalized();

    //    RenderSettings.fogDensity = _fogScale * _fogDensity.Evaluate(factor);
    //    RenderSettings.fogColor = _dayNightFogColor.Evaluate(factor);
    //}

    //private void Ambience()
    //{
    //    if (!_editAmbience) return;

    //    float factor = _dayNightCycle.TimeNormalized();

    //    RenderSettings.ambientIntensity = _ambientMinIntensity +
    //                                      (_ambientMaxIntensity - _ambientMinIntensity) * _ambientIntensityCurve.Evaluate(factor);
    //    RenderSettings.ambientLight =
    //        _dayNightCycle.IsDay() ? _dayNightCycle.SunLight.color : _dayNightCycle.MoonLight.color;
    //}

    //private void Skybox()
    //{
    //    if (!_editSkybox) return;

    //    float factor = _dayNightCycle.TimeDayNormalized();

    //    sun.intensity = _sunMinIntensity +
    //                    (_sunMaxIntensity - _sunMinIntensity) * _sunIntensityCurve.Evaluate(factor);
    //    sun.color = _sunColor.Evaluate(factor);
    //}

    //private void Sun()
    //{
    //    if (!_editSun) return;

    //    float factor = _dayNightCycle.TimeDayNormalized();
    //    Light sun = _dayNightCycle.SunLight;

    //    sun.intensity = _sunMinIntensity +
    //                    (_sunMaxIntensity - _sunMinIntensity) * _sunIntensityCurve.Evaluate(factor);
    //    sun.color = _sunColor.Evaluate(factor);
    //}

    //private void Moon()
    //{
    //    if (!_editMoon) return;

    //    float factor = _dayNightCycle.TimeNightNormalized();
    //    Light moon = _dayNightCycle.MoonLight;

    //    moon.intensity = _moonMinIntensity +
    //                     (_moonMaxIntensity - _moonMinIntensity) * _moonIntensityCurve.Evaluate(factor);
    //    moon.color = _moonColor.Evaluate(factor);
    //}
    #endregion

    [Header("You can enable or disable entire groups or certain elements.")]

    [SerializeField]
    private Ambient _ambience = new Ambient()
    {
        intensity = new MinMaxCurveElement(0, 1.2f)
    };

    [SerializeField] private FogGroup _fog = new FogGroup();

    [SerializeField]
    private SkyboxGroup _skybox = new SkyboxGroup()
    {
        atmosphereThickness = new MinMaxCurveElement(0.47f, 0.87f)
    };

    [SerializeField]
    private GradientMinMaxCurveGroup _lightSource = new GradientMinMaxCurveGroup()
    {
        intensity = new MinMaxCurveElement(max: 3)
    };

    //[SerializeField]
    //private GradientMinMaxCurveGroup _moon = new GradientMinMaxCurveGroup()
    //{
    //    intensity = new MinMaxCurveElement(max: 3)
    //};

    // Use this for initialization
    private void Start()
    {
        _dayNightCycle = GetComponent<DayNightCycle>();

        //RenderSettings.fog = _editFog;
        RenderSettings.fog = _fog.editGroup;
        _skyboxMaterial = RenderSettings.skybox;

        Debug.Assert(_skyboxMaterial != null, "Assign a Procedural Skybox in the Lighting settings.");
    }

    public void OnValidate()
    {
        if (_dayNightCycle == null)
            _dayNightCycle = GetComponent<DayNightCycle>();
        if (_skyboxMaterial == null)
            _skyboxMaterial = RenderSettings.skybox;

        //Ambience
        _ambience.intensity.min = Mathf.Clamp(_ambience.intensity.min, 0, 8);
        _ambience.intensity.max = Mathf.Clamp(_ambience.intensity.max, 0, 8);

        ////Sun
        //_sun.intensity.min = Mathf.Clamp(_sun.intensity.min, 0, 8);
        //_sun.intensity.max = Mathf.Clamp(_sun.intensity.max, 0, 8);

        ////Moon
        //_moon.intensity.min = Mathf.Clamp(_moon.intensity.min, 0, 8);
        //_moon.intensity.max = Mathf.Clamp(_moon.intensity.max, 0, 8);

        //Skybox
        _skybox.exposure.min = Mathf.Clamp(_skybox.exposure.min, 0, 8);
        _skybox.exposure.max = Mathf.Clamp(_skybox.exposure.max, 0, 8);

        _skybox.atmosphereThickness.min = Mathf.Clamp(_skybox.atmosphereThickness.min, 0, 5);
        _skybox.atmosphereThickness.max = Mathf.Clamp(_skybox.atmosphereThickness.max, 0, 5);

        _skybox.sunSize.min = Mathf.Clamp01(_skybox.sunSize.min);
        _skybox.sunSize.max = Mathf.Clamp01(_skybox.sunSize.max);

        _skybox.atmosphereThickness.min = Mathf.Clamp(_skybox.atmosphereThickness.min, 1, 10);
        _skybox.atmosphereThickness.max = Mathf.Clamp(_skybox.atmosphereThickness.max, 1, 10);

        if (!Application.isPlaying)
            UpdateTuning();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateTuning();
    }

    private void UpdateTuning()
    {
        LightSource();
        //Moon();
        Skybox();
        Ambience();
        Fog();
    }

    private void Fog()
    {
        if (!_fog.editGroup) return;

        float factor = _dayNightCycle.TimeNormalized();

        if (_fog.density.edit)
            RenderSettings.fogDensity = _fog.density.scale * _fog.density.curve.Evaluate(factor);
        if (_fog.color.edit)
            RenderSettings.fogColor = _fog.color.colorGradient.Evaluate(factor);
    }

    private void Ambience()
    {
        if (!_ambience.editGroup) return;

        float factor = _dayNightCycle.TimeNormalized();

        if (_ambience.intensity.edit)
            RenderSettings.ambientIntensity = _ambience.intensity.min +
                                              (_ambience.intensity.max - _ambience.intensity.min) * _ambience.intensity.curve.Evaluate(factor);
        if (_ambience.colorEqualsLightSource)
            RenderSettings.ambientLight =
                /*_dayNightCycle.IsDay() ? */_dayNightCycle.LightSource.color/* : _dayNightCycle.MoonLight.color*/;
    }

    private void Skybox()
    {
        if (!_skybox.editGroup) return;

        float factor = _dayNightCycle.TimeNormalized();

        if (_skybox.skyTintColor.edit)
        {
            Color color = _skybox.skyTintColor.colorGradient.Evaluate(factor);
            //print(color);
            _skyboxMaterial.SetColor("_SkyTint", color);
        }

        if (_skybox.sunSize.edit)
        {
            float size = _skybox.sunSize.min +
                         (_skybox.sunSize.max - _skybox.sunSize.min) * _skybox.sunSize.curve.Evaluate(factor);

            _skyboxMaterial.SetFloat("_SunSize", size);
        }

        if (_skybox.sunSizeConvergence.edit)
        {
            float convergence = _skybox.sunSizeConvergence.min +
                                (_skybox.sunSizeConvergence.max - _skybox.sunSizeConvergence.min) *
                                _skybox.sunSizeConvergence.curve.Evaluate(factor);

            _skyboxMaterial.SetFloat("_SunSizeConvergence", convergence);
        }

        if (_skybox.atmosphereThickness.edit)
        {
            float intensity = _skybox.atmosphereThickness.min +
                              (_skybox.atmosphereThickness.max - _skybox.atmosphereThickness.min) *
                              _skybox.atmosphereThickness.curve.Evaluate(factor);

            _skyboxMaterial.SetFloat("_AtmosphereThickness", intensity);
        }

        if (_skybox.exposure.edit)
        {
            float intensity = _skybox.exposure.min +
                              (_skybox.exposure.max - _skybox.exposure.min) * _skybox.exposure.curve.Evaluate(factor);

            _skyboxMaterial.SetFloat("_Exposure", intensity);
        }
    }

    private void LightSource()
    {
        if (!_lightSource.editGroup) return;

        float factor = _dayNightCycle.TimeNormalized();
        Light lightSource = _dayNightCycle.LightSource;

        if (_lightSource.intensity.edit)
            lightSource.intensity = _lightSource.intensity.min +
                            (_lightSource.intensity.max - _lightSource.intensity.min) * _lightSource.intensity.curve.Evaluate(factor);
        if (_lightSource.color.edit)
            lightSource.color = _lightSource.color.colorGradient.Evaluate(factor);
    }

    //private void Moon()
    //{
    //    if (!_moon.editGroup) return;

    //    float factor = _dayNightCycle.TimeDayNormalized();
    //    Light moon = _dayNightCycle.MoonLight;

    //    if (_moon.intensity.edit)
    //        moon.intensity = _moon.intensity.min +
    //                        (_moon.intensity.max - _moon.intensity.min) * _moon.intensity.curve.Evaluate(factor);
    //    if (_moon.color.edit)
    //        moon.color = _moon.color.colorGradient.Evaluate(factor);
    //}
}
