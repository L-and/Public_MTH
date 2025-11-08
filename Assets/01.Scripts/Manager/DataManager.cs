using System.Collections.Generic;
using UnityEngine;

public enum EMapConcept
{
  Map_Sewer,
  Map_UndergroundPrison,
  Map_Boss
}

// 맵 컨셉과 관련된 정보를 저장하는 구조체
[System.Serializable]
public struct FloorData
{
  [Tooltip("해당 컨셉이 시작되는 층")]
  public int startFloor;

  [Tooltip("해당 컨셉이 끝나는 층")]
  public int endFloor;

  [Tooltip("해당 층에 해당하는 컨셉의 맵")]
  public EMapConcept concept;
}

public class DataManager : MonoBehaviour
{
  /////////////////////////////////////////////////////////////////
  /// 리소스 데이터를 제외한 Game 전반적으로 쓰는 Data를 관리하는 Manager
  /////////////////////////////////////////////////////////////////

  [Header("현재 층")]
  public int currentFloor = 0;

  [Header("층별 컨셉 데이터 리스트")]
  [SerializeField] private List<FloorData> _floorDataList;

  [Header("모든 층 방 개수")]
  [SerializeField] private int _roomCount = 5;

  [Header("현재 주무기")]
  [SerializeField] private int _currentWeaponId = 0;

  [Header("현재 보조무기")]
  [SerializeField] private int _currentSideWeaponId = 0;

  [Header("현재 방출")]
  [SerializeField] private int _currentEmissionId = 0;


  // 프로퍼티
  public int RoomCount() { return _roomCount; }
  public int CurrentWeaponID() { return _currentWeaponId; }
  public int CurrentSideWeaponID() { return _currentSideWeaponId; }
  public int CurrentEmissionID() { return _currentEmissionId; }
  public void SetCurrentWeaponID(int value) { _currentWeaponId = value; }
  public void SetCurrentSideWeaponID(int value) { _currentSideWeaponId = value; }
  public void SetCurrentEmissionID(int value) { _currentEmissionId = value; }

  // 현재층에 해당하는 맵 컨셉 반환 함수
  public string GetMapConceptForFloor()
  {
    foreach (var data in _floorDataList)
    {
      if (currentFloor >= data.startFloor && currentFloor <= data.endFloor)
      {
        return data.concept.ToString(); // (string 방식이면 data.conceptName)
      }
    }

    Debug.LogWarning("현재층에 맞는 층 데이터가 없습니다. Prototype을 반환합니다.");
    return Constants.MAP_PROTOTYPE;
  }
}