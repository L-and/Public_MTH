using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
  [Header("Room Pool")]
  [SerializeField] private List<GameObject> _roomPrefabs; // Room 프리팹 5개

  [Header("Connector / Elevator")]
  [SerializeField] private GameObject _connectorPrefab;   // Connector 프리팹
  [SerializeField] private GameObject _elevatorPrefab;    // Elevator 프리팹

  [Header("Options")]
  [SerializeField] private int _roomsPerFloor = 5;        // 한 층 방 개수
  [SerializeField] private Transform _floorRoot;          // 생성 부모
  [SerializeField] private int _thisFloor = 0;            // 현재 층

  private Transform _attachPoint; // 현재 진행 Anchor
  private int _floorCount = 0;    // 층 카운트

  void Start()
  {
    CreateFloor();
  }

  public void CreateFloor()
  {
    // 초기화
    if (_floorRoot == null)
    {
      _floorRoot = new GameObject("FloorRoot").transform;
      _floorRoot.position = Vector3.zero;
    }
    else
    {
      for (int i = _floorRoot.childCount - 1; i >= 0; i--)
        Destroy(_floorRoot.GetChild(i).gameObject);
    }

    // 방 목록 섞기
    var candidates = new List<GameObject>(_roomPrefabs);
    Shuffle(candidates);

    // 1) 시작 엘리베이터 생성 (플레이어 시작 위치)
    var startElevator = Instantiate(_elevatorPrefab, _floorRoot);
    startElevator.transform.position = Vector3.zero;
    startElevator.transform.rotation = Quaternion.identity;

    var startAnchor = startElevator.GetComponent<ElevatorAnchor>();
    _attachPoint = startAnchor.elevatorAnchor;   // 출구를 기준으로 다음 연결 시작

    // 2) 방과 복도 생성
    int placed = 0;
    while (placed < _roomsPerFloor)
    {
      var nextPrefab = candidates[placed % candidates.Count];
      PlaceConnectorAndRoom(nextPrefab);
      placed++;
    }

    // 엘리베이터 앞 복도 배치
    var connector = Instantiate(_connectorPrefab, _floorRoot);
    var cn = connector.GetComponent<ConnectorAnchor>();
    AlignAtoB(connector.transform, cn.entryAnchor, _attachPoint);

    // 통로 마지막에 진행 포인트 갱신
    _attachPoint = cn.exitAnchor;

    // 3) 마지막 엘리베이터 생성 (다음 층으로 이동하는 출구)
    if (_elevatorPrefab != null && _attachPoint != null)
    {
      var endElevator = Instantiate(_elevatorPrefab, _floorRoot);
      endElevator.transform.Rotate(0f, 180f, 0f, Space.Self);      // 엘리베이터 프리팹 회전
      var ea = endElevator.GetComponent<ElevatorAnchor>();
      AlignAtoB(endElevator.transform, ea.elevatorAnchor, _attachPoint);
    }
  }

  private void PlaceConnectorAndRoom(GameObject roomPrefab)
  {
    // a) 복도 배치
    var connector = Instantiate(_connectorPrefab, _floorRoot);
    var cn = connector.GetComponent<ConnectorAnchor>();
    AlignAtoB(connector.transform, cn.entryAnchor, _attachPoint);

    // b) 방 배치
    var room = Instantiate(roomPrefab, _floorRoot);
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
}

