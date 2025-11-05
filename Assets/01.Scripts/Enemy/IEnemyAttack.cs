using UnityEngine;

public enum AttackKind{Melee, Ranged, Special}

public interface IEnemyAttack 
{
    AttackKind Kind { get; }
    float MinRange { get; } // 최소 사거리
    float MaxRange { get; } // 최대 사거리
    bool RequireLOS { get; } // 시야 필요 여부
    
    bool IsAttacking { get; }
    bool IsOnCooldown { get; }

    bool CanAttack(Transform target); // 내부적 쿨타임/사거리/시야 판단
    void Attack(Transform target); //실제 발동
}
