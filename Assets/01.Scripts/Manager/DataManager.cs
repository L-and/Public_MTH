using UnityEngine;

public class DataManager : MonoBehaviour
{
  /////////////////////////////////////////////////////////////////
  /// 리소스 데이터를 제외한 Game 전반적으로 쓰는 Data를 관리하는 Manager
  /////////////////////////////////////////////////////////////////

  [Header("현재 층")]
  public int currentFloor = 0;

  [Header("모든 층 방 개수")]
  [SerializeField] private int _roomCount = 5;

  [Header("하수구 맵 나오는 최대 층")]
  [SerializeField] private int _sewerMapMaxFloor = 1;

  [Header("지하도시(미제작) 맵 나오는 최대 층")]
  [SerializeField] private int _prototypeMapMaxFloor = 2;

  [Header("(미제작) 맵 나오는 최대 층")]
  [SerializeField] private int _prototype2MapMaxFloor = 3; // 미사용

  [Header("보스(미제작)가 나오는 층")]
  [SerializeField] private int _bossFloor = 4;

  // 현재 층 컨셉 체크 변수
  public string thisMapConcept;

  // 프로퍼티
  public int SewerMapMaxFloor() { return _sewerMapMaxFloor; }
  public int PrototypeMapMaxFloor() { return _prototypeMapMaxFloor; }
  public int RoomCount() { return _roomCount; } 
  public int BossFloor() { return _bossFloor; }
}