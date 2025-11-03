using _01.Scripts.PlayerControll;
using _01.Scripts.PlayerControll.Status;
using UnityEngine;

namespace _01.Scripts.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("플레이어 상태값 (Scriptable Object)")]
        [SerializeField] private PlayerStatSO PlayerStatSO;
        
        [SerializeField]
        public PlayerController PlayerController { get; private set; }

        [SerializeField] private PlayerStat playerStat;
        public PlayerStat PlayerStat => playerStat;

        private void Awake()
        {
            InitializePlayerStat();
            
            PlayerController = FindObjectOfType<PlayerController>();
            playerStat.pc = PlayerController;
        }

        /// <summary>
        /// 플레이어의 스탯정보를 초기화하는 메서드
        /// </summary>
        [ContextMenu("Player Stat 초기화")]
        public void InitializePlayerStat()
        {
            playerStat = new PlayerStat(PlayerStatSO.playerStat);
        }
    }
}
