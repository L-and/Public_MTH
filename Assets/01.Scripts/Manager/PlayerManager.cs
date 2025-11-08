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
        
        /// <summary>
        /// 플레이어 게임오브젝트
        /// 부가설명: 레벨1 시작에서 생성된 플레이어GO의 사용이 필요할 듯 하여 캐싱해둠
        /// </summary>
        private GameObject _playerInstance; 
        
        /// <summary>
        /// 인게임 시작 시 플레이어를 생성
        /// 
        /// 레벨1 시작(처음생성) 선택된 장비적용, 플레이어 생성 후 PlayerController 참조
        /// spawnPosition으로 플레이어 위치변경 
        /// </summary>
        /// <param name="spawnPosition">스폰할 위치</param>
        public void PlayerSpawn(Vector3 spawnPosition, GameObject playerInstance = null)
        {
            Debug.Log($"스폰위치: {spawnPosition}");
            // 플레이어가 처음 스폰되는것이라면
            // 프리팹 생성, 장비적용, PlayerController캐싱을 진행
            if (!_playerInstance)
            {
                Debug.Log("플레이어 처음생성");
                // 플레이어 스탯, 장비 초기설정
                InitializeLoadout();
                InitializePlayerStat();

                // 플레이어 프리팹 로드
                // TODO 테스트를 위한 코드임으로 수정이 필요할 수 있음
                // (GameManager.ResourceEx.playerPrefab이 준비되지 않았으면 테스트용 프리팹으로 생성)
                var playerPrefab = GameManager.ResourceEx.playerPrefab ? GameManager.ResourceEx.playerPrefab : playerInstance;
                
                Debug.Log(playerPrefab);
                if (!playerPrefab)
                {
                    var variableInfo = GameManager.ResourceEx.playerPrefab
                        ? "GameManager.ResourceEx.playerPrefab"
                        : "GameManager.PlayerManager.playerPrefab";
                    
                    Debug.LogWarning("[## 중요 ##] 지정된 플레이어 프리팹이 없습니다!!\n" +
                                     $"사용한 프리팹 변수경로: {variableInfo}");
                    
                    return;
                }

                // 플레이어 생성
                _playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                // stroyOnLoad(_playerInstance); // 게임진행중 타이틀이동시 플레이어가 안사라지는 문제가 있어서 주석처리함
                
                // 필요한 컴포넌트 캐싱
                if (_playerInstance.TryGetComponent<PlayerController>(out var pc))
                {
                    PlayerController = pc;

                    // PlayerController에 스탯, 로드아웃을 적용 
                    PlayerController.Initialize(_playerStat, currentLoadout);
                }
                else
                {
                    Debug.LogWarning("[## 중요 ##]플레이어 프리팹에서 PlayerController 컴포넌트를 찾을 수 없습니다.\n 플레이어 장비 초기설정에 실패했습니다.");
                }
            }
            
            // 스폰위치로 플레이어 이동
            PlayerController.Rb.MovePosition(spawnPosition);
        }
        
        /// <summary>
        /// PlayerLoadout에 선택된 장비를 적용
        /// </summary>
        private void InitializeLoadout()
        {
            currentLoadout ??= new PlayerLoadout(); // 인스턴스가 안만들어져있으면 만들어 줌
            
            // 선택된 장비ID 가져오기
            var weaponId = GameManager.GameData.CurrentWeaponID();
            var subWeaponId = GameManager.GameData.CurrentSideWeaponID();
            var emissionId = GameManager.GameData.CurrentEmissionID();
            
            // 기존 이벤트 구독 해제
            if (currentLoadout.Weapon)
                currentLoadout.Weapon.OnStatsChanged -= UpdateWeaponData;
                
            // currentLoadout에 장비 적용
            currentLoadout.Weapon = Instantiate(loadoutsSo.weapons[weaponId]);
            currentLoadout.SubWeapon = Instantiate(loadoutsSo.subWeapons[subWeaponId]);
            currentLoadout.Emission = Instantiate(loadoutsSo.emissions[emissionId]);
            
            // 이벤트 구독
            currentLoadout.Weapon.OnStatsChanged += UpdateWeaponData;
        }

        /// <summary>
        /// 런타임에서 사용할 PlayerStat 인스턴스를 생성 
        /// </summary>
        private void InitializePlayerStat()
        {
            _playerStat = new PlayerStat(playerStatSo.playerStat);
            // TODO 인스턴스가 이미 생성되어있으면 복사해서 값을 수정하는 코드를 추가해야 함
        }
        
        #region 이벤트 콜백 메서드
        
        /// <summary>
        /// 주무기(총기) 속성변경 적용 이벤트 콜백 메서드
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
        
        #region 컨텐스트메뉴 메서드

        [Header("씬 시작시 플레이어 자동생성")] 
        [SerializeField] private bool autoSpawn = true;
        [SerializeField] private GameObject playerPrefab;
        
        [ContextMenu("플레이어 생성 (컴포넌트가 할당된 게임오브젝트의 위치로 플레이어 스폰)")]
        private void CM_PlayerSpawn()
        {
            // 컴포넌트가 할당된 게임오브젝트의 위치로 플레이어 스폰 
            PlayerSpawn(transform.position, playerPrefab);
        }
        
        #endregion

        private void Start()
        {
            if (autoSpawn) CM_PlayerSpawn();
        }
    }
}
