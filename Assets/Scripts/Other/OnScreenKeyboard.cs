using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnScreenKeyboard : MonoBehaviour
{
    [SerializeField] private InputField output;
    [SerializeField] private int characterLimit = 20;

    private Dictionary<Keys, Button> buttons = new Dictionary<Keys, Button>();
    private bool caps = false;
    bool open = true;

    public System.Action OnEnterPressed;

    public int CharacterLimit
    {
        get { return characterLimit; }
        set { characterLimit = value; }
    }

    public string GetInputText()
    {
        return output == null ? "Input Field was null" : output.text;
    }

    private void Start()
    {
        Button[] b = GetComponentsInChildren<Button>();

        for (int i = 0; i < b.Length; i++)
        {
            // Numbers row should be the first
            if (i < 10) buttons.Add((Keys)Enum.Parse(typeof(Keys), 'N' + b[i].name), b[i]);
            else buttons.Add((Keys)Enum.Parse(typeof(Keys), b[i].name.ToUpper()), b[i]);
        }

        // Subscribe events
        buttons[Keys.SPACE].onClick.AddListener(Space);
        buttons[Keys.SHIFT].onClick.AddListener(ChangeShift);
        buttons[Keys.BACKSPACE].onClick.AddListener(Backspace);
        //buttons[Keys.ENTER].onClick.AddListener(Enter);

        for (int i = 0; i < 36; i++)
        {
            Button button = buttons[(Keys)i];
            button.onClick.AddListener(delegate { AddText(button); });
        }
    }

    /// <summary>
    /// Adds the typed character into the stream
    /// </summary>
    /// <param name="button"></param>
    private void AddText(Button button)
    {
        if(open && output.text.Length < CharacterLimit)
            output.text += button.GetComponentInChildren<Text>().text;
    }

    /// <summary>
    /// Removes the last character of the text
    /// </summary>
    private void Backspace()
    {
        if (!open) return;

        if (output.text.Length > 0)
            output.text = output.text.Remove(output.text.Length - 1);
    }

    /// <summary>
    /// Adds a space
    /// </summary>
    private void Space()
    {
        if(open && output.text.Length < characterLimit) output.text += " ";
    }

    private void Enter()
    {
        if (OnEnterPressed != null)
            OnEnterPressed.Invoke();
    }

    ///// <summary>
    ///// Adds a newLine character to the text
    ///// </summary>
    //private void Enter()
    //{
    //    output.text += System.Environment.NewLine;
    //}

    /// <summary>
    /// Visually updates the text for the keys in the keyboard
    /// </summary>
    private void ChangeShift()
    {
        if (caps == false)
        {
            for (int i = 0; i < 26; i++)
            {
                Button b = buttons[(Keys)i];
                Text text = b.GetComponentInChildren<Text>();
                text.text = text.text.ToUpper();
            }
            caps = true;
        }
        else
        {
            for (int i = 0; i < 26; i++)
            {
                Button b = buttons[(Keys)i];
                Text text = b.GetComponentInChildren<Text>();
                text.text = text.text.ToLower();
            }
            caps = false;
        }
    }

    /// <summary>
    /// Toggles if the input should be processed
    /// </summary>
    public void Toggle()
    {
        if (open) CloseKeyboard();
        else OpenKeyboard();
    }

    /// <summary>
    /// Allows input to be processed
    /// </summary>
    public void OpenKeyboard()
    {
        open = true;
    }

    /// <summary>
    /// Disables processing of input
    /// </summary>
    public void CloseKeyboard()
    {
        open = false;
    }

    private enum Keys {
        Q, W, E, R, T, Y, U, I, O, P,
        A, S, D, F, G, H, J, K, L,
        Z, X, C, V, B, N, M,
        N1, N2, N3, N4, N5, N6, N7, N8, N9, N0,
        SHIFT, BACKSPACE, ENTER, SPACE
    }
}
