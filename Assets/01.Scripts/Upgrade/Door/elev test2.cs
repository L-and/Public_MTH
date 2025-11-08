using _01.Scripts.PlayerControll;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class elevtest2 : MonoBehaviour
{
    [Header("플레이어 태그")]
    public string playerTag = "Player";

    [Header("엘리베이터 이동경로")]
    public Transform enterPoint;       // player enter position
    public Transform targetPosition;   // player elv position
    public Transform doorFront;        // elv door position
    public Transform exitPoint;        // player exit position

    [Header("이동 속도 및 대기 시간")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 3f;
    public float waitBeforeOpen = 3f;  // 문 열기 전 대기 시간

    [Header("UI 설정")]
    public GameObject upgradeUI;
    public GameObject gameUI;
    public float fadeDuration = 1f;

    private bool isMoving = false;
    private bool controlLocked = true;

    private PlayerController playerController;
    private AimLook aimLook;
    private PlayerInput playerInput;
    private DoorManager door;

    private void Start()
    {
        door = FindAnyObjectByType<DoorManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isMoving)
        {
            StartCoroutine(ElevatorEnterSequence(other.gameObject));
        }
    }

    private IEnumerator ElevatorEnterSequence(GameObject player)
    {
        isMoving = true;

        // 조작 스크립트 가져오기 및 잠금
        playerController = player.GetComponent<PlayerController>();
        aimLook = player.GetComponentInChildren<AimLook>();
        playerInput = player.GetComponent<PlayerInput>();

        if (playerController != null) playerController.enabled = false;
        if (aimLook != null) aimLook.enabled = false;
        if (playerInput != null) playerInput.enabled = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("🚶 플레이어가 엘리베이터 구역 진입 — 문 앞 위치로 이동 시작");
        yield return StartCoroutine(MovePlayer(player, enterPoint.position));
        Debug.Log("📍 플레이어 문 앞 도착");

        // 3초 대기 후 문 열림
        yield return new WaitForSeconds(waitBeforeOpen);
        if (door != null)
        {
            door.OpenDoors();
            Debug.Log("🚪 엘리베이터 문 열림");
        }

        // 엘리베이터 내부로 이동
        yield return StartCoroutine(MovePlayer(player, targetPosition.position));
        Debug.Log("🎯 엘리베이터 내부 중심 도착");

        // 문 방향으로 회전
        //yield return StartCoroutine(RotatePlayerToDoor(player));
        //Debug.Log("🔄 문 방향으로 회전 완료");

        // 업그레이드 UI 표시
        //yield return StartCoroutine(FadeInUpgradeUI());
        //Debug.Log("✨ Upgrade UI 표시 완료");

        isMoving = false;
    }

    private IEnumerator MovePlayer(GameObject player, Vector3 target)
    {
        Vector3 start = player.transform.position;
        Vector3 end = new Vector3(target.x, start.y, target.z);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            player.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        player.transform.position = end;
    }

    private IEnumerator RotatePlayerToDoor(GameObject player)
    {
        if (doorFront == null) yield break;

        Vector3 lookDir = doorFront.position - player.transform.position;
        lookDir.y = 0f;
        Quaternion startRot = player.transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(lookDir);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotateSpeed;
            player.transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
    }

    private IEnumerator FadeInUpgradeUI()
    {
        if (upgradeUI == null) yield break;

        upgradeUI.SetActive(true);
        CanvasGroup canvasGroup = upgradeUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = upgradeUI.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
