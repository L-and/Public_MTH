using UnityEngine;

public class ElevatorController : MonoBehaviour
{
  [Header("엘리베이터 내 StartPoint")]
  [SerializeField] private GameObject _startPoint;

  [Header("엘리베이터 내 다음 층 내려가는 Trigger")]
  [SerializeField] private GameObject _reloadTrigger;

  [Header("엘리베이터 문 막는 임시 벽 Object")]
  [SerializeField] private GameObject _doorBlocker;

  public GameObject StartPoint
  {
    get { return _startPoint; }
    private set { _startPoint = value; }
  }

  public void SetupForStart()
  {
    if (_startPoint != null) _startPoint.SetActive(true);

    if (_reloadTrigger != null) _reloadTrigger.SetActive(false);
  }
  
  public void SetupForEnd()
  {
    if (_startPoint != null) _startPoint.SetActive(false);

    if (_reloadTrigger != null) _reloadTrigger.SetActive(true);
  }
}
