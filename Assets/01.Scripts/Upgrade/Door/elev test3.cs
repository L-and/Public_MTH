using _01.Scripts.PlayerControll;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class elevtest3 : MonoBehaviour
{
  [Header("플레이어 태그")]
  public string playerTag = "Player";

  [Header("엘리베이터 이동경로")]
  public Transform targetPosition; // Player elv position 
  public Transform doorFront;      // elv door position

  [Header("이동 속도")]
  public float moveSpeed = 3f;
  public float rotateSpeed = 3f;

  [Header("UI 설정")]
  public GameObject upgradeUI;
  public GameObject gameUI;
  public float fadeDuration = 1f; // 페이드 시간

  private bool isMoving = false;
  private bool controlLocked = true;

  private PlayerController playerController;
  private AimLook aimLook;
  private PlayerInput playerInput;
  private GameObject currentPlayer;

  public static event Action OnUpgradeUIEnable;

  //private void OnTriggerEnter(Collider other)
  //{
  //    if (other.CompareTag(playerTag) && !isMoving)
  //    {
  //        StartCoroutine(ElevatorEnterSequence(other.gameObject));
  //    }
  //}

  public IEnumerator ElevatorEnterSequence(GameObject player)
  {
    isMoving = true;

    // 제어할 스크립트 가져오기
    playerController = player.GetComponent<PlayerController>();
    aimLook = player.GetComponentInChildren<AimLook>();
    playerInput = player.GetComponent<PlayerInput>();

    // 플레이어 기능 완전 차단
    if (playerController != null) playerController.enabled = false;
    if (aimLook != null) aimLook.enabled = false;
    if (playerInput != null) playerInput.enabled = false;

    //리지드바디 정지
    Rigidbody rb = player.GetComponent<Rigidbody>();
    if (rb != null)
    {
      rb.linearVelocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
    }

    // GameUI 페이드 아웃 시작
    //if (gameUI != null)
    //{
    //    gameUI.SetActive(false);

    //    Debug.Log("엘리베이터 진입 시 GameUI 페이드 아웃 시작");
    //    //yield return StartCoroutine(FadeOutGameUI()); 이거는 옛날꺼
    //}

    // 엘리베이터 내부로 이동
    Debug.Log("플레이어 엘리베이터 진입 시작");
    yield return StartCoroutine(MovePlayerSmooth(player, player.transform.position, targetPosition.position));
    Debug.Log("플레이어 엘리베이터 중심 도착");

    // 180회전 (문 방향)
    yield return StartCoroutine(RotatePlayerToDoor(player));

    // UI 표시
    yield return StartCoroutine(FadeInUpgradeUI());

    // 이쪽 쯤에 씬 전환 하면 좋을 듯
    Debug.Log("UI 선택 대기 중... (ElevatorUpgradeManager 신호 대기)");

    isMoving = false;
  }

  // 플레이어를 부드럽게 이동 -> 내부도착 
  private IEnumerator MovePlayerSmooth(GameObject player, Vector3 startPos, Vector3 endPos)
  {
    float t = 0f;

    while (t < 1f)
    {
      t += Time.deltaTime * moveSpeed;
      player.transform.position = Vector3.Lerp(startPos, endPos, t);
      yield return null;
    }
    player.transform.position = endPos;
  }

  // 문 방향으로 플레이어 회전시키는 코루틴
  private IEnumerator RotatePlayerToDoor(GameObject player)
  {
    // 도착후 180도로 문쪽을 향해 y축 회전
    Vector3 lookDir = doorFront.position - player.transform.position;
    lookDir.y = 0f;
    Quaternion startRot = player.transform.rotation;
    Quaternion endRot = Quaternion.LookRotation(lookDir);

    // 부드럽게 회전
    float rotT = 0f;
    while (rotT < 1f)
    {
      rotT += Time.deltaTime * rotateSpeed;
      player.transform.rotation = Quaternion.Slerp(startRot, endRot, rotT);
      yield return null;
    }

    // 엘리베이터 위치 도착
    player.transform.rotation = endRot;
  }

  //  업그레이드 UI 페이드인
  private IEnumerator FadeInUpgradeUI()
  {
    Debug.Log($"{upgradeUI}");

    if (upgradeUI == null)
      yield break;

    // UpgradeUI 활성화 이벤트 호출
    OnUpgradeUIEnable?.Invoke();

    //업그레이드 ui버튼 활성화
    // upgradeUI.SetActive(true);
    yield return null;

    CanvasGroup upgradeCanvas = upgradeUI.GetComponent<CanvasGroup>();
    if (upgradeCanvas == null)
      upgradeCanvas = upgradeUI.AddComponent<CanvasGroup>();

    upgradeCanvas.alpha = 0f;
    upgradeCanvas.interactable = false;
    upgradeCanvas.blocksRaycasts = false;

    float timer = 0f;
    while (timer < fadeDuration)
    {
      timer += Time.deltaTime;
      float t = timer / fadeDuration;
      upgradeCanvas.alpha = Mathf.Lerp(0f, 1f, t);
      yield return null;
    }

    upgradeCanvas.alpha = 1f;
    upgradeCanvas.interactable = true;
    upgradeCanvas.blocksRaycasts = true;

    Debug.Log("UpgradeUI 페이드 인 완료");
  }
}
