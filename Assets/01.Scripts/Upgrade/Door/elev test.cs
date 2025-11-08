using _01.Scripts.PlayerControll;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class elevtest : MonoBehaviour
{
    [Header("플레이어 태그")]
    public string playerTag = "Player";

    [Header("엘리베이터 이동경로")]
    public Transform targetPosition; // Player elv position 
    public Transform doorFront; // elv door position

    [Header("이동 속도")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 3f;

    [Header("UI 설정")]
    public GameObject upgradeUI;
    public GameObject gameUI; // 0 
    public float fadeDuration = 1f; // 페이드 시간

    private bool isMoving = false;
    private bool controlLocked = true; // 조작 잠금 유지

    private PlayerController playerController;
    private AimLook aimLook;
    private PlayerInput playerInput;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("현재 메인 카메라 이름: " + Camera.main.name);

        if (other.CompareTag(playerTag) && !isMoving)
        {
            StartCoroutine(ElevatorEnterSequence(other.gameObject));
        }
    }

    private IEnumerator ElevatorEnterSequence(GameObject player)
    {
        isMoving = true;

        // 조작 스트립트 가져오기
        playerController = player.GetComponent<PlayerController>();
        aimLook = player.GetComponentInChildren<AimLook>();
        playerInput = player.GetComponent<PlayerInput>();

        // 플레이어 기능 완전 차단 
        if (playerController != null) playerController.enabled = false;
        if (aimLook != null) aimLook.enabled = false;
        if (playerInput != null) playerInput.enabled = false;

        // 리지드바디 정지
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // ✅ GameUI 페이드 아웃 시작
        //if (gameUI != null)
        //{
        //    gameUI.SetActive(false);

        //    Debug.Log("✅ 엘리베이터 진입 시 GameUI 페이드 아웃 시작");
        //    //yield return StartCoroutine(FadeOutGameUI()); 이거는 옛날꺼
        //}

        // 자동 이동
        Debug.Log("플레이어 엘리베이터 진입 시작");
        Vector3 startPos = player.transform.position;
        Quaternion startRot = player.transform.rotation;
        Vector3 endPos = new Vector3(targetPosition.position.x, startPos.y, targetPosition.position.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            player.transform.position = Vector3.Lerp(startPos, endPos, t);
            player.transform.rotation = startRot;
            yield return null;
        }

        player.transform.position = endPos;
        Debug.Log("플레이어 엘리베이터 중심 도착");

        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            // 도착 후 180도로 플레이어 몸 회전
            Debug.Log("플레이어 회전");

            //현재 플레이어 회전 값 저장
            Quaternion startRotTurn = player.transform.rotation;

            // 문쪽으로 y축만 고정
            Vector3 lookDir = doorFront.position - player.transform.position;
            lookDir.y = 0f;
            Quaternion endRotTurn = Quaternion.LookRotation(lookDir);

            // 부드럽게 회전
            float rotT = 0f;
            while (rotT < 1f)
            {
                rotT += Time.deltaTime * rotateSpeed;
                player.transform.rotation = Quaternion.Slerp(startRotTurn, endRotTurn, rotT);
                mainCam.transform.rotation = Quaternion.Slerp(startRotTurn, endRotTurn, rotT);
                yield return null;
            }

            // player.transform.rotation = endRotTurn;
            // mainCam.transform.rotation = endRotTurn;
            Debug.Log("플레이어가 문 방향으로 180도 회전 완료");
            aimLook.UpdateRotation(endRotTurn, endRotTurn); // 완료 후 회전값을 aimLook에 적용

            yield return new WaitForSeconds(1f);
            Debug.Log("UI 페이드 전환 시작");
            yield return StartCoroutine(FadeInUpgradeUI());

        }

        isMoving = false;
    }

    private IEnumerator FadeInUpgradeUI()
    {
        if (upgradeUI == null)
            yield break;

        // 업그레이드 ui 버튼 활성화
        upgradeUI.SetActive(true);
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

        Debug.Log("✅ UpgradeUI 페이드 인 완료");
    }
}
