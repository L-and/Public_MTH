using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public class Lightweightshoes_8 : MonoBehaviour, IUpgradeEffect
{
    // UpgradeData.Name 과 정확히 동일해야 함
    public string EffectName => "경량형 신발"; 

    public void ApplyEffect(PlayerStatus playerStatus)
    {
        if (playerStatus == null)
        {
            Debug.LogWarning("PlayerStatus가 존재하지 않아 경량형 신발 효과를 적용할 수 없습니다.");
            return;
        }

        //  일반 이동속도 +20%
        var maxSpeedField = typeof(PlayerStatus)
            .GetField("maxSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (maxSpeedField != null)
        {
            float oldMaxSpeed = (float)maxSpeedField.GetValue(playerStatus);
            float newMaxSpeed = oldMaxSpeed * 1.2f;
            maxSpeedField.SetValue(playerStatus, newMaxSpeed);
            Debug.Log($"[경량형 신발] 이동속도 20% 증가 ({oldMaxSpeed:F1} → {newMaxSpeed:F1})");
        }
        else
        {
            Debug.LogWarning("PlayerStatus 내에 maxSpeed 변수를 찾을 수 없습니다.");
        }

        // 슬라이딩 속도 +50%
        var slidingSpeedField = typeof(PlayerStatus)
            .GetField("slidingMaxSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (slidingSpeedField != null)
        {
            float oldSlidingSpeed = (float)slidingSpeedField.GetValue(playerStatus);
            float newSlidingSpeed = oldSlidingSpeed * 1.5f;
            slidingSpeedField.SetValue(playerStatus, newSlidingSpeed);
            Debug.Log($"[경량형 신발] 슬라이딩 속도 50% 증가 ({oldSlidingSpeed:F1} → {newSlidingSpeed:F1})");
        }
        else
        {
            Debug.LogWarning("PlayerStatus 내에 slidingMaxSpeed 변수를 찾을 수 없습니다.");
        }
    }
}
