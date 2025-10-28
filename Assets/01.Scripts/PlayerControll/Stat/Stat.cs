using System;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] public float maxValue;
    [SerializeField] private float value;

    public float Value
    {
        get => value;
        set => this.value = Mathf.Clamp(value, 0, maxValue);
    }

    public void Initialize(float maxValue = 100)
    {
        this.maxValue = maxValue;
        this.value = maxValue;
    }

}
