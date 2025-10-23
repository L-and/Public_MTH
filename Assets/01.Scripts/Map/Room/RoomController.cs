using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RoomController : MonoBehaviour
{
  [SerializeField] private DoorController entryDoor;
  [SerializeField] private DoorController exitDoor;

  // 프리팹 원본 배열
  [SerializeField] private List<EnemySpawner> enemySpawnerPrefabs;

  // 씬에 생성된 스포너들 (Clone)
  private List<EnemySpawner> enemySpawners;

  private bool hasBeenTriggered = false;  // 이 방이 활성화 되었는지 확인
  private bool isRoomClear = false;
  public bool isAllDoorOpen { get; private set; } // 방이 클리어 되었는지 체크
  public string roomType = null;
  private const string BOSS_ROOM = "Boss Room";
  private const string NORMAL_ROOM = "Normal Room";

  void Start()
  {
    isAllDoorOpen = false;

    enemySpawners = new List<EnemySpawner>();

    if (roomType == NORMAL_ROOM)
    {
      var index = 0;

      foreach (EnemySpawner spawner in enemySpawnerPrefabs)
      {
        EnemySpawner newEnemySpawner = Instantiate(spawner, transform.position + new Vector3(-7 + (7 * index), 0, 7), transform.rotation, this.transform);

        enemySpawners.Add(newEnemySpawner);
        index++;
      }
    }
    else if(roomType == BOSS_ROOM)
    {
        EnemySpawner newEnemySpawner = Instantiate(enemySpawnerPrefabs[0], transform.position + new Vector3(0, 0, 7), transform.rotation, this.transform);

        enemySpawners.Add(newEnemySpawner);      
    }
  }

  void Update()
  {
    RoomClearCheck();
  }

  void OnTriggerEnter(Collider other)
  {
    // 이미 활성화 됐거나, Player가 아닐 경우 무시
    if (hasBeenTriggered || !other.CompareTag("Player"))
    {
      return;
    }

    // 1) 방 활성화
    hasBeenTriggered = true;

    // 2) 들어온 문 닫기
    entryDoor.CloseDoor();

    // 3) 적 스폰 시작
    EnemySpawn();

    // 4) 더 이상 트리거 발생하지 않도록 BoxCollider 비활성
    this.GetComponent<BoxCollider>().enabled = false;
  }

  private void EnemySpawn()
  {
    if (enemySpawners != null)
    {
      foreach (EnemySpawner spawner in enemySpawners)
      {
        if (spawner != null)
          spawner.SpawnEnemy();
      }
    }
  }
  
  private void RoomClearCheck()
  {
    if (enemySpawners != null)
    {
      if (!isAllDoorOpen)
      {
        var spawnerClearCount = 0;

        foreach (EnemySpawner spawner in enemySpawners)
        {
          if (spawner.isEnemyCleared)
            spawnerClearCount++;
        }

        if (spawnerClearCount == enemySpawners.Count && !isRoomClear)
          isRoomClear = true;

        if (isRoomClear)
          OpenAllDoor();
      }
    }
  }

  private void OpenAllDoor()
  {
    isAllDoorOpen = true;

    entryDoor.OpenDoor();
    exitDoor.OpenDoor();
  }
}
