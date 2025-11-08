using UnityEngine;
using _01.Scripts.PlayerControll.Status;

public class Speedloader_11 : MonoBehaviour,IUpgradeEffect
{
    public string EffectName => "스피드로더";

    public void ApplyEffect(PlayerStat playerStat)
    {
        // Weapon 참조
        var weapon = GameManager.PlayerManager?.currentLoadout?.Weapon;

        if (weapon == null)
        {
            Debug.LogWarning("Weapon 데이터가 존재하지 않아 스피드로더 효과를 적용할 수 없습니다.");
            return;
        }

        // 재장전 속도 증가 메서드 호출
        // 기존 대비 150% (1.5배)
        //float reloadMultiplier = 1.5f;
        //weapon.UpgradeReloadSpeed(reloadMultiplier);

        //Debug.Log($"스피드로더 효과 적용됨! 재장전 속도 {reloadMultiplier * 100f}%로 증가");
    }
}
