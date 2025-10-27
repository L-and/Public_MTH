using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
  private List<GameObject> _enemyPrefabs;

  private Transform _enemyRoot;

  void Start()
  {
    // 테스트용
    StartCoroutine(Init());
  }

  // 이 스크립트가 활성화 될 때
  private void OnEnable()
  {
    // EnemySpawnController.OnPlayerEnterRoom += EnemySpawn;
  }

  // 이 스크립트가 비활성화 될 때
  private void OnDisable()
  {

  }

  private void EnemySpawn()
  {
    
  }

  IEnumerator Init()
  {
    // 테스트용
    yield return new WaitForSeconds(3f);

    var _enemyPrefabsDict = GameManager.ResourceEx.enemyPrefabDict;

    _enemyPrefabs = new List<GameObject>();
    _enemyRoot = new GameObject("Enemy").transform;

    foreach (var pair in _enemyPrefabsDict)
      _enemyPrefabs.Add(pair.Value);
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
