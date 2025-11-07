using UnityEngine;
using _01.Scripts.PlayerControll.Status;

public class ReactionControllgloves_12 : MonoBehaviour, IUpgradeEffect
{
    public string EffectName => "반동 제어 장갑";

    public void ApplyEffect(PlayerStat playerStat)
    {
        var weapon = GameManager.PlayerManager?.currentLoadout?.Weapon;

        if (weapon == null)
        {
            Debug.LogWarning("Weapon 데이터가 존재하지 않아 반동 제어 장갑 효과를 적용할 수 없습니다.");
            return;
        }

        // 기존 값 저장
        float oldRpm = weapon.Rpm;
        float oldRecoil = weapon.VerticalRecoil;

        // 발사 속도 50% 증가 (RPM = 높을수록 빠름)
        weapon.Rpm = Mathf.RoundToInt(oldRpm * 1.5f);

        // 반동 50% 감소
        weapon.VerticalRecoil = oldRecoil * 0.5f;

        Debug.Log($"반동 제어 장갑 효과 적용됨! RPM {oldRpm} → {weapon.Rpm}, Recoil {oldRecoil} → {weapon.VerticalRecoil}");
    }
}
