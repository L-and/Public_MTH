using UnityEngine;

public class RoomDoorHandler : MonoBehaviour
{
  [SerializeField] private Transform entryDoor;
  [SerializeField] private Transform exitDoor;

  private bool playerInCheck = false;
  private bool isEnemyAlive = false;

  void Update()
  {
    if(playerInCheck)
    {
      if(!isEnemyAlive)
      {
        OpenAllDoor();
      }
    }
  }

  public void OpenEntryDoor()
  {
    entryDoor.position += new Vector3(0, 3.55f, 0);
  }

  public void CloseEntryDoor()
  {
    entryDoor.position -= new Vector3(0, 3.55f, 0);

    playerInCheck = true;
  }

  public void OpenAllDoor()
  {
    entryDoor.position = new Vector3(0, 5.3f, 0);
    exitDoor.position = new Vector3(0, 5.3f, 0);
  }
}
