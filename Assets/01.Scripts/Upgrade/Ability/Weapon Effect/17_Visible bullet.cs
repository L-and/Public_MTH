using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public class Visiblebullet_17 : MonoBehaviour, IUpgradeEffect
{
    public string EffectName => "가시탄";

    public void ApplyEffect(PlayerStat playerStat)
    {
        // 이미 효과가 추가되어 있다면 중복 방지
        if (GameManager.PlayerManager.GetComponent<WeakSpotDamageBooster>() == null)
        {
            GameManager.PlayerManager.gameObject.AddComponent<WeakSpotDamageBooster>();
            Debug.Log("가시탄 효과 적용됨! 약점(머리) 명중 시 대미지 50% 증가");
        }
        else
        {
            Debug.Log("이미 가시탄 효과가 적용되어 있습니다.");
        }
    }
}
