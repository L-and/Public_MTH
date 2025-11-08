using UnityEngine;
using _01.Scripts.PlayerControll.Status;

public class Extendedmagazine_13 : MonoBehaviour, IUpgradeEffect
{
    public string EffectName => "대형 탄창";

    public void ApplyEffect(PlayerStat playerStat)
    {
        var weapon = GameManager.PlayerManager?.currentLoadout?.Weapon;

        if (weapon == null)
        {
            Debug.LogWarning("Weapon 데이터가 존재하지 않아 대형 탄창 효과를 적용할 수 없습니다.");
            return;
        }

        // 기존 값 저장
        int oldMagSize = weapon.MagazineSize;

        // 탄창 용량 100% 증가 (2배)
        weapon.MagazineSize = Mathf.RoundToInt(oldMagSize * 2f);

        Debug.Log($" 대형 탄창 효과 적용됨! MagazineSize {oldMagSize} → {weapon.MagazineSize}");
    }
}
