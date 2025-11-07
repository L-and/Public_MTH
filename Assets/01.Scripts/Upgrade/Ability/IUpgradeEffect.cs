using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public interface IUpgradeEffect 
{
    string EffectName { get; }   // 효과 이름
    void ApplyEffect(PlayerStat playerStat);   // 효과 참조
}
