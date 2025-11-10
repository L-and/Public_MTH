using UnityEngine;
using DG.Tweening;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class BossDie : MonoBehaviour
{
    [Header("이펙트 & 사운드")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private AudioClip explosionSfx;
    [Range(0f,1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("컷신 설정")]
    [SerializeField] private float descendToY = 0f;
    [SerializeField] private float descendDuration = 3f;
    [SerializeField] private Vector3 camOffset = new Vector3(0f, 3.2f, -6.5f);
    [SerializeField] private float camApproachTime = 0.4f;
    [SerializeField] private float lingerAfterExplosion = 1.2f;

    [Header("카메라 복귀 설정")]
    [SerializeField] private bool restoreCameraAfter = true;
    [SerializeField] private bool destroyBossAfter = true;
    [SerializeField] private MonoBehaviour[] cameraControllersToDisable;

    [Header("입력 관련 (Input System 사용 시)")]
#if ENABLE_INPUT_SYSTEM
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string cutsceneActionMap = "UI";
    [SerializeField] private string gameplayActionMap = "Player";
    private string prevActionMap;
#endif

    [Header("기타")]
    [SerializeField] private GameObject heart;

    private Transform cam;
    private Transform oldParent;
    private Vector3 oldLocalPos;
    private Quaternion oldLocalRot;
    private bool[] controllerPrevEnabled;
    private Transform rig;
    private Sequence seq;
    private CursorLockMode prevLock;
    private bool prevCursorVisible;

    void Start()
    {
        cam = Camera.main?.transform;
        if (!cam)
        {
            Debug.LogWarning("[BossDie] MainCamera를 찾지 못했습니다.");
            return;
        }

        // 카메라 상태 저장
        oldParent = cam.parent;
        oldLocalPos = cam.localPosition;
        oldLocalRot = cam.localRotation;

        // 커서 상태 저장 및 컷신용 변경
        prevLock = Cursor.lockState;
        prevCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 카메라 제어 스크립트 끄기
        if (cameraControllersToDisable != null)
        {
            controllerPrevEnabled = new bool[cameraControllersToDisable.Length];
            for (int i = 0; i < cameraControllersToDisable.Length; i++)
            {
                if (!cameraControllersToDisable[i]) continue;
                controllerPrevEnabled[i] = cameraControllersToDisable[i].enabled;
                cameraControllersToDisable[i].enabled = false;
            }
        }

        // 액션맵 전환
#if ENABLE_INPUT_SYSTEM
        if (playerInput)
        {
            prevActionMap = playerInput.currentActionMap?.name;
            if (!string.IsNullOrEmpty(cutsceneActionMap))
                playerInput.SwitchCurrentActionMap(cutsceneActionMap);
        }
#endif

        // 임시 리그 생성
        rig = new GameObject("BossDeathCamRig").transform;
        rig.position = transform.position + camOffset;
        cam.SetParent(rig, true);
        cam.DOLocalMove(Vector3.zero, camApproachTime).SetEase(Ease.InOutSine);
        cam.DOLookAt(transform.position, camApproachTime);

        // 보스 내려가기
        var moveTween = transform.DOMoveY(descendToY, descendDuration).SetEase(Ease.Linear);
        moveTween.OnUpdate(() =>
        {
            rig.position = transform.position + camOffset;
            cam.LookAt(transform.position);
        });

        seq = DOTween.Sequence();
        seq.Append(moveTween);

        // 폭발 이펙트 + 사운드
        seq.AppendCallback(() =>
        {
            if (heart) Destroy(heart);

            if (explosion)
            {
                var vfx = Instantiate(explosion, transform.position, Quaternion.identity);
                vfx.transform.localScale = Vector3.one * 4f;
                vfx.Play();
                var main = vfx.main;
                Destroy(vfx.gameObject, main.duration + main.startLifetime.constantMax);
            }

            if (explosionSfx)
                AudioSource.PlayClipAtPoint(explosionSfx, transform.position, sfxVolume);
        });

        // 잠시 대기 후 복귀
        seq.AppendInterval(lingerAfterExplosion);
        seq.OnComplete(ReturnCameraAndClean);
    }

    private void ReturnCameraAndClean()
    {
        // 트윈 정리
        if (cam) cam.DOKill();
        if (rig) rig.DOKill();

        // 카메라 복귀
        if (restoreCameraAfter && cam)
        {
            cam.SetParent(oldParent, false);
            cam.localPosition = oldLocalPos;
            cam.localRotation = oldLocalRot;
        }

        // 카메라 컨트롤러 복원
        if (cameraControllersToDisable != null && controllerPrevEnabled != null)
        {
            for (int i = 0; i < cameraControllersToDisable.Length; i++)
            {
                if (!cameraControllersToDisable[i]) continue;
                cameraControllersToDisable[i].enabled = controllerPrevEnabled[i];
            }
        }

        // PlayerInput 복귀
#if ENABLE_INPUT_SYSTEM
        if (playerInput)
        {
            var targetMap = !string.IsNullOrEmpty(prevActionMap) ? prevActionMap : gameplayActionMap;
            if (!string.IsNullOrEmpty(targetMap))
                playerInput.SwitchCurrentActionMap(targetMap);
            if (!playerInput.enabled) playerInput.enabled = true;
        }
#endif

        // 커서 잠금 상태 복구
        Cursor.lockState = prevLock;
        Cursor.visible = prevCursorVisible;

        // 리그 제거 / 보스 삭제
        if (rig) Destroy(rig.gameObject);
        if (destroyBossAfter) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (seq != null && seq.IsActive()) seq.Kill();
        if (cam) cam.DOKill();
        if (rig) rig.DOKill();
    }
}
