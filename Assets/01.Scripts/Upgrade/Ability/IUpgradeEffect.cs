using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public interface IUpgradeEffect 
{
    string EffectName { get; }   // 효과이름
    void ApplyEffect(PlayerStatus playerStatus);   // 효과 적용 메서드
}
