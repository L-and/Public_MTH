using System.Collections;
using UnityEngine;

public class SwingDoorController : Door
{
  [Header("문이 열리는 최대 각도")]
  [SerializeField] private float _maxOpenAngle = 90f;

  private Quaternion _closeDoorAngle;  // 닫혔을때 회전각
  private Quaternion _openDoorAngle;   // 열렸을때 회전각

  void Start()
  {
    // 프리팹상에서 현재 회전 값을 닫힌 위치로 파악 하도록 함.
    _closeDoorAngle = transform.localRotation;
    _openDoorAngle = _closeDoorAngle * Quaternion.Euler(0f, _maxOpenAngle, 0f);
  }

  public override void Open()
  {
    StartCoroutine(SetupDoorState(_openDoorAngle));
  }

  public override void Close()
  {
    StartCoroutine(SetupDoorState(_closeDoorAngle));
  }

  // 문 상태 변화 동작 관련 함수
  private IEnumerator SetupDoorState(Quaternion endRot)
  {
    var timeSpeed = 0f;
    var startRot = transform.localRotation;

    while (timeSpeed <= 1f)
    {
      timeSpeed += Time.deltaTime * doorMoveSpeed;

      transform.localRotation = Quaternion.Lerp(startRot, endRot, timeSpeed);

      yield return null;
    }

    transform.localRotation = endRot;
  }
}
