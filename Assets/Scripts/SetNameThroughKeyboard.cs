using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNameThroughKeyboard : MonoBehaviour
{
    [SerializeField]private OnScreenKeyboard _keyboard = null;

    private void Start()
    {
        if (_keyboard == null) _keyboard = GetComponent<OnScreenKeyboard>();
        if (_keyboard == null) _keyboard = FindObjectOfType<OnScreenKeyboard>();
    }

    public void SetScore()
    {
        if (_keyboard == null) return;
        GameManager.Instance.Score = new Score(_keyboard.GetInputText());
    }
}
