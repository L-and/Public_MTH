using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  private CharacterController cc;
  private Vector3 moveInput;
  [SerializeField] private float curSpeed;
  [SerializeField] private float walkSpeed = 2f;
  [SerializeField] private float runSpeed = 5f;
  [SerializeField] private float turnSpeed = 10f;
  [SerializeField] private Camera mainCamera;
  private Vector3 velocity;
  private const float GRAVITY = -9.8f;
  private float interactionDistance = 3f;

  private Transform cam;

  void Awake()
  {
    cc = GetComponent<CharacterController>();

    cam = Camera.main.transform;
  }

  void Update()
  {
    // 이동 관련 당담.
    velocity.y += GRAVITY;
    var dir = moveInput * curSpeed + Vector3.up * velocity.y;
    cc.Move(dir * Time.deltaTime);

    // 회전 담당
    Turn();

    // 상호작용 담당
    if (Input.GetKeyDown(KeyCode.F))
    {
      // 카메라 화면 정중앙에서 앞 방향으로 Ray(광선)를 생성합니다.
      Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
      RaycastHit hit; // Ray가 부딪힌 물체의 정보를 담을 변수

      // Ray를 쏴서 interactionDistance 거리 안에 무언가 부딪혔다면
      if (Physics.Raycast(ray, out hit, interactionDistance))
      {
        // 부딪힌 물체가 IInteractable 인터페이스를 가지고 있는지 확인합니다.
        if (hit.collider.TryGetComponent(out IInteractable interactable))
        {
          // 가지고 있다면, 그 물체의 Interact() 함수를 실행합니다.
          interactable.Interact();
        }
      }
    }
  }

  private void OnMove(InputValue value)
  {
    var move = value.Get<Vector2>();

    moveInput = new Vector3(move.x, 0, move.y);
  }

  private void Turn()
  {
    if (moveInput != Vector3.zero)
    {
      // 이동 방향을 바라보는 회전값(Quaternion) 계산
      Quaternion targetRot = Quaternion.LookRotation(moveInput);

      // 현재 각도에서 목표 각도로 부드럽게 회전
      transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
    }
  }
}
