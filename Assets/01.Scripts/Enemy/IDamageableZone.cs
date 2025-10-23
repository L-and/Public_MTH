using UnityEngine;

public interface IDamageableZone 
{
    bool IsDead { get; }
    void ApplyHit(float rawDamage, Vector3 hitPoint, HitZones zone);
}
