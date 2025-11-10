using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : MonoBehaviour
{
  /////////////////////////////////////////////
  /// Scene을 전반적으로 관리하는 Manager
  ////////////////////////////////////////////

  // 현재 씬이 로드중인지 확인하는 변수
  private bool _isLoading = false;

  /// <summary>
  /// 외부에서 Scene 로드를 위해 호출하는 함수
  /// </summary>
  /// <param name="sceneName">씬 이름 매개변수</param>
  public void LoadScene(string sceneName)
  {
    if (_isLoading)
    {
      Debug.LogWarning("이미 씬을 로드 중 입니다.");
      return;
    }

    StartCoroutine(LoadSceneAsync(sceneName));
  }
  
  // 실제 씬 로드를 하는 함수
  private IEnumerator LoadSceneAsync(string sceneName)
  {
    _isLoading = true;

    /// 로딩 UI 또는 페이드 아웃 시작

    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

    while (!asyncLoad.isDone)
    {
      /// 로딩 진행하는 동안 실행될 영역

      yield return null;  // 다음 프레임까지 대기
    }

    /// 로딩 UI 숨기기 또는 페이드 인 시작 부분
    
    _isLoading = false;
  }
}
