using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogForUnityEvent : MonoBehaviour
{
    public string text;

    public void Print()
    {
        Debug.Log(text);
    }

    public void PrintText(string textParameter)
    {
        Debug.Log(textParameter);
    }
}
