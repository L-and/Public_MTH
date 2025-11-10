using UnityEngine;

public class ElevatorRoomTrigger : MonoBehaviour
{
  [Header("엘리베이터 방을 나가는 문 Object")]
  [SerializeField] private GameObject _doorController;

  [Header("양 문일 경우 (우측 Object)")]
  [SerializeField] private GameObject _doorController2;

  [Header("Elevator Controller")]
  [SerializeField] private ElevatorController _elevatorController;

  private bool _isOpen = false;

  private bool _isEndRoom = false;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player") && !_isOpen)
    {
      if (_isEndRoom)
      {
        _elevatorController.DoorsOpen(0f);
      }
      else
      {
        _doorController.GetComponent<Door>().Open();

        if (_doorController2 != null) _doorController2.GetComponent<Door>().Open();
      }

      _isOpen = true;

      gameObject.SetActive(false);
    }
  }

  public void IsEndRoom(bool isEndRoom)
  {
    _isEndRoom = isEndRoom;
  }
}
