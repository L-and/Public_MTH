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

    /// <summary>
    /// 해당 스탯을 cost만큼 차감할 수 있는지 검사.
    /// 차감이 가능: value를 cost만큼 차감하고 true를 반환,
    /// 차감이 불가능: false 리턴
    /// </summary>
    /// <param name="cost">차감하려는 양</param>
    /// <returns>차감 성공 여부</returns>
    public bool TryDecrease(float cost)
    {
        if (value - cost >= 0)
        {
            value -= cost;
            return true;
        }
        else
        {
            return false;
        }
    }
}
