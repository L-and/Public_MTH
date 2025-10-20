using UnityEngine;

public class RoomController : MonoBehaviour
{
  [SerializeField] private DoorController entryDoor;
  [SerializeField] private DoorController exitDoor;

  private bool isGameStart = false;
  private bool isOpen = false;

  private float timeCount = 5f;

  void Update()
  {
    if (!isOpen && IsRoomClear())
    {
      isOpen = true;

      entryDoor.OpenDoor();
      exitDoor.OpenDoor();
    }
  }

  public void PlayerInRoom()
  {
    entryDoor.CloseDoor();

    isGameStart = true;

    EnemySpawn();
  }

  private void EnemySpawn()
  {
    //TODO: 적 생성
  }

  private bool IsRoomClear()
  {
    if (isGameStart)
    {
      if (timeCount >= 0)
      {
        timeCount -= Time.deltaTime;
      }
      else
      {
        return true;
      }
    }
    return false;
  }
}
