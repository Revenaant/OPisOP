using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeDissolve : MonoBehaviour {

    [SerializeField] private float _speed = 0.5f;

    private Material mat;
    private float dissolveProgress;

    // Use this for initialization
    void Awake()
    {
        mat = GetComponent<Renderer>().material;
    }

    public bool Materialize()
    {
        dissolveProgress = 1;
        mat.SetFloat("_DissolveScale", dissolveProgress);

        StartCoroutine(MyCoroutines.DoUntil(() => { return dissolveProgress == 0; }, () =>
        {
            dissolveProgress = Mathf.Clamp01(Mathf.Lerp(dissolveProgress, 0, _speed * Time.deltaTime));
            if (dissolveProgress <= 0 + 0.3f) dissolveProgress = 0;

            mat.SetFloat("_DissolveScale", dissolveProgress);

        }, () =>
        {
            GetComponent<Renderer>().material.shader = Shader.Find("Standard");
            //print("FIUCK");
        }));

        return true;
    }

    public bool Dematerialize()
    {
        if (mat.shader.name != "Custom/DissolveShader")
            mat.shader = Shader.Find("Custom/DissolveShader");

        dissolveProgress = 0;
        mat.SetFloat("_DissolveScale", dissolveProgress);


        StartCoroutine(MyCoroutines.DoUntil(() => { return dissolveProgress == 1; }, () =>
        {
            dissolveProgress = Mathf.Clamp01(Mathf.Lerp(dissolveProgress, 1, _speed * Time.deltaTime));
            if (dissolveProgress >= 1 - 0.3f) dissolveProgress = 0;

            mat.SetFloat("_DissolveScale", dissolveProgress);
        }));

        return true;
    }

    private void OnEnable()
    {
        Materialize();
    }
}
