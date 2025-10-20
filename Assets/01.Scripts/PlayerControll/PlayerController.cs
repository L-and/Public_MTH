// PlayerController.cs

using System;
using System.Globalization;
using _01.Scripts.PlayerControll.Status;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 플레이어의 최상위 컨트롤러이자, 각 상태 머신을 중재하는 Coordinator 역할을 합니다.
namespace _01.Scripts.PlayerControll
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerStatus))]
    public class PlayerController : MonoBehaviour
    {
        // 플레이어 모델 오브젝트
        [SerializeField] private Transform playerOrientation;
        
        /// <summary>
        /// 플레이어의 속성정보 컴포넌트 프로퍼티
        /// </summary>
        public PlayerStatus Status { get; private set; }
        
        // 상태 머신 참조
        public MovementStateMachine MovementFSM { get; private set; }
        public RightHandStateMachine RightHandFSM { get; private set; }
        public SubWeaponStateMachine SubWeaponFSM { get; private set; }
        public EmissionStateMachine EmissionFSM { get; private set; }

        // 컴포넌트 참조 프로퍼티
        public CapsuleCollider CapsuleCollider { get; private set; }
        public PlayerInput PlayerInput { get; private set; }
        public Rigidbody Rb { get; private set; }
        public Animator Anim { get; private set; }
        public Transform PlayerCamera { get; private set; }

        // 입력 값
        public Vector2 MoveInput { get; private set; }
        public Vector2 mouseDeltaInput;
        private float _xRotation;
        private float _yRotation;
        
        // 이동방향 프로퍼티
        public Vector3 MoveDirection => (transform.forward * MoveInput.y + transform.right * MoveInput.x).normalized; // 플레이어의 Transform과 입력에따른 단위벡터

        // 지면검사관련 필드/메서드
        [Header("지면 검사")] 
        [Tooltip("지면검사의 기준위치"), SerializeField] public Transform groundCheckPivot; // 지면검사의 기준위치 (플레이어의 발 아래지점)
        [SerializeField] private float groundCheckDistance = 0.02f; // 지면검사용 거리
        [SerializeField] private LayerMask groundLayerMask;
        public bool IsGrounded => Physics.Raycast(groundCheckPivot.position, -transform.up, groundCheckDistance, groundLayerMask);
        
        // 디버그용 
        [SerializeField] private Text speedText;
        [SerializeField] private Text stateText;
        
        public float PlatSpeed
        {
            get
            {
                return new Vector3(Rb.linearVelocity.x, 0f, Rb.linearVelocity.z).magnitude;
            }
        }

        public string MovementState
        {
            get
            {
                string currState;
                return MovementFSM.CurrentState.ToString();
            }
        }

        # region MovementState 상태전환 무시 기능
        public bool isMovementStateLocked; // MovementState의 상태전환을 강제로 무시하는 플래그 변수

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
            Status = GetComponent<PlayerStatus>();
            // 컴포넌트 초기화
            CapsuleCollider = GetComponent<CapsuleCollider>();
            PlayerInput = GetComponent<PlayerInput>();
            Rb = GetComponent<Rigidbody>();
            Anim = GetComponent<Animator>();
            PlayerCamera = Camera.main.transform;

            // 상태 머신 생성
            MovementFSM = new MovementStateMachine(this);
            RightHandFSM = new RightHandStateMachine(this);
            SubWeaponFSM = new SubWeaponStateMachine(this);
            EmissionFSM = new EmissionStateMachine(this);
        }

        #region Unity Functions
        private void Start()
        {
            // 각 상태 머신의 초기 상태 설정
            MovementFSM.Initialize(MovementFSM.IdleState);
            RightHandFSM.Initialize(RightHandFSM.IdleState);
            SubWeaponFSM.Initialize(SubWeaponFSM.ReadyState);
            EmissionFSM.Initialize(EmissionFSM.ReadyState);
        }

        private void Update()
        {
            // 각 상태 머신의 Update 로직 실행
            MovementFSM.CurrentState?.OnUpdate();
            RightHandFSM.CurrentState?.OnUpdate();
            SubWeaponFSM.CurrentState?.OnUpdate();
            EmissionFSM.CurrentState?.OnUpdate();
            
            // 디버그
            speedText.text = PlatSpeed.ToString(CultureInfo.InvariantCulture);
        }

        private void FixedUpdate()
        {
            Aim();
            
            // 각 상태 머신의 FixedUpdate 로직 실행 (주로 물리 관련)
            MovementFSM.CurrentState?.OnFixedUpdate();
            RightHandFSM.CurrentState?.OnFixedUpdate();
            SubWeaponFSM.CurrentState?.OnFixedUpdate();
            EmissionFSM.CurrentState?.OnFixedUpdate();
            
            // 디버그
            stateText.text = MovementState;
        }


        #endregion

        # region InputSystem 
        private void OnMove(InputValue value)
        {
            MoveInput = value.Get<Vector2>();
        }

        // 입력에 따라 마우스커서를 On/Off
        // TODO: 커서를 On/Off하는 코드를 공용스크립트로 옮겨야 함
        private void OnCursor(InputValue value)
        {
            if (value.isPressed)
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
        
        // 마우스 조작
        private void OnLook(InputValue value)
        {
            mouseDeltaInput = value.Get<Vector2>();
        }
        
        # endregion
        
        #region Coordinator (중재자) 역할
        // 다른 상태 머신이 현재 상태를 쉽게 조회할 수 있도록 프로퍼티 제공
        public bool IsReloading => RightHandFSM.CurrentState is WeaponReloadState;
        public bool IsUsingSubWeapon => SubWeaponFSM.CurrentState is SubWeaponUsingState;
        public bool IsUsingEmission => EmissionFSM.CurrentState is EmissionUsingState;
        // 두 왼손 액션 중 하나라도 사용 중인지 확인하는 편의용 프로퍼티
        public bool IsAnyLeftHandActionInUse => IsUsingSubWeapon || IsUsingEmission;
        #endregion
        
        # region 플레이어 조작관련 메서드(키보드)

        /// <summary>
        /// 플레이어가 바라보는 방향으로 이동을 적용하는 메서드
        /// </summary>
        public void MovePlayer(float acceleration)
        {
            Rb.AddForce(MoveDirection * acceleration, ForceMode.Force);

            Debug.DrawRay(transform.position, MoveDirection, Color.yellow);
            // Debug.Log($"Speed: {Rb.linearVelocity.magnitude}");
            
        }

        /// <summary>
        /// 최대속도 제한을 적용하는 메서드
        /// </summary>
        public void LimitSpeed()
        {
            Vector3 flatVel = new Vector3(Rb.linearVelocity.x, 0f, Rb.linearVelocity.z);
            
            // 속도제한 적용
            if (flatVel.magnitude > Status.CurrentMaxSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * Status.CurrentMaxSpeed;
                Rb.linearVelocity = new Vector3(limitedVel.x, Rb.linearVelocity.y, limitedVel.z);
            }
        }

        public void Jump()
        {
            if (!IsGrounded) return;
            
            Debug.Log("점프성공!");
            Rb.AddForce(transform.up * Status.CurrentJumpForce, ForceMode.Impulse);
        }

        public void Dash()
        {
            // TODO 스테미너에 따른 대쉬가능여부 처리
            
            // 방향조작이 있으면 해당방향, 없으면 바라보는 방향으로 대쉬방향 계산
            var dir = (MoveInput == Vector2.zero) ? 
                transform.forward : 
                MoveDirection;
            
            var velocity = dir * Status.dashPower;
            Rb.AddForce(velocity, ForceMode.Impulse);
        }

        public void Sliding()
        {
            var slidingForce = Status.slidingPower;
            var velocity = MoveDirection * slidingForce; 
            Rb.AddForce(velocity, ForceMode.Force);
            
        }
        # endregion
        
        # region 플레이어 조작관련 메서드(마우스)

        public void Aim()
        {
            var mouseX = mouseDeltaInput.x * Status.mouseSensitivity;
            var mouseY = mouseDeltaInput.y * Status.mouseSensitivity;

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
          
            transform.rotation = Quaternion.Euler(0.0f, _yRotation, 0f);
            playerOrientation.rotation = Quaternion.Euler(_xRotation, _yRotation, 0.0f);
        }
        
        # endregion
    }
}