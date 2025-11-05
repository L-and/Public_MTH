using _01.Scripts.PlayerControll.Status;
using UnityEngine;

public interface IUpgradeEffect 
{
    string EffectName { get; }   // ȿ���̸�
    void ApplyEffect(PlayerStat playerStat);   // ȿ�� ���� �޼���
}
