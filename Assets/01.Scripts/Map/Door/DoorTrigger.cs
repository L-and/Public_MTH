using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
  [Header("이 Collider에 Trigger 될 시 열릴 문 Object")]
  [SerializeField] GameObject doorController;

  private bool _isOpen = false;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player") && !_isOpen)
    {
      doorController.GetComponent<Door>().Open();

      _isOpen = true;

      gameObject.SetActive(false);
    }
  }
}
