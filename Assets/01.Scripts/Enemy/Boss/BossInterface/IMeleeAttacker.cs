using UnityEngine;

public interface IMeleeAttacker
{
    bool IsRunning { get; }
    bool CanUse(Transform target, float distance);     // 지금 상황에서 쓸 수 있는가
    void Execute(Transform target);                    // 코루틴 내부에서 windup/쿨타임 처리
}
