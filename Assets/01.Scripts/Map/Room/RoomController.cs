using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomController : MonoBehaviour
{
  [Header("방에 있는 문")]
  [SerializeField] private GameObject entryDoor;
  [SerializeField] private GameObject exitDoor;

  [Header("방에 있는 스포너 리스트")]
  [SerializeField] private List<EnemySpawner> Spawners;

  private int totalEnemysToSpawn = 0; // 이 방에 생성될 수 있는 총 몬스터 수
  private int killedEnemysCount = 0;  // 현재 죽은 몬스터 수
  private bool isCleared = false;     // 현재 방이 클리어 되었는지 체크

  void OnTriggerEnter(Collider other)
  {
    // 이미 클리어한 방인지, Trigger에 잡힌 Object가 Player 태그를 가지고 있는지 체크
    if (!isCleared && other.CompareTag("Player"))
    {
      // 문이 있으면 닫음.
      CloseAllDoor();

      // 한번 들어왔으니 더이상 방 안 Trigger는 필요 없으므로 비활성화
      GetComponent<BoxCollider>().enabled = false;

      // 몬스터 스포너 활성화 함수 호출
      ActivateSpawners();
    }
  }

  // 스포너 활성 및 총 몬스터 수 집계하는 함수
  private void ActivateSpawners()
  {
    // 스포너가 없거나 비어있는지 체크
    if (Spawners == null || Spawners.Count == 0)
      return;

    // 카운트 초기화
    totalEnemysToSpawn = 0;

    // 가지고 있는 스포너 순회
    foreach (EnemySpawner spawner in Spawners)
    {
      // 스포너 초기화 함수 호출
      spawner.Initialize(this);
      // 해당 스포너의 최대 스폰할 몬스터 개수 가져옴.
      totalEnemysToSpawn += spawner.GetMaxSpawnCount();

      spawner.TrySpawnEnemy();
    }
  }

  // 몬스터가 죽으면 해당 함수 호출 (EnemyDamage)
  public void NotifyEnemyDied()
  {
    // 이 방이 이미 클리어 되었으면 리턴
    if (isCleared)
      return;

    // 킬 카운터 증가
    killedEnemysCount++;
    Debug.Log($"[{gameObject.name}] 몬스터 처치. ({killedEnemysCount} / {totalEnemysToSpawn})");

    // 모든 몬스터 처치했는지 체크
    if (killedEnemysCount >= totalEnemysToSpawn)
    {
      OpenAllDoor();
    }
  }
  
  // 모든 문이 닫히는 함수
  private void CloseAllDoor()
  {
    if (entryDoor != null && exitDoor != null)
    {
      entryDoor.GetComponent<Door>().Close();
      exitDoor.GetComponent<Door>().Close();
    }
  }
  
  // 모든 문이 열리는 함수
  private void OpenAllDoor()
  {
    // 현재 방이 클리어 되었으니 true
    isCleared = true;

    entryDoor.GetComponent<Door>().Open();
    exitDoor.GetComponent<Door>().Open();

    Debug.Log($"[{gameObject.name}] 방 클리어! 문이 열립니다.");
  }
}
