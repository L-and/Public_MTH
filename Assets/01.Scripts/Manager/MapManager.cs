using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapManager : MonoBehaviour
{
  private Dictionary<string, GameObject> _mapPrefabs; // 현재 층의 맵 리소스가 전부 들어있는 변수

  private List<GameObject> _roomPrefabs; // 방 프리팹만 가지고 있는 리스트 변수
  // private List<GameObject> connectorPrefabs;
  private GameObject _connectorPrefab;  // 통로 프리팹
  private GameObject _elevatorPrefab;   // 엘리베이터 프리팹

  private TextMeshProUGUI floorCountText; // 현재 층 표시하는 TextUI
  private Transform _mapRoot;             // 맵 생성 부모
  private Transform _attachPoint;         // 현재 진행 Anchor

  private GameObject _startElevator;  // 시작부분 엘리베이터 Object

  private bool _isAllUsed;    // 맵 리스트 전부 훑었는지 체크
  private bool _isCount;      // 재시작하는건지 다음층 넘어가는건지 체크
  private bool _isBossLevel;  // 

  async void Start()
  {
    // 재시작 or 다음층 넘어가는지 체크
    if (!_isCount)
    {
      GameManager.GameData.currentFloor++;
      _isCount = true;
    }

    var mapConceptName = GameManager.GameData.GetMapConceptForFloor();

    var elevatorPrefab = GameManager.ResourceEx.GetElevatorPrefab(mapConceptName);

    // 엘리베이터 프리팹이 null이면 다시 불러옴.
    if (elevatorPrefab == null)
    {
      Debug.LogWarning("맵을 생성하는 과정에서 Elevator Prefab이 Null 이어서 맵을 제대로 생성하지 못했습니다.");

      // 리소스 다시 불러오기
      await GameManager.ResourceEx.LoadElevatorPrefabs();

      elevatorPrefab = GameManager.ResourceEx.GetElevatorPrefab(mapConceptName);

      if (elevatorPrefab == null)
      {
        Debug.LogWarning("Elevator Prefab을 찾을 수 없습니다.");
        return;
      }
    }

    _elevatorPrefab = elevatorPrefab;

    // 맵 불러오기
    await GameManager.ResourceEx.LoadMapPrefabs(mapConceptName);

    StartCoroutine(SetupMap());
  }

  private IEnumerator SetupMap()
  {
    // GameScene에서 시작하기 위해 테스트용
    // yield return new WaitForSeconds(3f);

    // 1) 씬이 시작되면 맵 리소스 데이터 전체를 가져옴.
    _mapPrefabs = GameManager.ResourceEx.mapPrefabDict;
    // 2) 프리팹 초기화 
    _roomPrefabs = new List<GameObject>();
    _connectorPrefab = null;

    _mapRoot = new GameObject("Map").transform;

    // 3) 맵 리소스 데이터를 전체 순회 하면서 프리팹별로 나눔.
    foreach (var pair in _mapPrefabs)
    {
      string key = pair.Key;
      GameObject prefab = pair.Value;

      if (key.StartsWith(Constants.ROOM))
        _roomPrefabs.Add(prefab);
      else if (key.StartsWith(Constants.CONNECTOR))
        _connectorPrefab = prefab;
    }

    // 방 생성 함수 호출
    CreateFloor();

    // 플레이어 생성
    GameManager.PlayerSpawn.PlayerSpawn();

    // 엘리베이터 문이 열림.
    _startElevator.GetComponent<ElevatorController>().DoorsOpen(1f);

    yield return null;
  }

  // 방 초기화하고 생성하는 함수
  public void CreateFloor()
  {
    // 1) 현재 층이 무슨 컨셉의 층인지 가져움.
    var thisFloorConcept = GameManager.GameData.GetMapConceptForFloor();

    // 2) 시작 엘리베이터 생성 (플레이어 시작 위치)
    _startElevator = Instantiate(_elevatorPrefab, _mapRoot);
    _startElevator.transform.position = Vector3.zero;
    _startElevator.transform.rotation = Quaternion.identity;
    var startAnchor = _startElevator.GetComponent<ElevatorAnchor>();
    _attachPoint = startAnchor.elevatorAnchor;   // 출구를 기준으로 다음 연결 시작
    _startElevator.GetComponent<ElevatorController>().SetupForStart();

    // 3) 현재 레벨(층)이 보스인지 일반레벨인지 체크
    switch (thisFloorConcept)
    {
      case Constants.NORMAL_ROOM:
        NormalMapSetting();
        break;
      case Constants.BOSS_ROOM:
        BossMapSetting();
        break;
    }
  }

  // 일반적인 맵 생성
  private void NormalMapSetting()
  {
    // 1) 방 목록 섞기
    var candidates = new List<GameObject>(_roomPrefabs);
    var roomCount = GameManager.GameData.RoomCount();
    Shuffle(candidates);

    // 2) 방과 복도 생성
    int placed = 0;
    while (placed < roomCount)
    {
      if (placed > candidates.Count && !_isAllUsed)
      {
        _isAllUsed = true;
        Shuffle(candidates);
      }
        
      var nextPrefab = candidates[placed % candidates.Count];
      PlaceConnectorAndRoom(nextPrefab);
      placed++;
    }

    // 마지막 엘리베이터 앞 복도 배치
    var connector = Instantiate(_connectorPrefab, _mapRoot);
    var cn = connector.GetComponent<ConnectorAnchor>();
    AlignAtoB(connector.transform, cn.entryAnchor, _attachPoint);

    // 통로 마지막에 진행 포인트 갱신
    _attachPoint = cn.exitAnchor;

    // 3) 마지막 엘리베이터 생성 (다음 층으로 이동하는 출구)
    if (_elevatorPrefab != null && _attachPoint != null)
    {
      var endElevator = Instantiate(_elevatorPrefab, _mapRoot);
      endElevator.transform.Rotate(0f, 180f, 0f, Space.Self);      // 엘리베이터 프리팹 회전

      var ea = endElevator.GetComponent<ElevatorAnchor>();
      AlignAtoB(endElevator.transform, ea.elevatorAnchor, _attachPoint);
      endElevator.GetComponent<ElevatorController>().SetupForEnd();
    }
  }

  // 보스방이 있는 맵 생성
  private void BossMapSetting()
  {
    // 1) 복도 먼저 생성 (총 3개)
    for (int i = 0; i < 3; i++)
    {
      var connector = Instantiate(_connectorPrefab, _mapRoot);
      var cn = connector.GetComponent<ConnectorAnchor>();
      AlignAtoB(connector.transform, cn.entryAnchor, _attachPoint);

      // 통로 마지막에 진행 포인트 갱신
      _attachPoint = cn.exitAnchor;
    }

    // 2) 보스방 생성
    var room = Instantiate(_roomPrefabs[0], _mapRoot);
    var ra = room.GetComponent<RoomAnchor>();
    AlignAtoB(room.transform, ra.entryAnchor, _attachPoint);
  }

  // 복도와 방 배치하는 함수
  private void PlaceConnectorAndRoom(GameObject roomPrefab)
  {
    // a) 복도 배치
    var connector = Instantiate(_connectorPrefab, _mapRoot);
    var cn = connector.GetComponent<ConnectorAnchor>();
    AlignAtoB(connector.transform, cn.entryAnchor, _attachPoint);

    // b) 방 배치
    var room = Instantiate(roomPrefab, _mapRoot);
    var ra = room.GetComponent<RoomAnchor>();
    AlignAtoB(room.transform, ra.entryAnchor, cn.exitAnchor);

    // c) 진행 포인트 갱신
    _attachPoint = ra.exitAnchor;
  }

  // Anchor A가 속한 루트(rootToMove)를 움직여, Anchor A를 Anchor B에 정렬
  private void AlignAtoB(Transform rootToMove, Transform anchorToMove, Transform anchorTarget)
  {
    // 1) 회전 정렬
    // anchorToMove(예: 새 방의 입구)가 anchorTarget(예: 복도의 출구)을
    // 정확히 마주보도록(즉, 180도 반대 방향) 목표 회전값을 계산합니다.
    // (anchorTarget.forward의 반대 방향을 바라보도록 설정)
    Quaternion targetRotation = Quaternion.LookRotation(-anchorTarget.forward, anchorTarget.up);

    // rootToMove에 적용해야 할 '회전 차이값(delta)'을 계산합니다.
    // (목표 회전값 * 현재 회전값의 역)
    Quaternion rotationDelta = targetRotation * Quaternion.Inverse(anchorToMove.rotation);

    // rootToMove(맵/복도 루트)를 회전시킵니다.
    // (자식 객체인 anchorToMove도 따라서 회전합니다)
    rootToMove.rotation = rotationDelta * rootToMove.rotation;

    // 2) 위치 정렬
    
    // 이제 rootToMove가 올바른 방향을 바라보고 있으므로, 위치 오프셋을 계산합니다.
    // (이 부분은 기존 코드가 맞습니다)
    Vector3 positionOffset = anchorTarget.position - anchorToMove.position;

    // rootToMove 자체의 위치를 변경합니다.
    rootToMove.position += positionOffset;
  }

  // list에 있는 순서를 섞는 함수
  private void Shuffle<T>(IList<T> list)
  {
    for (int i = list.Count - 1; i > 0; i--)
    {
      int j = Random.Range(0, i + 1);
      (list[i], list[j]) = (list[j], list[i]);
    }
  }
}

