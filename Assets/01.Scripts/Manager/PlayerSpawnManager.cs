using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
  private GameObject _playerPrefab;   // 플레이어 프리팹을 가지고 있는 변수
  private GameObject _curPlayer;      // 현재 생성되어있는 플레이어를 나타내는 변수
  private GameObject _elevatorPrefab; // 엘리베이터 프리팹을 가지고 있는 변수
  private GameObject _startPoint;     // 시작 위치를 나타내는 변수
  private bool _isPlayerSpawn = false; // 플레이어가 소환 됐는지 확인하는 변수

  

  private void SetupGameScene()
  {
    // 플레이어와 엘리베이터 프리팹 가져옴.
    _playerPrefab = GameManager.ResourceEx.playerPrefab;
    _elevatorPrefab = GameManager.ResourceEx.GetElevatorPrefab(Constants.ELEVATOR_PROTOTYPE);

    if (_elevatorPrefab != null)
      _startPoint = _elevatorPrefab.GetComponent<ElevatorController>().StartPoint;
    else
    {
      Debug.LogError("엘리베이터 프리팹 리소스를 제대로 불러오지 못해서 시작 포인트를 찾지 못했습니다.\n 시작 지점 위치를 (0,0,0)으로 이동합니다.");
      _startPoint.transform.position = new Vector3(0, 0, 0);
    }
  }

  // 플레이어 스폰 관련 함수
  // 1) 게임메뉴 -> 처음 게임시작 시 플레이어 최초 생성
  // 2) 다음층으로 넘어갈때 (GameScene 다시 로드) 플레이어를 다시 생성하지 않고 위치만 이동
  public void PlayerSpawn()
  {
    if (!_isPlayerSpawn)
    {
      SetupGameScene();
      _curPlayer = Instantiate(_playerPrefab, _startPoint.transform.position, _startPoint.transform.rotation);
      DontDestroyOnLoad(_curPlayer.gameObject);
      _isPlayerSpawn = true;
    }
    else
    {
      _curPlayer.SetActive(false);
      
      // 2. 위치 회전값 초기화.
      _curPlayer.transform.position = _startPoint.transform.position;
      _curPlayer.transform.rotation = _startPoint.transform.rotation;

      _curPlayer.SetActive(true);
    }
  }
}
