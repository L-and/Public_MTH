// PlayerController.cs

using System;
using System.Globalization;
using _01.Scripts.Enums;
using _01.Scripts.PlayerControll.Animation;
using _01.Scripts.PlayerControll.Status;
using _01.Scripts.Weapons_ScriptableObjects.Emission;
using _01.Scripts.Weapons_ScriptableObjects.Loadout;
using _01.Scripts.Weapons_ScriptableObjects.SubWeapon;
using _01.Scripts.Weapons_ScriptableObjects.Weapon;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using WeaponBehaviour = _01.Scripts.Weapon.WeaponBehaviour;

namespace _01.Scripts.PlayerControll
{
    /// <summary>
    /// 플레이어에 대한 입력을 받아 필드로 다른 스크립트로 전달,
    /// 컴포넌트에 직접적인 처리를 총괄하는 스크립트
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IDamageableZone
    {
        # region 플레이어 스탯 필드/프로퍼티
        
        /// <summary>
        /// 플레이어의 속성정보 컴포넌트 프로퍼티
        /// </summary>
        public PlayerStat Stat { get; private set; }

        public float CurrentMaxSpeed
        {
            get
            {
                if (MovementFSM.CurrentState is DashState)
                    return Stat.dashMaxSpeed;
                if (MovementFSM.CurrentState is SlidingState)
                    return Stat.slidingMaxSpeed;
                if (MovementFSM.CurrentState is JumpState)
                    return Stat.jumpMaxSpeed;
                
                return Stat.maxSpeed;
            }
        }

        public float CurrentAcceleration
        {
            get
            {
                if (MovementFSM.CurrentState is JumpState)
                    return Stat.AirAcc;
                if (MovementFSM.CurrentState is SlidingState)
                    return Stat.SlidingAcc;

                 return Stat.BaseAcc;
            }
        }
        
        /// <summary>
        /// 무적상태를 판별하는 프로퍼티
        /// </summary>
        public bool IsInvincible => MovementFSM.CurrentState is DashState;
        
        # endregion
        
        # region 컴포넌트/클래스 참조 프로퍼티
  
        // 상태 머신 참조
        public MovementStateMachine MovementFSM { get; private set; } // 사용X
        public SubWeaponStateMachine SubWeaponFSM { get; private set; } // 사용X
        public EmissionStateMachine EmissionFSM { get; private set; }

        // Enum 상태필드
        public EPlayerStates.SubWeaponState SubWeaponState;
        
        // 컴포넌트 참조 프로퍼티
        public CapsuleCollider CapsuleCollider { get; private set; }
        public PlayerInput PlayerInput { get; private set; }
        public Rigidbody Rb { get; private set; }
        public Transform PlayerCamera { get; private set; }

        private Recoil _recoil;
        
        # endregion

        /// <summary>
        /// 캐릭터 팔 애니메이션 스크립트
        /// </summary>
        public CharacterAnimationController CharacterAnimController { get; private set; }
        public Animator GunAnimController  { get; private set; }

        // PlayerInput 입력값 프로퍼티
        public Vector2 MoveInput { get; private set; }
        public Vector2 MouseDeltaInput { get; private set; }

        # region 스테미너 관련
        
        [Header("스테미너 관련 필드")] 
        [Tooltip("초당 스테미너 충전량"), SerializeField] private float staminaRegenAmount = 0.5f;
        [Tooltip("대쉬할 때 스테미너 소모량"), SerializeField] private float dashCost = 1f;
        public float DashCost => dashCost;
        
        # endregion
        
        #region 지면검사관련 필드/메서드
        [Header("지면 검사용 필드")] 
        [Tooltip("지면검사의 기준위치"), SerializeField] public Transform groundCheckPivot;
        [SerializeField] private float groundCheckDistance = 0.02f; // 지면검사용 거리
        [SerializeField] private LayerMask groundLayerMask;
        public bool IsGrounded => Physics.Raycast(groundCheckPivot.position, -transform.up, groundCheckDistance, groundLayerMask);
        #endregion
       
        # region 이동속도, 입력 이동방향 프로퍼티
        /// <summary>
        /// 평면 이동속도
        /// </summary>
        public float PlatSpeed
        {
            get
            {
                return new Vector3(Rb.linearVelocity.x, 0f, Rb.linearVelocity.z).magnitude;
            }
        }
  
        /// <summary>
        /// 입력에 따른 이동방향 프로퍼티
        /// </summary>
        public Vector3 MoveDirection => (transform.forward * MoveInput.y + transform.right * MoveInput.x).normalized; // 플레이어의 Transform과 입력에따른 단위벡터
        
        # endregion
        
        # region 무기/보조무기/방출 관련 필드/컴포넌트
        
        private bool _holdingFire;
        
        /// <summary>
        /// 마지막 발사시간
        /// </summary>
        private float _lastShotTime;

        
        [Header("주무기/보조무기/방출 컴포넌트")]
        [SerializeField] private Transform mainWeaponPosition; // 주무기 장착위치
        
        public WeaponBehaviour equippedWeapon;
        [SerializeField] public PlayerEmission playerEmission;

        public EmissionAbilityData CurrentEmission => playerEmission.data;

        [SerializeField] public PlayerSubWeapon playerSubWeapon;
        
        # endregion
        
        # region 디버그용 필드/프로퍼티
        
        [Header("디버그")]
        [SerializeField] private Text speedText;
        [SerializeField] private Text stateText;
        
        public string MovementState
        {
            get
            {
                return MovementFSM.CurrentState?.ToString();
            }
        }
        
        # endregion
        
        # region MovementState 상태전환 무시 기능
        public bool isMovementStateLocked; // MovementState의 상태전환을 강제로 무시하는 플래그 변수
        private IDamageableZone _damageableZoneImplementation;

        /// <summary>
        /// MovementState의 상태변경을 일정시간 잠금하는 함수
        /// (점프 직후에 공중상태임에도 지면체크가 원활하게 되지않아 사용)
        /// </summary>
        public void UnlockMovementStateLock(float time)
        {
            Invoke("Unlocking", time);
        }

        private void Unlocking()
        {
            isMovementStateLocked = false;
        }
        
        #endregion

        #region 플레이어 초기설정(장비, 스탯) 메서드
        
        /// <summary>
        /// PlayerManager에 의해 호출되어 플레이어의 스탯,장비를 설정합니다.
        /// </summary>
        public void Initialize(PlayerStat stat, PlayerLoadout loadout) // Overload for PlayerManager
        {
            Stat = stat;

            // 보조무기, 방출 컴포넌트 캐싱
            playerEmission = GetComponent<PlayerEmission>();
            playerSubWeapon = GetComponent<PlayerSubWeapon>();
            
            // 무기, 보조무기, 방출 초기설정 진행
            SetupMainWeapon(loadout.Weapon);
            SetupEmission(loadout.Emission);
            SetupSubWeapon(loadout.SubWeapon);
            
            // 무기에 맞는 캐릭터 애니메이터 적용
            CharacterAnimController.Animator.runtimeAnimatorController =
                loadout.Weapon.characterAnimator;
        }

        /// <summary>
        /// 주무기 초기설정 메서드 (GameObject 생성, 컴포넌트 캐싱, weaponData 값 적용)
        /// </summary>
        /// <param name="weaponData"></param>
        private void SetupMainWeapon(WeaponData weaponData)
        {
            if (!weaponData)
            {
                Debug.LogWarning("주무기가 선택되지 않았습니다!");
                return;
            }

            if (!mainWeaponPosition)
            {
                Debug.LogWarning("주무기 장착위치가 할당되지 않았습니다!");
                return;
            }

            var weaponGO = Instantiate(weaponData.prefab, mainWeaponPosition);

            if (!weaponGO.TryGetComponent<WeaponBehaviour>(out equippedWeapon))
            {
                Debug.LogWarning($"[총기: {weaponData.name}] WeaponBehaviour이 없습니다!");
                return;
            }

            GunAnimController = equippedWeapon.GetAnimator();

            UpdateMainWeaponData(weaponData); // 총기 속성값 적용
        }

        /// <summary>
        /// 총기의 속성값(공격력,탄창 등)을 적용하는 메서드
        /// </summary>
        /// <param name="weaponData">총기의 속성값들</param>
        public void UpdateMainWeaponData(WeaponData weaponData)
        {
            // 데미지 설정 TODO 여기가 옳은 위치인가?
            GameManager.PlayerManager.PlayerStat.bulletDamage = weaponData.Damage;
            
            // 탄창/RPM 설정
            equippedWeapon.Initialize();
            equippedWeapon.SetMagazineSize(weaponData.MagazineSize);
            equippedWeapon.SetRateOfFire(weaponData.Rpm);
            
            // 반동설정
            
            if (!_recoil)
            {
                Debug.LogWarning($"[플레이어 프리팹 {name}] Recoil 컴포넌트가 없습니다!");
                return;
            }
            
            _recoil.RecoilX = weaponData.VerticalRecoil;
            _recoil.RecoilY = weaponData.HorizontalRecoil;
            
            
            Debug.Log($"[총기설정 완료] 공격력: {weaponData.Damage}, RPM: {weaponData.Rpm}, 탄창크기: {weaponData.MagazineSize}, 반동: ({weaponData.HorizontalRecoil}, {weaponData.VerticalRecoil})");
        }
        
        /// <summary>
        /// 보조무기 초기설정 메서드
        /// </summary>
        /// <param name="subWeaponData"></param>
        private void SetupSubWeapon(SubWeaponData subWeaponData)
        {
            if (subWeaponData == null)
            {
                Debug.LogWarning("보조무기가 선택되지 않았습니다!");
                return;
            }

            playerSubWeapon.SubWeaponData = subWeaponData;
            Debug.Log($"[보조무기] {subWeaponData.name} 장착됨");
            
            playerSubWeapon.InitializeSubWeapon(this);
        }

        /// <summary>
        /// 방출 초기설정 메서드
        /// </summary>
        /// <param name="emissionData"></param>
        private void SetupEmission(EmissionAbilityData emissionData)
        {
            if (emissionData == null)
            {
                Debug.LogWarning("방출이 선택되지 않았습니다!");
                return;
            }
            playerEmission.data = emissionData;
            Debug.Log($"[방출] {emissionData.abilityName} 장착됨");

            playerEmission.InitializeEmission(this);
        }

        #endregion

        #region Unity Functions

        private void OnDrawGizmos()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;

            Gizmos.DrawRay(groundCheckPivot.position, -transform.up * 10f);
        }
        
