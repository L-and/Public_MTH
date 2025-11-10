using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  [Header("스포너당 스폰한 몬스터 개수")]
  [SerializeField] private int maxSpawns = 3;

  private List<GameObject> _enemyPrefabs;  // 몬스터 프리팹 가지고 있는 리스트 변수
  private RoomController myRoom; // 내가 속한 방
  private GameObject myActiveEnemy; // 현재 생성된 몬스터

  private bool isWaitingForDeath = false; // 현재 스폰 기다려야하는지 체크
  private int spawnedCount = 0; // 현재까지 스폰된 몬스터 수

  public void Initialize(RoomController room)
  {
    _enemyPrefabs = GameManager.ResourceEx.enemyPrefabDict.Values.ToList();
    Shuffle(_enemyPrefabs);

    myRoom = room;
    spawnedCount = 0;
    isWaitingForDeath = false;
  }

  // 몬스터 스폰 시도하는 함수
  public void TrySpawnEnemy()
  {
    // 현재 스폰된 몬스터 수가 최대 몬스터 수 보다 작고 스폰을 기다리고 있는 중이 아니라면 실행
    if (spawnedCount < maxSpawns && !isWaitingForDeath)
    {
      // 스폰 기다리기 위해 true
      isWaitingForDeath = true;
      // 생성된 몬스터를 가져옴.
      myActiveEnemy = Instantiate(_enemyPrefabs[spawnedCount%_enemyPrefabs.Count], transform.position, transform.rotation);
      // 스폰 카운트 증가
      spawnedCount++;
      // 몬스터 사망 처리하는 스크립트를 가져옴.
      EnemyDamage enemyScript = myActiveEnemy.GetComponent<EnemyDamage>();

      if (enemyScript != null)
        // 스폰된 몬스터에게 스포너와 방 정보 넘기면서 초기화 함수 호출
        enemyScript.MySpawnerAndRoomInfo(this, myRoom);
    }
  }
  
  // 몬스터가 죽으면 해당 함수 호출 (EnemyDamage)
  public void NotifyEnemyDied(GameObject enemy)
  {
    // 내가 소환한 몬스터랑 동일한지 체크
    if(enemy == myActiveEnemy)
    {
      isWaitingForDeath = false;
      myActiveEnemy = null;

      TrySpawnEnemy(); 
    }
  }

  // maxSpawns 프로퍼티
  public int GetMaxSpawnCount()
  {
    return maxSpawns;
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