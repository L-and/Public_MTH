using _01.Scripts.PlayerControll;
using _01.Scripts.PlayerControll.Status;
using _01.Scripts.Weapons_ScriptableObjects.Loadout;
using UnityEngine;

namespace _01.Scripts.Manager
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("플레이어 스탯 (Scriptable Object)")]
        [SerializeField] private PlayerStatSO playerStatSo;
        
        private PlayerStat _playerStat;
        public PlayerStat PlayerStat => _playerStat;
        
        [SerializeField]
        public PlayerController PlayerController { get; private set; }
        
        
        [Header("선택할 수 있는 장비 목록(무기, 보조무기, 방출) (Scriptable Object)")]          
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
            // 기존 이벤트 구독 해제
            if (currentLoadout.Weapon)
                currentLoadout.Weapon.OnStatsChanged -= UpdateWeaponData;
                
            currentLoadout.Weapon = Instantiate(loadoutsSo.weapons[weaponId]);
            currentLoadout.SubWeapon = Instantiate(loadoutsSo.subWeapons[subWeaponId]);
            currentLoadout.Emission = Instantiate(loadoutsSo.emissions[emissionId]);
            
            // 이벤트 구독
            currentLoadout.Weapon.OnStatsChanged += UpdateWeaponData;
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
            PlayerController.Initialize(_playerStat, currentLoadout);
        }
        
        /// <summary>
        /// 플레이어의 스탯정보를 초기화하는 메서드
        /// </summary>
        [ContextMenu("Player Stat 초기화")]
        public void InitializePlayerStat()
        {
            _playerStat = new PlayerStat(playerStatSo.playerStat);
        }
        
        #region 속성값 변경 적용 이벤트메서드

        /// <summary>
        /// 주무기(총기) 속성변경 적용
        /// </summary>
        private void UpdateWeaponData()
        {
            if (!PlayerController)
            {
                Debug.LogWarning("PlayerController가 연결되지 않았습니다.");
            }
            
            PlayerController.UpdateMainWeaponData(currentLoadout.Weapon);
        }
        
        #endregion
    }
}
