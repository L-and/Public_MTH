using UnityEngine;
using _01.Scripts.PlayerControll.Status;

public class Bulletproofjacket_9 : MonoBehaviour, IUpgradeEffect
{
    // UpgradeData.Name 과 정확히 동일해야 함
    public string EffectName => "방탄복"; 

    public void ApplyEffect(PlayerStat playerStat)
    {
        if (playerStat == null)
        {
            Debug.LogWarning("⚠️ PlayerStat가 존재하지 않아 방탄복 효과를 적용할 수 없습니다.");
            return;
        }

        // 체력 50% 증가
        float oldMax = playerStat.hp.maxValue;
        playerStat.hp.maxValue = Mathf.RoundToInt(oldMax * 1.5f);
        playerStat.hp.Value = playerStat.hp.maxValue; // 체력도 꽉 채우기

        Debug.Log($"방탄복 효과 적용됨! HP {oldMax} → {playerStat.hp.maxValue}");
    }
}
