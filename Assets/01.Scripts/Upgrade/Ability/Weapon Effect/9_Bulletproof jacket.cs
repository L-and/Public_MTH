using UnityEngine;
using _01.Scripts.PlayerControll.Status;

public class Bulletproofjacket_9 : MonoBehaviour, IUpgradeEffect
{
    // UpgradeData.Name 과 정확히 동일해야 함
    public string EffectName => "방탄복"; 

    public void ApplyEffect(PlayerStatus playerStatus)
    {
        if (playerStatus == null)
        {
            Debug.LogWarning("⚠️ PlayerStatus가 존재하지 않아 방탄복 효과를 적용할 수 없습니다.");
            return;
        }

        // 체력 50% 증가
        float oldMax = playerStatus.hp.maxValue;
        playerStatus.hp.maxValue = Mathf.RoundToInt(oldMax * 1.5f);
        playerStatus.hp.Value = playerStatus.hp.maxValue; // 체력도 꽉 채우기

        Debug.Log($"방탄복 효과 적용됨! HP {oldMax} → {playerStatus.hp.maxValue}");
    }
}
