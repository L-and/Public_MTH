using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
  [SerializeField] RoomDoorHandler roomDoorHandler;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      roomDoorHandler.OpenEntryDoor();
    }
  }

  private void OnTriggerExit(Collider other)
  {
    Debug.Log("Exit 호출");
    roomDoorHandler.CloseEntryDoor();

    var collider = GetComponent<BoxCollider>();
    collider.enabled = false;
  }
}
