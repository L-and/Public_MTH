using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
  [SerializeField] DoorController doorController;

  private bool isOpen = false;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player") && !isOpen)
    {
      isOpen = true;

      doorController.OpenDoor();
    }
  }

  private void OnTriggerExit(Collider other)
  {
    var collider = GetComponent<BoxCollider>();
    collider.enabled = false;

    isOpen = false;
  }
}
