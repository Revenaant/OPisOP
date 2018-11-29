using System;
using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
    public float Min, Max;

    public MinMaxAttribute(float min, float max)
    {
        Min = min;
        Max = max;
    }
}

[Serializable]
public struct MinMaxPair
{
    public enum Position { Invalid, Under, InRange, Over }

    public float Min, Max;

    public MinMaxPair(float min, float max)
    {
        Min = min;
        Max = max;
    }

    public float Clamp(float value)
    {
        return Mathf.Clamp(value, Min, Max);
    }

    public float RandomValue
    {
        get { return UnityEngine.Random.Range(Min, Max); }
    }

    public Position Evaluate(float value)
    {
        if (value >= Min && value <= Max) return Position.InRange;
        if (value > Max) return Position.Over;
        if (value < Min) return Position.Under;

        return Position.Invalid;
    }
}