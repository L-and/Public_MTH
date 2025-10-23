using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
  [Header("Room Pool")]
  [SerializeField] private List<GameObject> normalRoomPrefabs;  // NormalRoom 프리팹 5개
  [SerializeField] private GameObject boosRoomPrefab;           // BoosRoom 프리팹

  [Header("Connector / Elevator / Player")]
  [SerializeField] private GameObject connectorPrefab;    // Connector 프리팹
  [SerializeField] private GameObject elevatorPrefab;     // Elevator 프리팹
  [SerializeField] private GameObject playerPrefab;       // Player 프리팹

  [Header("Options")]
  [SerializeField] private int thisFloor = 0;             // 현재 층
  [SerializeField] private int boosFloor = 3;             // 보스 층
  [SerializeField] private int roomsPerFloor = 5;         // 한 층 방 개수

  private TextMeshProUGUI floorCountText; // 현재 층 표시하는 TextUI
  private Transform floorRoot;            // 생성 부모
  private Transform _attachPoint;         // 현재 진행 Anchor
  private int floorCount = 0;             // 층 카운트
  private bool isRetry = false;           // 리트라이인지 아닌지 체크
  private GameObject spawnPoint;          // 플레이어 스폰 포인트

  private const string BOSS_ROOM = "Boss Room";
  private const string NORMAL_ROOM = "Normal Room";

  private static MapManager instance;

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);

      SceneManager.sceneLoaded += OnSceneLoaded;
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    floorCountText = GameObject.FindWithTag("FloorCountTextUI").GetComponent<TextMeshProUGUI>();
    CreateFloor();
  }

  // 방 초기화하고 생성하는 함수
  public void CreateFloor()
  {
    if (!isRetry)
    {
      floorCount++;
      thisFloor = floorCount;
      floorCountText.text = $"{thisFloor} Floor";
    }

    // 초기화
    if (floorRoot == null)
    {
      floorRoot = new GameObject("FloorRoot").transform;
      floorRoot.position = Vector3.zero;
    }
    else
    {
      for (int i = floorRoot.childCount - 1; i >= 0; i--)
        Destroy(floorRoot.GetChild(i).gameObject);
    }

    // 1) 시작 엘리베이터 생성 (플레이어 시작 위치)
    var startElevator = Instantiate(elevatorPrefab, floorRoot);
    startElevator.transform.position = Vector3.zero;
    startElevator.transform.rotation = Quaternion.identity;

    var startAnchor = startElevator.GetComponent<ElevatorAnchor>();
    _attachPoint = startAnchor.elevatorAnchor;   // 출구를 기준으로 다음 연결 시작

    startElevator.GetComponent<ElevatorController>().SetupForStart();

    // 보스층인지 일반층인지 체크
    var floorCheck = floorCount % boosFloor;

    if (floorCheck == 0)
      BossMapSetting();
    else
      NormalMapSetting();

    PlayerSqawner();
  }

  // 일반적인 맵 생성
  private void NormalMapSetting()
  {
    // 방 목록 섞기
    var candidates = new List<GameObject>(normalRoomPrefabs);
    Shuffle(candidates);

    // 2) 방과 복도 생성
    int placed = 0;
    while (placed < roomsPerFloor)
    {
      var nextPrefab = candidates[placed % candidates.Count];
      PlaceConnectorAndRoom(nextPrefab);
      placed++;
    }

    // 엘리베이터 앞 복도 배치
    var connector = Instantiate(connectorPrefab, floorRoot);
    var cn = connector.GetComponent<ConnectorAnchor>();
    AlignAtoB(connector.transform, cn.entryAnchor, _attachPoint);

    // 통로 마지막에 진행 포인트 갱신
    _attachPoint = cn.exitAnchor;

    // 3) 마지막 엘리베이터 생성 (다음 층으로 이동하는 출구)
    if (elevatorPrefab != null && _attachPoint != null)
    {
      var endElevator = Instantiate(elevatorPrefab, floorRoot);
      endElevator.transform.Rotate(0f, 180f, 0f, Space.Self);      // 엘리베이터 프리팹 회전

      var ea = endElevator.GetComponent<ElevatorAnchor>();
      AlignAtoB(endElevator.transform, ea.elevatorAnchor, _attachPoint);

      endElevator.GetComponent<ElevatorController>().SetupForEnd();
    }
  }
  
  // 보스방이 있는 맵 생성
  private void BossMapSetting()
  {
    // 2) 복도복도 보스방 복도복도 배치
    for (int i = 0; i < 2; i++)
    {
      var connector = Instantiate(connectorPrefab, floorRoot);
      var cn = connector.GetComponent<ConnectorAnchor>();
      AlignAtoB(connector.transform, cn.entryAnchor, _attachPoint);

      // 통로 마지막에 진행 포인트 갱신
      _attachPoint = cn.exitAnchor;
    }

    var room = Instantiate(boosRoomPrefab, floorRoot);
    var roomController = room.GetComponent<RoomController>();
    roomController.roomType = BOSS_ROOM;
    var ra = room.GetComponent<RoomAnchor>();
    AlignAtoB(room.transform, ra.entryAnchor, _attachPoint);

    _attachPoint = ra.exitAnchor;

    for (int i = 0; i < 2; i++)
    {
      var connector = Instantiate(connectorPrefab, floorRoot);
      var cn = connector.GetComponent<ConnectorAnchor>();
      AlignAtoB(connector.transform, cn.entryAnchor, _attachPoint);

      // 통로 마지막에 진행 포인트 갱신
      _attachPoint = cn.exitAnchor;
    }

    // 3) 마지막 엘리베이터 생성 (다음 층으로 이동하는 출구)
    if (elevatorPrefab != null && _attachPoint != null)
    {
      var endElevator = Instantiate(elevatorPrefab, floorRoot);
      endElevator.transform.Rotate(0f, 180f, 0f, Space.Self);      // 엘리베이터 프리팹 회전

      var ea = endElevator.GetComponent<ElevatorAnchor>();
      AlignAtoB(endElevator.transform, ea.elevatorAnchor, _attachPoint);

      endElevator.GetComponent<ElevatorController>().SetupForEnd();
    }
  }

  private void PlaceConnectorAndRoom(GameObject roomPrefab)
  {
    // a) 복도 배치
    var connector = Instantiate(connectorPrefab, floorRoot);
    var cn = connector.GetComponent<ConnectorAnchor>();
    AlignAtoB(connector.transform, cn.entryAnchor, _attachPoint);

    // b) 방 배치
    var room = Instantiate(roomPrefab, floorRoot);
    var roomController = room.GetComponent<RoomController>();
    roomController.roomType = NORMAL_ROOM;
    var ra = room.GetComponent<RoomAnchor>();
    AlignAtoB(room.transform, ra.entryAnchor, cn.exitAnchor);

    // c) 진행 포인트 갱신
    _attachPoint = ra.exitAnchor;
  }

  // Anchor A가 속한 루트(rootToMove)를 움직여, Anchor A를 Anchor B에 정렬
  private void AlignAtoB(Transform rootToMove, Transform anchorToMove, Transform anchorTarget)
  {
    // 위치 정렬
    // anchorToMove의 월드 좌표를 기준으로 위치 오프셋 계산
    Vector3 positionOffset = anchorTarget.position - anchorToMove.position;

    // rootToMove 자체의 위치를 변경
    rootToMove.position += positionOffset;
  }

  private void Shuffle<T>(IList<T> list)
  {
    for (int i = list.Count - 1; i > 0; i--)
    {
      int j = Random.Range(0, i + 1);
      (list[i], list[j]) = (list[j], list[i]);
    }
  }

  // 플레이어 생성
  private void PlayerSqawner()
  {
    spawnPoint = GameObject.FindWithTag("PlayerSpawnPoint");

    if (spawnPoint != null)
      Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
    else
      Debug.Log("Player가 Spawn할 SpawnPoint가 없습니다.");
  }

  private void OnDestroy()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }
}

