using _01.Scripts.PlayerControll;
using InfimaGames.LowPolyShooterPack;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class elevtest : MonoBehaviour
{
    [Header("플레이어 태그")]
    public string playerTag = "Player";

    [Header("플레이어가 이동할 목표 위치")]
    public Transform targetPosition; // Player elv position 

    [Header("플레이어가 바라볼 문 위치 (엘리베이터 문)")]
    public Transform doorFront; // elv door position

    [Header("이동 속도")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 3f;

    [Header("UI ＆ Blur 설정")]
    public GameObject upgradeUI;
    private CameraBlurController blur;
    
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
            StartCoroutine(MovePlayerToCenter(other.gameObject));
        }
    }

    private IEnumerator MovePlayerToCenter(GameObject player)
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
            //1초간 부드럽게 블러처리
            blur = mainCam.GetComponent<CameraBlurController>();
            if (blur == null)
                Debug.LogWarning("CameraBlurController를 Main Camera에서 찾지 못했습니다.");

            // 도착 후 180도로 플레이어 몸 회전
            Debug.Log("플레이어 회전");

            //현재 플레이어 회전 값 저장
            Quaternion startRotTurn = player.transform.rotation;
           
            // 문쪽으로 y축만 고정
            Vector3 lookDir = doorFront.position - player.transform.position;
            lookDir.y =0f;
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

            // 블러 시작
            if (blur != null)
                StartCoroutine(FadeInBlur(blur, 1f));
        }
        
        isMoving = false;
    }

    // 블러 적용
    private IEnumerator FadeInBlur(CameraBlurController blur, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            blur.SetBlurIntensity(t);
            yield return null;
        }

        Debug.Log("블러 적용됨");
    }

    // 블러 해제
    public IEnumerator FadeOutBlur(CameraBlurController blur, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = 1f - Mathf.Clamp01(time / duration);
            blur.SetBlurIntensity(t);
            yield return null;
        }

        blur.DisableBlur();
        Debug.Log("블러 해제 완료");
    }
}
