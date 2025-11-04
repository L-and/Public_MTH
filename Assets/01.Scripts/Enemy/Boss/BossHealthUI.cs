using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private BossPhase bossPhase; // 보스 체력 스크립트
    [SerializeField] private Slider healthSlider; // Canvas에 있는 슬라이더

    private void Start()
    {
        if (bossPhase != null)
        {
            // 체력 변화가 있을 때마다 Slider 갱신
            bossPhase.onDamage.AddListener(UpdateSlider);
            // 처음 체력 상태 반영
            UpdateSlider(bossPhase.CurrentHp, bossPhase.CurrentMax);
        }
    }

    private void OnDestroy()
    {
        if (bossPhase != null)
            bossPhase.onDamage.RemoveListener(UpdateSlider);
    }

    private void UpdateSlider(float current, float max)
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }
}
