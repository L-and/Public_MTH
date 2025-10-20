using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
  [SerializeField] RoomController roomController;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      roomController.PlayerInRoom();
    }
  }
}
