using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ElevatorController : MonoBehaviour
{
  [Header("엘리베이터 내 StartPoint")]
  [SerializeField] private GameObject _startPoint;

  [Header("엘리베이터 문 막는 임시 벽 Object")]
  [SerializeField] private GameObject _doorBlocker;

  private bool hasBeenTriggered = false;  // Trigger 작동 했는지 체크하는 함수

  private const string GAMESCENE = "Game Scene";

  public GameObject StartPoint
  {
    get { return _startPoint; }
    private set { _startPoint = value; }
  }

  // 맵 처음 시작부분 엘리베이터 설정
  public void SetupForStart()
  {
    if (_startPoint != null) _startPoint.SetActive(true);

    var collider = GetComponent<BoxCollider>();

    if (collider != null) collider.enabled = false;
  }

  // 맵 끝에 있는 엘리베이터 설정
  public void SetupForEnd()
  {
    if (_startPoint != null) _startPoint.SetActive(false);

    var collider = GetComponent<BoxCollider>();

    if (collider != null) collider.enabled = true;
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!hasBeenTriggered && other.CompareTag("Player"))
    {
      // Trigger 작동 했기 때문에 더이상 추가 작동되지 않게 하기 위해서 true
      hasBeenTriggered = true;
      Debug.Log("다음 층으로 내려갑니다.");
      // 실제 엘리베이터 문 닫히기 전에 플레이어가 나가지 않도록 투명 벽 활성화
      _doorBlocker.SetActive(true);
      // 엘리베이터 문이 닫히는 함수 호출
      CloseElevatorDoor();
      // 문이 다 닫히면 씬 로드 실행
      GameManager.SceneEx.LoadScene(GAMESCENE, false);
      // 현재 층 수 올라감.
      GameManager.GameData.currentFloor++;
    }
  }

  // 엘리베이터 문 닫히는 함수
  private void CloseElevatorDoor()
  {

  }

  // 엘리베이터 문 열리는 함수
  public void OpenElevatorDoor()
  {

  }
}
