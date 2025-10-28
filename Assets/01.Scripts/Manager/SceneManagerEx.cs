using System.Collections;
using System.Threading.Tasks;
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
  private bool isPlayerSpawn = false; // 플레이어가 소환 됐는지 확인하는 변수

  private const string ELEVATOR = "Elevator_";

  // 현재 씬이 로드중인지 확인하는 변수
  private bool isLoading = false;

  private bool isUpdate;
  private int count = 3;
  private float cur = 0f;

  //TODO: 게임 메뉴에서 게임 씬으로 넘어갈때 층 +1 하기
 
  void Start()
  {
    SetupScene();
  }

  void Update()
  {
    if(!isUpdate && !isLoading)
    {
      cur += Time.deltaTime;

      if (cur > count)
      {
        SetupScene();
        cur = 0f;
      }
    }
  }

  private void SetupScene()
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

  // 플레이어 스폰 관련 함수
  // 1) 게임메뉴 -> 게임시작 시 플레이어 최초 생성
  // 2) 다음층으로 넘어갈때 (GameScene 다시 로드) 플레이어 위치 이동
  public void PlayerSpawn()
  {
    if (!isPlayerSpawn)
    {
      _curPlayer = Instantiate(_playerPrefab, _startPoint.transform.position, _startPoint.transform.rotation);
    }
    else
    {
      _curPlayer.transform.position = _startPoint.transform.position;
    }
  }

  // 외부에서 Scene 로드를 위해 호출하는 함수
  public void LoadScene(string sceneName)
  {
    if (isLoading)
    {
      Debug.LogWarning("이미 씬을 로드 중 입니다.");
      return;
    }

    StartCoroutine(LoadSceneAsync(sceneName));
  }
  
  // 실제 씬 로드를 하는 함수
  private IEnumerator LoadSceneAsync(string sceneName)
  {
    isLoading = true;

    /// 로딩 UI 또는 페이드 아웃 시작

    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

    while (!asyncLoad.isDone)
    {
      /// 로딩 진행하는 동안 실행될 영역

      yield return null;  // 다음 프레임까지 대기
    }

    /// 로딩 UI 숨기기 또는 페이드 인 시작 부분
    
    isLoading = false;
  }
}
