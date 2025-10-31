using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
  [Header("이 Collider에 Trigger 될 시 열릴 문 Object")]
  [SerializeField] GameObject doorController;

  private bool isOpen = false;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player") && !isOpen)
    {
      isOpen = true;

      doorController.GetComponent<Door>().Open();
    }
  }

  private void OnTriggerExit(Collider other)
  {
    var collider = GetComponent<BoxCollider>();
    collider.enabled = false;

    isOpen = false;
  }
}
