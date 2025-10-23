using System;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] public int maxValue;
    [SerializeField] private int value;

    public int Value
    {
        get => value;
        set => this.value = Mathf.Clamp(value, 0, maxValue);
    }

    public void Initialize(int maxValue = 100)
    {
        this.maxValue = maxValue;
        this.value = maxValue;
    }

}
