// PlayerController.cs

using System;
using System.Globalization;
using _01.Scripts.PlayerControll.Animation;
using _01.Scripts.PlayerControll.Status;
using _01.Scripts.Weapons_ScriptableObjects.Emission;
using _01.Scripts.Weapons_ScriptableObjects.Loadout;
using _01.Scripts.Weapons_ScriptableObjects.SubWeapon;
using UnityEngine;
using UnityEngine.InputSystem;
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
        public MovementStateMachine MovementFSM { get; private set; }
        public SubWeaponStateMachine SubWeaponFSM { get; private set; }
        public EmissionStateMachine EmissionFSM { get; private set; }

        // 컴포넌트 참조 프로퍼티
        public CapsuleCollider CapsuleCollider { get; private set; }
        public PlayerInput PlayerInput { get; private set; }
        public Rigidbody Rb { get; private set; }
        public Transform PlayerCamera { get; private set; }
        
        # endregion

        /// <summary>
        /// 캐릭터 팔 애니메이션 스크립트
        /// </summary>
        public CharacterAnimationController CharacterAnimController { get; private set; }


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
        /// <summary>
        /// 현재무기 스크립트 TODO WeaponSO를 사용하도록 수정필요
        /// </summary>
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
        }

        /// <summary>
        /// PlayerManager에 의해 호출되어 플레이어의 스탯과 장비를 설정합니다.
        /// </summary>
        public void Initialize(PlayerStat stat, PlayerLoadout loadout) // Overload for PlayerManager
        {
            Stat = stat;

            // 보조무기, 방출 컴포넌트 캐싱
            playerEmission = GetComponent<PlayerEmission>();
            playerSubWeapon = GetComponent<PlayerSubWeapon>();
            
            // 무기, 보조무기, 방출 초기설정 진행
            
            // Removed: SetupWeapon(loadout.SelectedWeapon);
            SetupEmission(loadout.Emission);
            SetupSubWeapon(loadout.SubWeapon);
                
            // 상태 머신 생성
            MovementFSM = new MovementStateMachine(this);
            SubWeaponFSM = new SubWeaponStateMachine(this);
            EmissionFSM = new EmissionStateMachine(this);
            
            // 각 상태 머신의 초기 상태 설정
            MovementFSM.Initialize(MovementFSM.IdleState);
            SubWeaponFSM.Initialize(SubWeaponFSM.ReadyState);
            EmissionFSM.Initialize(EmissionFSM.ReadyState);
        }

        // Removed: private void SetupWeapon(WeaponDataSO weaponData) { ... }
        
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


        #region Unity Functions

        private void Update()
        {
            // 애니메이터에 필요한 값 전달
            CharacterAnimController.movementVelocity = Rb.linearVelocity;
            
            // 무기발사 (코드이동 필요)
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
            
            // TODO 제거필요 (무기데미지 변경 테스트코드)
            if (Input.GetKeyDown(KeyCode.P))
            {
                GameManager.PlayerManager.PlayerStat.bulletDamage = 20;
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
            speedText.text = PlatSpeed.ToString(CultureInfo.InvariantCulture);
        }

        private void FixedUpdate()
        {
            // 각 상태 머신의 FixedUpdate 로직 실행 (주로 물리 관련)
            MovementFSM.CurrentState?.OnFixedUpdate();
            SubWeaponFSM.CurrentState?.OnFixedUpdate();
            EmissionFSM.CurrentState?.OnFixedUpdate();
            
            // 디버그
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

        /// <summary>
        /// 방출키 입력
        /// </summary>
        public void OnTryEmission(InputAction.CallbackContext context)
        {
            switch (context)
            {
                case {phase: InputActionPhase.Started}:
                    // TODO
                    // EmissionFSM의 EmissionUsingState.Enter에서
                    // 애니메이션과 동작이 실행되도록 수정해야 함
                    // emission.ExecuteEmission(emissiondata);
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
            _lastShotTime = Time.time;

            equippedWeapon.Fire();
        }

        /// <summary>
        /// 무기 재장전
        /// </summary>
        private void Reload()
        {
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
        public bool IsUsingSubWeapon => SubWeaponFSM.CurrentState is SubWeaponUsingState;
        public bool IsUsingEmission => EmissionFSM.CurrentState is EmissionUsingState;
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
            
            Debug.Log("점프성공!");
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

        /// <summary>
        /// 방출공격 사용
        /// </summary>
        public void FireEmission()
        {
            playerEmission.ExecuteEmission(CurrentEmission);
        }
        
        public void ApplyDamage(float damage)
        {
            if (IsInvincible)
            {
                // TODO 무적상태에서 피격시 스타일리쉬액션 연동코드 작성
                return;
            }
            Debug.Log("플레이어 피격당함");
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
            Debug.Log("공격당함!");
            Stat.hp.Value -= rawDamage;

            if (IsDead)
            {
                // TODO: 플레이어 사망처리 코드 작성
                Debug.Log("## 플레이어 사망 ##");
            }
        }
    }
}