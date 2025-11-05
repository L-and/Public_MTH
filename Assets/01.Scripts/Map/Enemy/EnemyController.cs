using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IInteractable
{

  // "나 죽었어!"라고 외칠 이벤트 (다른 스크립트가 구독할 수 있음)
  public static event Action OnEnemyDied;
  
  

  private bool isDead = false;

  private bool hasTriggered = false;

  private void OnTriggerEnter(Collider other)
  {
    if (!hasTriggered && other.CompareTag("Player"))
    {
      hasTriggered = true;

      
    }
  }

  public void Interact()
  {
    Debug.Log($"{this.name} 이 죽었습니다!");

    isDead = true;

    // 1) "나 죽는다!"라고 이벤트를 구독한 모든 스크립트(스포너 등)에게 알림
    OnEnemyDied?.Invoke();
  }
}