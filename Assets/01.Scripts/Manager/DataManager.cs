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

  [Header("현재 주무기")]
  [SerializeField] private int _currentWeaponId = 0;

  [Header("현재 보조무기")]
  [SerializeField] private int _currentSideWeaponId = 0;

  [Header("현재 방출")]
  [SerializeField] private int _currentEmissionId = 0;

  // 현재 층 컨셉 체크 변수
  public string thisMapConcept;

  // 프로퍼티
  public int SewerMapMaxFloor() { return _sewerMapMaxFloor; }
  public int PrototypeMapMaxFloor() { return _prototypeMapMaxFloor; }
  public int RoomCount() { return _roomCount; } 
  public int BossFloor() { return _bossFloor; }
  public int CurrentWeaponID() { return _currentWeaponId; }
  public int CurrentSideWeaponID() { return _currentSideWeaponId; }
  public int CurrentEmissionID() { return _currentEmissionId; }
  public void SetCurrentWeaponID(int value) { _currentWeaponId = value; }
  public void SetCurrentSideWeaponID(int value) { _currentSideWeaponId = value; }
  public void SetCurrentEmissionID(int value) { _currentEmissionId = value; }
}