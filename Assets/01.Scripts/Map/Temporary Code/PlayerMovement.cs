using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_TPS : MonoBehaviour
{
  private CharacterController cc;
  private Vector3 moveInput; // WASD 입력을 저장할 변수

  [Header("Movement Settings")]
  [SerializeField] private float walkSpeed = 10f;
  [SerializeField] private float runSpeed = 10f;
  private float curSpeed; // 현재 속도 (걷기 또는 뛰기)

  [Header("Camera & Rotation Settings")]
  [SerializeField] private Camera mainCamera;
  [SerializeField] private float mouseSensitivity = 25f;
  private float xRotation = 0f; // 카메라의 상하 회전 각도를 저장할 변수

  [Header("Physics & Interaction")]
  private Vector3 velocity;
  private const float GRAVITY = -9.81f;
  [SerializeField] private float interactionDistance = 3f;

  void Awake()
  {
    cc = GetComponent<CharacterController>();
    curSpeed = walkSpeed; // 기본 속도는 걷기로 설정
  }

  void Start()
  {
    // 게임이 시작되면 마우스 커서를 잠그고 보이지 않게 만듭니다.
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  void Update()
  {
    // 1. 이동 처리: 카메라 방향 기준으로 이동
    MovePlayer();

    // 2. 중력 적용
    ApplyGravity();

    // 3. 상호작용 처리
    HandleInteraction();
  }

  // Input System의 'Move' Action에 연결될 함수
  private void OnMove(InputValue value)
  {
    Vector2 input = value.Get<Vector2>();
    moveInput = new Vector3(input.x, 0, input.y);
  }

  // Input System의 'Look' Action에 연결될 함수
  private void OnLook(InputValue value)
  {
    Vector2 mouseDelta = value.Get<Vector2>();

    float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
    float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

    // 마우스 상하 움직임으로 카메라의 고개를 조절
    xRotation -= mouseY;
    // 카메라가 너무 많이 꺾이지 않도록 각도를 -90도 ~ 90도 사이로 제한
    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

    // 실제 카메라에 상하 회전 적용 (localRotation 사용)
    mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

    // 마우스 좌우 움직임으로 플레이어 전체를 회전
    transform.Rotate(Vector3.up * mouseX);
  }

  private void MovePlayer()
  {
    // 캐릭터가 바라보는 방향(forward)과 오른쪽 방향(right)을 기준으로 이동 벡터 계산
    Vector3 moveDirection = transform.forward * moveInput.z + transform.right * moveInput.x;
    cc.Move(moveDirection.normalized * curSpeed * Time.deltaTime);
  }

  private void ApplyGravity()
  {
    // 땅에 닿아있으면 중력 속도 초기화
    if (cc.isGrounded && velocity.y < 0)
    {
      velocity.y = -2f; // 약간의 음수값으로 안정적으로 땅에 붙도록 함
    }
    velocity.y += GRAVITY * Time.deltaTime;
    cc.Move(velocity * Time.deltaTime);
  }

  private void HandleInteraction()
  {
    // 상호작용 키(F)를 눌렀을 때
    if (Input.GetKeyDown(KeyCode.F)) // New Input System으로 바꾸려면 별도 Action 필요
    {
      Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
      if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
      {
        if (hit.collider.TryGetComponent(out IInteractable interactable))
        {
          interactable.Interact();
        }
      }
    }
  }
}
