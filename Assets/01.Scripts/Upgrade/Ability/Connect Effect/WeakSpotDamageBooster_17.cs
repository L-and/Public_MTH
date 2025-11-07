using UnityEngine;

// 가시탄 기능 처리(약점 명중 시 rawDamage × 1.5f)
public class WeakSpotDamageBooster : MonoBehaviour
{

    public static float WeakBonusMultiplier = 1.5f; // 50% 증가

    private void OnEnable()
    {
        BodyPartDamageModifier.OnWeakHit += ApplyWeakBonus;
    }

    private void OnDisable()
    {
        BodyPartDamageModifier.OnWeakHit -= ApplyWeakBonus;
    }

    private void ApplyWeakBonus(ref float damage)
    {
        damage *= WeakBonusMultiplier;
        Debug.Log($"약점 명중! Damage ×{WeakBonusMultiplier}");
    }
}
