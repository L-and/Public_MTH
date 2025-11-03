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
    public GameObject gameUI; // 0 
    public float fadeDuration = 1f; // 페이드 시간

    //private CameraBlurController blur;

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
        //if (aimLook != null)
        //{
        //    Debug.Log("AimLook 입력 잠금");
        //}


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

            // ✨ 회전 중 블러는 끄고 (혹시 남아있을 경우 대비) 0 
            //if (aimLook != null)
                //aimLook.UpdateBlurState(false, 0.3f);

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

            // 블러 시작 0
            //if (aimLook != null)
            //{
            //    aimLook.UpdateRotation(endRotTurn, endRotTurn);
            //}

            // 살짝 지연 후 블러 적용 0
            //yield return new WaitForSeconds(0.2f);
            //if (aimLook != null)
            //{
            //    Debug.Log("회전 완료 후 블러 적용 시작 ✨");
            //   aimLook.UpdateBlurState(true, 1f);
            //}

            // ✨ 블러가 다 적용될 때까지 잠시 대기 후 UI 등장
            //yield return new WaitForSeconds(1f);
            //Debug.Log("UI 작동 ✨");
            //upgradeUI.SetActive(true);

        }

        isMoving = false;
    }


    //private IEnumerator FadeOutGameUI()
    //{
    //    CanvasGroup gameCanvas = gameUI.GetComponent<CanvasGroup>();
    //    if (gameCanvas == null)
    //        gameCanvas = gameUI.AddComponent<CanvasGroup>();

    //    gameCanvas.interactable = false;
    //    gameCanvas.blocksRaycasts = false;

    //    float timer = 0f;
    //    float startAlpha = gameCanvas.alpha;

    //    while (timer < fadeDuration)
    //    {
    //        timer += Time.deltaTime;
    //        float t = timer / fadeDuration;
    //        gameCanvas.alpha = Mathf.Lerp(startAlpha, 0f, t);
    //        yield return null;
    //    }

    //    gameCanvas.alpha = 0f;
    //    gameUI.SetActive(false); // ✅ 완전히 투명해진 뒤 비활성화
    //    Debug.Log("✅ GameUI 페이드 아웃 완료 및 비활성화");
    //}


    //upgradeUI 페이드 인 함수 
    private IEnumerator FadeInUpgradeUI()
    {
        if (upgradeUI == null)
            yield break;

        // 업그레이드 활성화
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

 

    // 블러 해제 (UI 닫을 때 호출용) 0 
    //public IEnumerator FadeOutBlur(float duration)
    //{
    //    if (aimLook != null)
    //    {
    //        aimLook.UpdateBlurState(false, duration); // 블러 OFF ✨
    //    }

    //    Debug.Log("블러 해제 완료");
    //    yield return null;
    //}


    //// 블러 적용
    //private IEnumerator FadeInBlur(CameraBlurController blur, float duration)
    //{
    //    Debug.Log("블러 적용됨");
    //    float time = 0f;
    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        float t = Mathf.Clamp01(time / duration);
    //        blur.SetBlurIntensity(t);
    //        yield return null;
    //    }

    //    Debug.Log("ui작동");
    //    upgradeUI.SetActive(true);
    //}

    //// 블러 해제
    //public IEnumerator FadeOutBlur(CameraBlurController blur, float duration)
    //{
    //    float time = 0f;
    //    while (time < duration)
    //    {
    //        time += Time.deltaTime;
    //        float t = 1f - Mathf.Clamp01(time / duration);
    //        blur.SetBlurIntensity(t);
    //        yield return null;
    //    }

    //    blur.DisableBlur();
    //    Debug.Log("블러 해제 완료");
    //}
}
