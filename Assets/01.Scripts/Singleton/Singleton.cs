using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
  private static T _instance;

  public static T Instance
  {
    get
    {
      if (_instance == null)
      {
        Debug.LogWarning($"[Singleton] {typeof(T)} 의 인스턴스에 접근을 했지만 초기화 되지 않았거나 존재하지 않습니다.");
      }

      return _instance;
    }
  }

  // 중복 방지 및 초기화
  protected virtual void Awake()
  {
    if (_instance != null && _instance != this)
    {
      Debug.LogWarning($"[Singleton] {typeof(T)} 의 중복 인스턴스가 감지되었습니다. 중복된 인스턴스를 제거합니다.");
      Destroy(gameObject);
    }
    else
    {
      _instance = this as T;          // 유일한 인스턴스로 설ㅈ정
      DontDestroyOnLoad(gameObject);  // 씬 전환시 파괴되지 않도록 설정
    }
  }
}