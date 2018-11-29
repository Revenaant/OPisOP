using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputToText : MonoBehaviour
{
    private Text _textObject = null;

    [SerializeField] private bool _update = false;
    [SerializeField] private string _text;

    // Use this for initialization
    private void Start()
    {
        _textObject = GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_update) Output();
    }

    public void UpdateText(string text)
    {
        _text = text;
    }

    public void Output()
    {
        if (_textObject != null)
            _textObject.text = _text;
    }
}
