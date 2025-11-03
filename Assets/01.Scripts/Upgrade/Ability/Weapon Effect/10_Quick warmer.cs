using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public class Quickwarmer_10 : MonoBehaviour, IUpgradeEffect
{
    // UpgradeData.Name 과 정확히 동일해야 함
    public string EffectName => "급속 온열기"; 

    public void ApplyEffect(PlayerStatus playerStatus)
    {
        if (playerStatus == null)
        {
            Debug.LogWarning(" PlayerStatus가 존재하지 않아 급속 온열기 효과를 적용할 수 없습니다.");
            return;
        }

        // 기존 최대 과열치 저장
        float oldMax = playerStatus.overheat.maxValue;

        // 게이지 증가 속도를 높이기 위해, 최대치를 30% 감소시켜 더 빠르게 차게 함
        playerStatus.overheat.maxValue = Mathf.RoundToInt(oldMax * 0.7f);

        // 현재 게이지가 새 최대치를 넘지 않도록 조정
        if (playerStatus.overheat.Value > playerStatus.overheat.maxValue)
            playerStatus.overheat.Value = playerStatus.overheat.maxValue;

        Debug.Log($"[급속 온열기] 과열 게이지 증가속도 30% 상승! (MaxValue {oldMax} → {playerStatus.overheat.maxValue})");
    }
}
