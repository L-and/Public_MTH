using System.Collections;
using UnityEngine;

public class VerticalSlidingDoorController : Door
{
  [Header("문이 열리는 최대 높이")]
  [SerializeField] private float openHeight = 5.3f;   // 문이 열리는 최대 높이

  private Vector3 closeDoorPos; // 닫혔을때 위치
  private Vector3 openDoorPos;  // 열렸을때 위치

  void Start()
  {
    // 위치 값 초기화
    closeDoorPos = transform.position;
    openDoorPos = new Vector3(transform.position.x, openHeight, transform.position.z);
  }

  public override void Open()
  {
    StartCoroutine(SetupDoorState(openDoorPos));
  }

  public override void Close()
  {
    StartCoroutine(SetupDoorState(closeDoorPos));
  }
  
  // 문 상태 변화 동작 관련 함수
  private IEnumerator SetupDoorState(Vector3 endPos)
  {
    var timeSpeed = 0f;
    var startPos = transform.position;

    while (timeSpeed <= 1f)
    {
      timeSpeed += Time.deltaTime * doorMoveSpeed;

      transform.position = Vector3.Lerp(startPos, endPos, timeSpeed);

      yield return null;
    }

    transform.position = endPos;
  }
}
