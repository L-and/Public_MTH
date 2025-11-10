using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public class Overheatmachine_20 : MonoBehaviour, IUpgradeEffect
{
    // UpgradeData.Name 과 정확히 동일해야 함
    public string EffectName => "과열 축적기"; 

    public void ApplyEffect(PlayerStat playerStatus)
    {
        if (playerStatus == null)
        {
            Debug.LogWarning("PlayerStatus가 존재하지 않아 과열 축척기 효과를 적용할 수 없습니다.");
            return;
        }

        // 기존 최대 과열 수치 저장
        float oldMax = playerStatus.overheat.maxValue;

        // 과열 게이지 최대치 3배로 증가
        playerStatus.overheat.maxValue = Mathf.RoundToInt(oldMax * 3f);

        // 현재 게이지가 새 최대치를 넘지 않도록 조정
        if (playerStatus.overheat.Value > playerStatus.overheat.maxValue)
            playerStatus.overheat.Value = playerStatus.overheat.maxValue;

        Debug.Log($" [과열 축척기] 과열 게이지 최대치 3배 증가 ({oldMax} → {playerStatus.overheat.maxValue})");
    }
}
