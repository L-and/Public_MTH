using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public class Booster20_7 : MonoBehaviour, IUpgradeEffect
{
    // UpgradeData.Name 과 정확히 동일해야 함
    public string EffectName => "부스터 2.0";

    public void ApplyEffect(PlayerStat playerStat)
    {
        if (playerStat == null)
        {
            Debug.LogWarning(" PlayerStat가 존재하지 않아 부스터 2.0 효과를 적용할 수 없습니다.");
            return;
        }

        // 회피 시간 50% 증가 (dashDurationTime)
        var dashDurationField = typeof(PlayerStat)
            .GetField("dashDurationTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (dashDurationField != null)
        {
            float currentDashTime = (float)dashDurationField.GetValue(playerStat);
            float newDashTime = currentDashTime * 1.5f;
            dashDurationField.SetValue(playerStat, newDashTime);
            Debug.Log($" [부스터 2.0] 회피 시간 50% 증가 ({currentDashTime:F2}s → {newDashTime:F2}s)");
        }
        else
        {
            Debug.LogWarning(" PlayerStat 내에서 dashDurationTime 변수를 찾지 못했습니다.");
        }

        // 스테미나 최대치 +1
        float oldMaxStamina = playerStat.stamina.maxValue;
        playerStat.stamina.maxValue = oldMaxStamina + 1f;

        // 현재 스테미나가 최대치를 초과하지 않도록
        if (playerStat.stamina.Value > playerStat.stamina.maxValue)
            playerStat.stamina.Value = playerStat.stamina.maxValue;

        Debug.Log($" [부스터 2.0] 스테미나 최대치 {oldMaxStamina} → {playerStat.stamina.maxValue} ( +1 )");
    }
}
