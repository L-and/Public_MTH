using System.Collections;
using UnityEngine;

public abstract class Door : MonoBehaviour
{
  [Header("문 열리는 속도")]
  [SerializeField] protected float doorMoveSpeed;  // 문 열리는 속도

  // 문이 열리는 기능
  public abstract void Open();
  // 문이 닫히는 기능
  public abstract void Close();
}
