using UnityEngine;
using _01.Scripts.PlayerControll.Status;

public class Magnum_1 : MonoBehaviour, IUpgradeEffect
{
    public string EffectName => "매그넘";

    public void ApplyEffect(PlayerStat playerStat)
    {
        // 무기 데이터 접근
        var weapon = GameManager.PlayerManager?.currentLoadout?.Weapon;

        if (weapon == null)
        {
            Debug.LogWarning(" Weapon 데이터가 존재하지 않아 매그넘 효과를 적용할 수 없습니다.");
            return;
        }

        // 기존 값 저장
        float oldDamage = weapon.Damage;
        float oldRpm = weapon.Rpm;

        // 대미지 2배 증가
        weapon.Damage = oldDamage * 2f;

        // 공격 속도 25% 감소 (RPM은 분당 발사 속도)
        weapon.Rpm = Mathf.RoundToInt(oldRpm * 0.75f);

        Debug.Log($"매그넘 효과 적용됨! Damage {oldDamage} → {weapon.Damage}, RPM {oldRpm} → {weapon.Rpm}");
    }
}
