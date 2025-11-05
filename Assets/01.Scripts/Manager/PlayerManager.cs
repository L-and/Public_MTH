using System;
using _01.Scripts.PlayerControll;
using _01.Scripts.PlayerControll.Status;
using _01.Scripts.Weapons_ScriptableObjects.Loadout;
using UnityEngine;

namespace _01.Scripts.Manager
{
    /// <summary>
    ///
    /// 데이터 관리방식:
    /// 플레이어 속성(체력, 스테미너...), 장비목록(주무기, 보조무기, 방출) 데이터를 Scriptable Object로 관리,
    /// 게임시작(플레이어 스폰) 시 SO의 데이터를 토대로 플레이어 속성, 장비목록을 필드로 저장하게 됨
    ///
    /// 
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        [Header("플레이어 프리팹")]
        [SerializeField] private GameObject _playerPrefab;
        
        #region Scriptable Object 필드들  (플레이어 스탯, 장비목록들을 SO로 만들어서 게임플레이 시 적용하는 방식)
        
        [Header("플레이어 스탯 (Scriptable Object)")]
        [SerializeField] private PlayerStatSO playerStatSo; // 플레이어 속성값
        
        [Header("선택할 수 있는 장비 목록(무기, 보조무기, 방출) (Scriptable Object)")]          
        [SerializeField] private PlayerLoadoutsSO loadoutsSo; // 무기, 보조무기, 방출 목록들 SO
        
        #endregion

        # region 게임플레이 중 사용되는 게임 데이터 필드들
        
        [Header("게임플레이 중 사용되는 데이터")]
        [SerializeField] public PlayerLoadout currentLoadout;
        
        [SerializeField] private PlayerStat _playerStat;
        public PlayerStat PlayerStat => _playerStat;
   
        public PlayerController PlayerController { get; private set; }
        
        # endregion

        private void Awake()
        {
            // TODO 나중에 씬 연결을 하게되면 제거해야 함
            // 씬 플레이 시 플레이어의 프리팹을 생성하여 테스트를 편하게 하기위해 작성한 코드임
            PlayerSpawnTest();
        }

        /// <summary>
        /// 선택된 장비ID를 이용하여 플레이어 게임오브젝트를 생성하는 메서드
        /// 사용방법: 장비선택 UI 이후 레벨1 시작에서 사용될것으로 예상
        /// </summary>
        [ContextMenu("플레이어 생성")]
        private void PlayerSpawnTest()
        {
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
            currentLoadout = new PlayerLoadout();
            
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
            var spawnPos = transform.position;
            var playerGo = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);

            // 플레이어 게임오브젝트 생성
            if (playerGo.TryGetComponent<PlayerController>(out var pc))
            {
                PlayerController = pc;
                
                // 플레이어 속성값을 SO에서 가져와서 게임플레이중 데이터로 사용
                InitializePlayerStat();
            
                // PlayerController에 스탯 정보 전달
                PlayerController.Initialize(_playerStat, currentLoadout);
            }
        }
        
        /// <summary>
        /// 플레이어의 스탯정보를 초기화하는 메서드 (게임 시작시 사용)
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
