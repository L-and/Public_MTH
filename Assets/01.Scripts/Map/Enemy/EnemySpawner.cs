using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  [Header("Enemy PreFabs")]
  [SerializeField] private List<GameObject> enemyPrefabs;

  private List<GameObject> shuffleEnemyPrefabs;
  private int curEnemyIndex = 0;
  private EnemyController curSpawnEnemy;

  public bool isEnemyCleared { get; private set; }

  void Start()
  {
    isEnemyCleared = false;
  }

  void Update()
  {
    if (curEnemyIndex == enemyPrefabs.Count && !isEnemyCleared)
      isEnemyCleared = true;
  }

  public void SpawnEnemy()
  {
    if (enemyPrefabs == null || enemyPrefabs.Count == 0)
    {
      Debug.Log("스폰할 적이 리스트에 없습니다.");

      return;
    }

    shuffleEnemyPrefabs = new List<GameObject>(enemyPrefabs);
    Shuffle(shuffleEnemyPrefabs);

    curEnemyIndex = 0;
    SpawnNextEnemy();
  }

  private void SpawnNextEnemy()
  {
    // 1) 리스트에 스폰할 적이 남아있는지 확인
    if (curEnemyIndex < shuffleEnemyPrefabs.Count)
    {
      // 2) 스폰할 적 프리팹을 가져옴.
      GameObject prefabToSpawn = shuffleEnemyPrefabs[curEnemyIndex];

      // 3) 적을 스폰함.
      GameObject spawnObject = Instantiate(prefabToSpawn, transform.position + prefabToSpawn.transform.position, transform.rotation * Quaternion.Euler(0f, 180f, 0f));

      // 4) 스폰된 적의 EnemyCOntroller 스크립트를 가져옴.
      curSpawnEnemy = spawnObject.GetComponent<EnemyController>();

      // 5) 해당 프리팹이 사망했을 경우 알 수 있도록 이벤트 구독
      if (curSpawnEnemy != null)
        curSpawnEnemy.OnEnemyDied += OncurrentEnemyDied;
      else
        Debug.Log($"{prefabToSpawn.name} 프리팹에 EnemyController 스크립트가 없습니다!!!!!!");
    }
  }
  
  private void OncurrentEnemyDied()
  {
    // 1) 방금 죽은 적과의 이벤트 연결을 끊습니다.
    if (curSpawnEnemy != null)
      curSpawnEnemy.OnEnemyDied -= OncurrentEnemyDied;

    // 2) 다음 적을 스폰하기 위해 인덱스 1 올림.
    curEnemyIndex++;

    // 3) 다음 적 스폰을 위해 적 스폰 함수 호출
    SpawnNextEnemy();
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