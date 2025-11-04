using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ElevatorController : MonoBehaviour
{
  [Header("플레이어 시작지점")]
  [SerializeField] private GameObject _startPoint;

  [Header("문 막는 임시 벽 Object")]
  [SerializeField] private GameObject _doorBlocker;

  [Header("입구 상단 층 수 표시 Object")]
  [SerializeField] private GameObject _elevatorEntranceFloorText;

  [Header("안 측면 층 수 표시 Object")]
  [SerializeField] private GameObject _elevatorInsideFloorText;

  [Header("위로 올라가는 표시 MeshRenderer List")]
  [SerializeField] private List<MeshRenderer> arrowsUp = new List<MeshRenderer>();

  [Header("아래로 내려가는 표시 MeshRenderer List")]
  [SerializeField] private List<MeshRenderer> arrowDown = new List<MeshRenderer>();

  [Header("바깥 문 Animation")]
  [SerializeField] private Animation _outterDoorsAnim;

  [Header("안쪽 문 Animation")]
  [SerializeField] private Animation _innerDoorsAnim;

  [Header("문 열리는 Audio")]
  [SerializeField] private AudioSource _doorOpenAudio;

  [Header("문 닫히는 Audio")]
  [SerializeField] private AudioSource _doorCloseAudio;

  private bool hasBeenTriggered = false;  // Trigger 작동 했는지 체크하는 함수

  void Start()
  {
    var curFloor = GameManager.GameData.currentFloor;
    
    _elevatorEntranceFloorText.GetComponent<TextMeshProUGUI>().text = curFloor.ToString();
    _elevatorInsideFloorText.GetComponent<TextMeshProUGUI>().text = curFloor.ToString();

    SwitchArrows(false, false);
  }

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
      // TODO : 엘리베이터 안에 들어왔을때 발생하는 업그레이드 선택창 관련 추가

      // 화살표 내려가는 표시
      SwitchArrows(false, true);
      // Trigger 작동 했기 때문에 더이상 추가 작동되지 않게 하기 위해서 true
      hasBeenTriggered = true;
      Debug.Log("다음 층으로 내려갑니다.");
      // 실제 엘리베이터 문 닫히기 전에 플레이어가 나가지 않도록 투명 벽 활성화
      _doorBlocker.SetActive(true);
      // 엘리베이터 문이 닫히는 함수 호출
      StartCoroutine(DoorsOpenClose(_innerDoorsAnim, 1, -1, 0, _doorCloseAudio));
      StartCoroutine(DoorsOpenClose(_outterDoorsAnim, 1, -1, 0, _doorCloseAudio));
      // 현재 층 수 올라감.
      GameManager.GameData.currentFloor++;
      // 문이 다 닫히면 씬 로드 실행
      GameManager.SceneEx.LoadScene(Constants.GAMESCENE, false);
    }
  }

  // 화살표 표시 함수
  private void SwitchArrows(bool upValue, bool downValue)
  {
    for (int i = 0; i < arrowsUp.Count; i++)
    {
      arrowsUp[i].enabled = upValue;
      arrowDown[i].enabled = downValue;
    }
  }

  public void DoorsOpen(float delayTime)
  {
    StartCoroutine(DoorsOpenClose(_innerDoorsAnim, 0, 1, delayTime, _doorOpenAudio));
    StartCoroutine(DoorsOpenClose(_outterDoorsAnim, 0, 1, delayTime, _doorOpenAudio));
  }

  // 엘리베이터 문 닫히는 함수
  IEnumerator DoorsOpenClose(Animation anim, float initTime, float speed, float delay, AudioSource doorFX)
  {
    yield return new WaitForSeconds(delay);
    yield return null;

    anim[anim.clip.name].normalizedTime = initTime;
    anim[anim.clip.name].speed = speed;
    anim.Play();

    doorFX.Play();
  }
}
