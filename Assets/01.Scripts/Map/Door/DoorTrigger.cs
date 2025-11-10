using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
  [Header("이 Collider에 Trigger 될 시 열릴 문 Object")]
  [SerializeField] private GameObject _doorController;

  [Header("양 문일 경우 (오른쪽 Object)")]
  [SerializeField] private GameObject _doorController2;

  private bool _isOpen = false;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player") && !_isOpen)
    {
      _doorController.GetComponent<Door>().Open();

      if (_doorController2 != null) _doorController2.GetComponent<Door>().Open();

      _isOpen = true;

      gameObject.SetActive(false);
    }
  }
}
