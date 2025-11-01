using UnityEngine;

public interface ISummonAttacker
{
    bool IsRunning { get; }
    bool CanUse(Transform target, float distance);
    void Execute(Transform target);
}