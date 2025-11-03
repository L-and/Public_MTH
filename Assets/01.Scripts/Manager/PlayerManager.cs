using _01.Scripts.Emission;
using _01.Scripts.PlayerControll;
using _01.Scripts.PlayerControll.Status;
using _01.Scripts.ScriptableObjects.Loadout;
using _01.Scripts.SubWeapon;
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
        [SerializeField] private PlayerLoadoutsSO loadoutsSo; // 무기, 보조무기, 방출 목록들 SO

        [Header("선택된 무기/보조무기/방출")]
        [SerializeField] public PlayerLoadout currentLoadout;
        
        private void Awake()
        {
            currentLoadout = new PlayerLoadout();
            // TODO 테스트용 코드라서 이후에 제거 필요
            // 게임시작 후 스탯, 장비적용 테스트용 코드
            SetLoadout(0, 0, 0);
            GameStart();
        }

        /// <summary>
        /// 장비선택 UI에서 선택된 장비SO를 불러와서 LoadOut에 적용시키는 메서드
        /// </summary>
        /// <param name="weaponId"></param>
        /// <param name="subWeaponId"></param>
        /// <param name="emissionId"></param>
        public void SetLoadout(int weaponId, int subWeaponId, int emissionId)
        {
            
            // currentLoadout.Weapon = loadoutsSo.weapons[weaponId];
            currentLoadout.SubWeapon = loadoutsSo.subWeapons[subWeaponId];
            currentLoadout.Emission = loadoutsSo.emissions[emissionId];
        }
        
        /// <summary>
        /// 플레이어의 스탯, 선택된 장비를 적용시키는 중요 메서드
        /// (플레이어가 생성된 후 호출)
        /// </summary>
        public void GameStart()
        {
            PlayerController = FindObjectOfType<PlayerController>();

            // PlayerStat 정보를 SO에서 가져오기
            InitializePlayerStat();
            
            // PlayerController에 스탯 정보 전달
            PlayerController.Initialize(playerStat, currentLoadout);
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