        private void Awake()
        {
            // 컴포넌트 초기화
            CapsuleCollider = GetComponent<CapsuleCollider>();
            PlayerInput = GetComponent<PlayerInput>();
            Rb = GetComponent<Rigidbody>();
            PlayerCamera = Camera.main.transform;
            CharacterAnimController = transform.GetComponentInChildren<CharacterAnimationController>();
            
            // private 컴포넌트 캐싱
            _recoil = transform.GetComponentInChildren<Recoil>(); // 반동 컴포넌트
        }

        private void Start()
        {
            // 상태 머신 생성
            MovementFSM = new MovementStateMachine(this);
            SubWeaponFSM = new SubWeaponStateMachine(this);
            EmissionFSM = new EmissionStateMachine(this);

            SubWeaponState = EPlayerStates.SubWeaponState.Ready;
            
            // 각 상태 머신의 초기 상태 설정
            MovementFSM.Initialize(MovementFSM.IdleState);
            SubWeaponFSM.Initialize(SubWeaponFSM.ReadyState);
            EmissionFSM.Initialize(EmissionFSM.ReadyState);
        }
        
        private void Update()
        {
            // 애니메이터에 필요한 값 전달
            CharacterAnimController.movementVelocity = Rb.linearVelocity;
            
            // 무기발사 (TODO 코드이동 필요)
            if (_holdingFire)
            {
                if (Time.time - _lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
                {
                    Fire(); // 사격
                }
            }

            // TODO 제거필요 (보조무기 사용 테스트코드)
            if (Input.GetKeyDown(KeyCode.F))
            {
                playerSubWeapon.ExecuteSubWeapon();
            }


            // 업그레이드 테스트
            if (Input.GetKeyDown(KeyCode.F1))
            {
                GameManager.PlayerManager.currentLoadout.Weapon.VerticalRecoil = -4;
                GameManager.PlayerManager.currentLoadout.Weapon.Rpm = 600;

                GameManager.PlayerManager.currentLoadout.Weapon.MagazineSize = 20;
            }

            // 스테미너 회복
            if (Stat.stamina.Value < Stat.stamina.maxValue)
            {
                Stat.stamina.Value += staminaRegenAmount * Time.deltaTime;
            }
            
            // 각 상태 머신의 Update 로직 실행
            MovementFSM.CurrentState?.OnUpdate();
            SubWeaponFSM.CurrentState?.OnUpdate();
            EmissionFSM.CurrentState?.OnUpdate();
            
            // 디버그
            if (speedText)
                speedText.text = PlatSpeed.ToString(CultureInfo.InvariantCulture);
        }

        private void FixedUpdate()
        {
            // 각 상태 머신의 FixedUpdate 로직 실행 (주로 물리 관련)
            MovementFSM.CurrentState?.OnFixedUpdate();
            SubWeaponFSM.CurrentState?.OnFixedUpdate();
            EmissionFSM.CurrentState?.OnFixedUpdate();
            
            // 디버그
            if (stateText)
                stateText.text = MovementState;
        }
        #endregion

        # region Inputs

        /// <summary>
        /// 플레이어 조작을 활성화
        /// </summary>
        public void ActivePlayerInput()
        {
            PlayerInput.ActivateInput();
        }
        
        /// <summary>
        /// 플레이어 조작을 비활성화
        /// </summary>
        public void DeactivePlayerInput()
        {
            PlayerInput.DeactivateInput();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// 입력에 따라 마우스커서를 On/Off TODO: 커서를 On/Off하는 코드를 공용스크립트로 옮겨야 함
        /// </summary>
        public void OnCursor(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Cursor.visible = !Cursor.visible;
            }

            if (Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        
        /// <summary>
        /// 마우스조작
        /// </summary>
        public void OnLook(InputAction.CallbackContext context)
        {
            MouseDeltaInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// 발사 입력
        /// </summary>
        public void OnTryFire(InputAction.CallbackContext context)
        {
            switch (context)
            {
                case {phase: InputActionPhase.Started}:
                    _holdingFire = true;
                    // Debug.Log("마우스 버튼 클릭");
                    break;
                case {phase: InputActionPhase.Performed}:
                    // Debug.Log("마우스 홀드");
                    break;
                case {phase: InputActionPhase.Canceled}:
                    _holdingFire = false;
                    // Debug.Log("마우스 뗌");
                    break;
            }
        }

        /// <summary>
        /// 재장전 입력
        /// </summary>
        public void OnTryReload(InputAction.CallbackContext context)
        {
            switch (context)
            {
                case {phase: InputActionPhase.Started}:
                    Reload();
                    break;
            }
        }
        
        # endregion

        # region 무기관련 메서드
        
        /// <summary>
        /// 무기 발사
        /// </summary>
        private void Fire()
        {
            if (!equippedWeapon) return;
            if (equippedWeapon.WeaponState == EPlayerStates.WeaponState.Reload) return; // 재장전중일땐 발사못하도록 막음
            
            _lastShotTime = Time.time; // 연사를 위해 발사된 시각 저장
            
            // 발사상태(격발/공격발)에 따라 캐릭터 애니메이션 재생
            CharacterAnimController.FireAnimation(equippedWeapon.HasAmmunition());
            
            equippedWeapon.Fire();
        }

        /// <summary>
        /// 무기 재장전
        /// </summary>
        private void Reload()
        {
            // 재장전 진행중이라면 캔슬못하도록 실행막기
            if (equippedWeapon.WeaponState == EPlayerStates.WeaponState.Reload) return;
            
            CharacterAnimController.ReloadAnimation(!equippedWeapon.HasAmmunition());
            equippedWeapon.Reload();
        }
        
        # endregion
        
        # region 애니메이션 이벤트 메서드

        public void EjectCasing()
        {
            equippedWeapon.EjectCasing();
        }
        
        public void FillAmmunition(int amount)
        {
            equippedWeapon.FillAmmunition(-1);
        }
        
        # endregion
        
        #region Coordinator (중재자) 역할
        // 다른 상태 머신이 현재 상태를 쉽게 조회할 수 있도록 프로퍼티 제공
        public bool IsUsingSubWeapon => SubWeaponState is EPlayerStates.SubWeaponState.Using;
        public bool IsUsingEmission => EmissionFSM.CurrentState is EmissionUsingState;
        public bool IsReloading => equippedWeapon.WeaponState is EPlayerStates.WeaponState.Reload;
        // 두 왼손 액션 중 하나라도 사용 중인지 확인하는 편의용 프로퍼티
        public bool IsAnyLeftHandActionInUse => IsUsingSubWeapon || IsUsingEmission;
        
        #endregion
        
        # region 플레이어 조작관련 Rigidbody 메서드

        /// <summary>
        /// 플레이어가 바라보는 방향으로 이동을 적용하는 메서드
        /// </summary>
        public void MovePlayer(float acceleration)
        {
            Rb.AddForce(MoveDirection * acceleration, ForceMode.Force);

            Debug.DrawRay(transform.position, MoveDirection, Color.yellow);
        }

        /// <summary>
        /// 최대속도 제한을 적용하는 메서드
        /// </summary>
        public void LimitSpeed()
        {
            Vector3 flatVel = new Vector3(Rb.linearVelocity.x, 0f, Rb.linearVelocity.z);
            
            // 속도제한 적용
            if (flatVel.magnitude > CurrentMaxSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * CurrentMaxSpeed;
                Rb.linearVelocity = new Vector3(limitedVel.x, Rb.linearVelocity.y, limitedVel.z);
            }
        }

        public void Jump()
        {
            if (!IsGrounded) return;
            
            Rb.AddForce(transform.up * Stat.CurrentJumpForce, ForceMode.Impulse);
        }

        public void Dash()
        {
            // 방향조작이 있으면 해당방향, 없으면 바라보는 방향으로 대쉬방향 계산
            var dir = (MoveInput == Vector2.zero) ? 
                transform.forward : 
                MoveDirection;
        
            var velocity = dir * Stat.dashPower;
            Rb.AddForce(velocity, ForceMode.Impulse);
        }

        public void Sliding()
        {
            var slidingForce = Stat.slidingPower;
            var velocity = MoveDirection * slidingForce; 
            Rb.AddForce(velocity, ForceMode.Force);
            
        }
        # endregion

        # region 애니메이션 SMB 메서드 (장비의 FSM 상태변경을 위함)

        /// <summary>
        /// 캐릭터 애니메이터에의 Emission 레이어에서 Default State가 실행되면 왼손이 사용종료되었음을 FSM에 알려줌
        /// </summary>
        public void SetLeftHandStateCooldown()
        {
            Debug.Log("왼손 상태를 CooldownStat로 설정!");
            EmissionFSM?.ChangeState(EmissionFSM.CooldownState);
            SubWeaponFSM?.ChangeState(SubWeaponFSM.CooldownState);
        }
        
        # endregion
        
        #region 업그레이드 적용 관련

        // 슬라이딩 업그레이드 시 몇초당 과열게이지를 충전할지를 판단하는 변수
        public float getOverHeatSecWithUpgrade = 2f;
        // [업그레이드] 슬라이딩 업그레이드 플래그
        public  bool IsSlidingUpgrade { get; private set; }
        
        // 슬라이딩 업그레이드
        [ContextMenu("슬라이딩 업그레이드")]
        public void SlidingUpgrade()
        {
            IsSlidingUpgrade = true;
        }

        /// <summary>
        /// 재장전 속도증가 업그레이드를 적용 TODO 업그레이드 적용기능을 담당하는 스크립트를 만들어야할듯
        /// </summary>
        /// <param name="value"></param>
        public void UpgradeReloadSpeed(float value)
        {
            GunAnimController.SetFloat("Reload Speed", value); // TODO 직접 총기애니메이터를 참조해서 파라미터를 변경하는거라 리팩터링 필요
            CharacterAnimController.SetReloadSpeed(value);
        }

        [ContextMenu("재장전 업그레이드 적용")]
        public void UpgradeReload()
        {
            UpgradeReloadSpeed(1.5f);
        }
        
        #endregion
        /// <summary>
        /// 방출공격 사용
        /// </summary>
        public void FireEmission()
        {
            playerEmission.ExecuteEmission(CurrentEmission);
        }
        
        // 데미지 적용
        public void ApplyDamage(float damage)
        {
            Debug.Log("플레이어 피격");
            // 대쉬중에는 데미지적용 X, 스타일리쉬 액션 실행
            if (IsInvincible)
            {
                StyleEventManager.TriggerStyleAction(EStyleType.Dodge);
                return;
            }
            Stat.hp.Value -= damage;

            if (Stat.hp.Value <= 0f)
            {
                Debug.Log("## 플레이어 사망 ##");
                // TODO 플레이어 사망로직 추가
            }
        }

        public bool IsDead => Stat.hp.Value <= 0f;
        public void ApplyHit(float rawDamage, Vector3 hitPoint, HitZones zone)
        {
            ApplyDamage(rawDamage);
        }
    }
}