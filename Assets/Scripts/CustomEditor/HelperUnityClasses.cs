using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCoroutines
{
    public static IEnumerator WaitOneFrame(Action action)
    {
        yield return null;
        action();
    }

    public static IEnumerator WaitXFrames(int frames, Action action)
    {
        for (int i = frames; i > 0; i--)
            yield return null;
        action();
    }

    public static IEnumerator WaitForEndOfFrame(Action action)
    {
        yield return new WaitForEndOfFrame();
        action();
    }

    public static IEnumerator Wait(float seconds = 1)
    {
        yield return new WaitForSeconds(seconds);
    }

    public static IEnumerator Wait(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }

    public static IEnumerator Wait<T>(float seconds, Action<T> action, T parameter)
        where T : class
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke(parameter);
    }

    public static IEnumerator DoWhile(Func<bool> pred, Action doWhile, Action doAfter = null, bool updateEveryFrame = true)
    {
        while (pred != null && pred())
        {
            if (updateEveryFrame) yield return null;
            if (doWhile != null) doWhile.Invoke();
        }

        if (doAfter != null) doAfter.Invoke();
    }

    public static IEnumerator DoUntil(Func<bool> pred, Action doWhile, Action doAfter = null, bool updateEveryFrame = true)
    {
        while (pred != null && !pred())
        {
            if (updateEveryFrame) yield return null;
            if (doWhile != null) doWhile.Invoke();
        }

        if (doAfter != null) doAfter.Invoke();
    }

    public static IEnumerator DoWhen(Func<bool> pred, Action action = null, bool updateEveryFrame = true)
    {
        if (pred == null)
        {
            if (action != null) action();
        }
        else
        {
            //Debug.Log("I am here.");
            while (!pred())
            {
                //Debug.Log("I am looping");
                if (updateEveryFrame) yield return null;
            }

            if (action != null) action.Invoke();
        }
    }
}

public class ComponentStatus
{
    public static void SetComponentEnabled(Component component, bool value = true)
    {
        if (component == null) return;
        if (component is Renderer)
        {
            (component as Renderer).enabled = value;
        }
        else if (component is Collider)
        {
            (component as Collider).enabled = value;
        }
        else if (component is Animation)
        {
            (component as Animation).enabled = value;
        }
        else if (component is Animator)
        {
            (component as Animator).enabled = value;
        }
        else if (component is AudioSource)
        {
            (component as AudioSource).enabled = value;
        }
        else if (component is Behaviour)
        {
            (component as Behaviour).enabled = value;
        }
        else
        {
            Debug.Log("Don't know how to enable " + component.GetType().Name);
        }
    }

    public static bool IsEnabled(Component component)
    {
        bool value = false;
        if (component == null)
        {
            Debug.Log("Component is null.");
            return value;
        }
        if (component is Renderer)
        {
            value = (component as Renderer).enabled;
        }
        else if (component is Collider)
        {
            value = (component as Collider).enabled;
        }
        else if (component is Animation)
        {
            value = (component as Animation).enabled;
        }
        else if (component is Animator)
        {
            value = (component as Animator).enabled;
        }
        else if (component is AudioSource)
        {
            value = (component as AudioSource).enabled;
        }
        else if (component is Behaviour)
        {
            value = (component as Behaviour).enabled;
        }
        else
        {
            Debug.Log("Don't know if enabled " + component.GetType().Name);
        }

        return value;
    }
}

