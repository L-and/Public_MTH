using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IInteractable
{
  [Header("Death Effect")]
  [SerializeField] private Material deadMaterial;
  [SerializeField] private float dsetroyDelay = 5f;

  // "나 죽었어!"라고 외칠 이벤트 (다른 스크립트가 구독할 수 있음)
  public event Action OnEnemyDied;

  private bool isDead = false;
  private Renderer renderer;

  void Awake()
  {
    renderer = GetComponent<Renderer>();
  }

  public void Interact()
  {
    Debug.Log($"{this.name} 이 죽었습니다!");

    isDead = true;

    // 1) "나 죽는다!"라고 이벤트를 구독한 모든 스크립트(스포너 등)에게 알림
    OnEnemyDied?.Invoke();

    // 2) 5초뒤 자신을 파괴하고 없애는 함수 호출
    StartCoroutine(DestroyEnemyObject());
  }

  private IEnumerator DestroyEnemyObject()
  {
    // 1) 죽은 적이 더이상 움직이지 않도록 함.
    var col = GetComponent<Collider>();

    if (col != null)
      col.enabled = false;

    // 2) 머테리얼 변경 (죽은 애니메이션 대체)
    if (deadMaterial != null)
      renderer.material = deadMaterial;

    // 3) destroyDelay 시간 만큼 기다림
    yield return new WaitForSeconds(dsetroyDelay);

    // 4) 오브젝트 파괴
    Destroy(gameObject);
  }
}