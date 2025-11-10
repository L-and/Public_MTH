using System;
using UnityEngine;

//BodyPart.OnCollisionEnter()에 직접 건드리지 않고, 이벤트로 연결    
public static class BodyPartDamageModifier
{

    public delegate void WeakHitDelegate(ref float damage); // 약점 피격 이벤트

    public static WeakHitDelegate OnWeakHit;

    // 약점 판정 및 이벤트 호출
    public static void ProcessHit(ref float rawDamage, HitZones zone)
    {
        if (zone == HitZones.Weak && OnWeakHit != null)
        {
            OnWeakHit(ref rawDamage);
        }
    }
}
