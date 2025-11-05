using UnityEngine;

public interface IRangedAttacker
{
    bool IsRunning { get; }
    bool CanUse(Transform target, float distance);
    void Execute(Transform target);
}
