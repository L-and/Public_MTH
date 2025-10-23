using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
  [SerializeField] private float openHeight = 5.3f;   // 문이 열리는 최대 높이
  [SerializeField] private float doorMoveSpeed = 1f;  // 문이 열리는 속도

  private Vector3 initDoorPos;  // 
  private Vector3 openDoorPos;

  void Start()
  {
    initDoorPos = transform.position;
    openDoorPos = initDoorPos + new Vector3(0, 3.55f, 0);
  }

  public void OpenDoor()
  {
    StartCoroutine(SetDoorState(openDoorPos));
  }

  public void CloseDoor()
  {
    StartCoroutine(SetDoorState(initDoorPos));
  }
  
  IEnumerator SetDoorState(Vector3 endPos)
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
