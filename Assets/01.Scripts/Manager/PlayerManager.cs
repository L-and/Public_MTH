using _01.Scripts.PlayerControll;
using _01.Scripts.PlayerControll.Status;
using _01.Scripts.ScriptableObjects.Loadout;
using UnityEngine;

namespace _01.Scripts.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("플레이어 상태값 (Scriptable Object)")]
        [SerializeField] private PlayerStatSO PlayerStatSO;
        
        [SerializeField]
        public PlayerController PlayerController { get; private set; }

        [Header("플레이어 스탯 (Scriptable Object)")]          
        [SerializeField] private PlayerStat playerStat;
        public PlayerStat PlayerStat => playerStat;
        
        [Header("플레이어 장비 (Scriptable Object)")]          
        [SerializeField] private PlayerLoadoutSO playerLoadout; 

        private void Awake()
        {
            PlayerController = FindObjectOfType<PlayerController>();

            // PlayerStat 정보를 SO에서 가져오기
            InitializePlayerStat();
            
            // PlayerController에 스탯 정보 전달
            PlayerController.Initialize(playerStat, playerLoadout);
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
