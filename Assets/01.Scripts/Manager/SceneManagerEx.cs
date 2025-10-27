using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : MonoBehaviour
{
  /////////////////////////////////////////////
  /// Scene을 전반적으로 관리하는 Manager
  ////////////////////////////////////////////

  private GameObject _playerPrefab;
  private GameObject _elevatorPrefab;
  private GameObject _startPoint;

  private const string ELEVATOR = "Elevator_";

  private bool isUpdate;
  private int count = 3;
  private float cur = 0f;

  void Start()
  {
    Init();
  }

  void Update()
  {
    if(!isUpdate)
    {
      cur += Time.deltaTime;

      if (cur > count)
      {
        Init();
        cur = 0f;
      }
    }
  }

  private void Init()
  {
    _playerPrefab = GameManager.ResourceEx.playerPrefab;

    var mapPrefabs = GameManager.ResourceEx.mapPrefabDict;

    foreach (var pair in mapPrefabs)
    {
      string key = pair.Key;

      if (key.StartsWith(ELEVATOR))
        _elevatorPrefab = pair.Value;
    }

    if (_elevatorPrefab != null)
    {
      _startPoint = _elevatorPrefab.GetComponent<ElevatorController>().StartPoint;
      isUpdate = true;
    }
    else
      isUpdate = false;
  }

  // 플레이어 최초 생성 (이후 파괴-생성 하지 않고 그대로 유지)
  public void PlayerSpawn()
  {
    Instantiate(_playerPrefab, _startPoint.transform.position, _startPoint.transform.rotation);
  }
}
