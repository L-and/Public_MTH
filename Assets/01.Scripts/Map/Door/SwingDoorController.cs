using System.Collections;
using UnityEngine;

public class SwingDoorController : Door
{
  [Header("문이 열리는 최대 각도")]
  [SerializeField] private float maxOpenAngle = 90f;

  [Header("문이 닫히는 최소 각도")]
  [SerializeField] private float minCloseAngle = 0f;

  private Quaternion closeDoorAngle;  // 닫혔을때 회전각
  private Quaternion openDoorAngle;   // 열렸을때 회전각

  void Start()
  {
    // 회전 값 초기화
    closeDoorAngle = Quaternion.Euler(0f, minCloseAngle, 0f);
    openDoorAngle = Quaternion.Euler(0f, maxOpenAngle, 0f);
  }

  public override void Open()
  {
    StartCoroutine(SetupDoorState(openDoorAngle));
  }

  public override void Close()
  {
    StartCoroutine(SetupDoorState(closeDoorAngle));
  }

  // 문 상태 변화 동작 관련 함수
  private IEnumerator SetupDoorState(Quaternion endRot)
  {
    var timeSpeed = 0f;
    var startRot = transform.rotation;

    while (timeSpeed <= 1f)
    {
      timeSpeed += Time.deltaTime * doorMoveSpeed;

      transform.rotation = Quaternion.Lerp(startRot, endRot, timeSpeed);

      yield return null;
    }

    transform.rotation = endRot;
  }
}
