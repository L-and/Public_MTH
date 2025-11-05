using _01.Scripts.PlayerControll.Status;
using UnityEngine;

namespace _01.Scripts.PlayerControll
{
    [CreateAssetMenu(fileName = "New PlayerStat", menuName = "Player/Player Stat")]
    public class PlayerStatSO : ScriptableObject
    {
        [Header("플레이어 속성값")]
        [SerializeField] public PlayerStat playerStat;
    }
}