using UnityEngine;
using _01.Scripts.PlayerControll.Status;

public class Freezebullet_14 : MonoBehaviour, IUpgradeEffect
{
    public string EffectName => "냉동탄";

    public void ApplyEffect(PlayerStat playerStat)
    {
        if (GameManager.PlayerManager == null || GameManager.PlayerManager.PlayerStat == null)
        {
            Debug.LogWarning("PlayerStat을 찾을 수 없어 냉동탄 효과를 적용할 수 없습니다.");
            return;
        }

        var weapon = GameManager.PlayerManager.currentLoadout?.Weapon;
        var stat = GameManager.PlayerManager.PlayerStat;

        if (weapon == null)
        {
            Debug.LogWarning(" Weapon 데이터가 존재하지 않아 냉동탄 효과를 적용할 수 없습니다.");
            return;
        }

        float currentHP = stat.hp.Value;
        float oldDamage = weapon.Damage;

        // 체력이 20 이하일 때만 대미지 2배로 적용
        if (currentHP <= 20)
        {
            weapon.Damage = oldDamage * 2f;
            Debug.Log($"냉동탄 효과 발동! HP {currentHP} → 적에게 주는 대미지 2배 ({oldDamage} → {weapon.Damage})");
        }
        else
        {
            // 체력이 20 이상이면 원래 대미지로 복귀
            weapon.Damage = oldDamage;
            Debug.Log($" 냉동탄 비활성화 중 (현재 HP {currentHP})");
        }
    }
}
