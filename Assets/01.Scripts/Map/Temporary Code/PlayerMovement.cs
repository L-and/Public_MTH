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
  private Vector3 velocity;
  private const float GRAVITY = -9.8f;

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
