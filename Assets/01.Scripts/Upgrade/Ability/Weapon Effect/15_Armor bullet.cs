using UnityEngine;
using _01.Scripts.PlayerControll.Status;

public class Armorbullet_15 : MonoBehaviour, IUpgradeEffect
{
    public string EffectName => "철갑탄";

    public void ApplyEffect(PlayerStat playerStat)
    {
        var weapon = GameManager.PlayerManager?.currentLoadout?.Weapon;

        if (weapon == null)
        {
            Debug.LogWarning("Weapon 데이터가 존재하지 않아 철갑탄 효과를 적용할 수 없습니다.");
            return;
        }

        float oldDamage = weapon.Damage;

        // 대미지 30% 증가
        weapon.Damage = oldDamage * 1.3f;

        Debug.Log($"철갑탄 효과 적용됨! Damage {oldDamage} → {weapon.Damage}");
    }
}
