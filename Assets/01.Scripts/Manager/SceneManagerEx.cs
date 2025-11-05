using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : MonoBehaviour
{
  /////////////////////////////////////////////
  /// Scene을 전반적으로 관리하는 Manager
  ////////////////////////////////////////////

  private GameObject _playerPrefab;   // 플레이어 프리팹을 가지고 있는 변수
  private GameObject _curPlayer;      // 현재 생성되어있는 플레이어를 나타내는 변수
  private GameObject _elevatorPrefab; // 엘리베이터 프리팹을 가지고 있는 변수
  private GameObject _startPoint;     // 시작 위치를 나타내는 변수
  private bool _isPlayerSpawn = false; // 플레이어가 소환 됐는지 확인하는 변수

  // 현재 씬이 로드중인지 확인하는 변수
  private bool _isLoading = false;

  //TODO: 게임 메뉴에서 게임 씬으로 넘어갈때 층 +1 하기

  private void SetupGameScene()
  {
    // 플레이어와 엘리베이터 프리팹 가져옴.
    _playerPrefab = GameManager.ResourceEx.playerPrefab;
    _elevatorPrefab = GameManager.ResourceEx.elevatorPrefab;

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
      _curPlayer = Instantiate(_playerPrefab, _startPoint.transform.position, _startPoint.transform.rotation);
      _isPlayerSpawn = true;
    }
    else
    {
      _curPlayer.transform.position = _startPoint.transform.position;
    }
  }

  /// <summary>
  /// 외부에서 Scene 로드를 위해 호출하는 함수
  /// true를 넣으면 SetupGameScene()를 실행함.
  /// 이는 GameScene 관련 세팅이기 때문에 GameScene으로 가는 경우가 아니거나
  /// 아래 경우가 아닌 경우엔 보통은 false를 쓰면 됨.
  /// 
  /// *** true를 쓰는 상황 ***
  /// 1) 메인메뉴 -> 게임씬으로 넘어가는 경우
  /// 2) 게임씬에서 맵 컨셉이 바뀌는 경우 (현재 일정 층 이상 지나면 다른 컨셉의 맵이 나오도록 구현함.)
  /// 
  /// *** false를 쓰는 상황 ***
  /// 1) 컷씬 -> 메인메뉴, 게임씬 -> 메인메뉴
  /// 
  /// *** 죽어서 재시작할 경우 두가지 중 하나 선택 할 수 있도록 ***
  /// 1) 1층부터 다시 함 -> true
  /// 2) 해당층부터 다시 하면서 플레이어 정보 그대로 들고갈 경우 -> false
  /// </summary>
  /// <param name="sceneName">씬 이름 매개변수</param>
  /// <param name="isInitialSpawn">처음 시작인지 체크</param>
  public void LoadScene(string sceneName, bool isInitialSpawn)
  {
    if (_isLoading)
    {
      Debug.LogWarning("이미 씬을 로드 중 입니다.");
      return;
    }

    if (isInitialSpawn)
      SetupGameScene();

    StartCoroutine(LoadSceneAsync(sceneName));
  }
  
  // 실제 씬 로드를 하는 함수
  private IEnumerator LoadSceneAsync(string sceneName)
  {
    _isLoading = true;

    /// 로딩 UI 또는 페이드 아웃 시작

    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

    while (!asyncLoad.isDone)
    {
      /// 로딩 진행하는 동안 실행될 영역

      yield return null;  // 다음 프레임까지 대기
    }

    /// 로딩 UI 숨기기 또는 페이드 인 시작 부분
    
    _isLoading = false;
  }
}
