using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BossDie : MonoBehaviour
{
    [Header("VFX & SFX")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private AudioClip explosionSfx;
    [Range(0f,1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("컷신/연출")]
    [SerializeField] private float descendToY = 0f;
    [SerializeField] private float descendDuration = 3f;
    [SerializeField] private Vector3 camOffset = new Vector3(0f, 3.2f, -6.5f);
    [SerializeField] private float camApproachTime = 0.4f;      // 컷신 시작시 약간 다가가고
    [SerializeField] private float lingerAfterExplosion = 1.2f; // 폭발 후 잠시 정지
    [SerializeField] private bool restoreCameraAfter = true;
    [SerializeField] private bool destroyBossAfter = true;

    [Header("효과(선택)")]
    [SerializeField] private bool doShakeOnExplosion = true;
    [SerializeField] private float shakeDuration = 0.35f;
    [SerializeField] private float shakeStrength = 0.55f;
    [SerializeField] private int shakeVibrato = 18;
    [SerializeField] private float shakeRandomness = 90f;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeOutTime = 1f;

    [Header("기타")]
    [SerializeField] private GameObject heart;

    private Transform cam;
    private Vector3 prevCamPos;
    private Quaternion prevCamRot;
    private Sequence seq;

    void Start()
    {
        cam = Camera.main ? Camera.main.transform : null;
        if (cam == null)
        {
            Debug.LogWarning("[BossDie] MainCamera를 찾지 못했습니다. 메인 카메라 Tag 확인.");
            return;
        }

        // 플레이어 카메라 컨트롤 끄기(있다면) — 예: 마우스룩 스크립트
        var playerLook = cam.GetComponent<MonoBehaviour>();
        // 필요 시 여기서 비활성화 (예: playerLook.enabled = false;)

        prevCamPos = cam.position;
        prevCamRot = cam.rotation;

        // 컷신 시퀀스
        seq = DOTween.Sequence();

        // 1) 컷신 시작: 카메라를 보스 쪽으로 부드럽게 이동/조준
        Vector3 startTargetPos = transform.position + camOffset;
        seq.Append(cam.DOMove(startTargetPos, camApproachTime).SetEase(Ease.InOutSine));
        seq.Join(cam.DOLookAt(transform.position, camApproachTime));

        // 2) 보스가 내려가는 동안, 매 프레임 카메라를 보스 기준 camOffset만큼 **따라가게** 함
        var moveTween = transform.DOMoveY(descendToY, descendDuration).SetEase(Ease.Linear);
        moveTween.OnUpdate(() =>
        {
            Vector3 followPos = transform.position + camOffset;
            cam.position = followPos;
            cam.LookAt(transform.position);
        });

        seq.Append(moveTween);

        // 3) 도착 시점 폭발 + 사운드
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

            if (explosionSfx) AudioSource.PlayClipAtPoint(explosionSfx, transform.position, sfxVolume);
            if (doShakeOnExplosion && cam) cam.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness);
        });

        // 4) 잠깐 유지
        seq.AppendInterval(lingerAfterExplosion);

        // 5) 페이드 아웃(선택)
        if (fadeImage)
        {
            var c = fadeImage.color; c.a = 0f; fadeImage.color = c;
            seq.Append(fadeImage.DOFade(1f, fadeOutTime));
        }

        // 6) 마무리(카메라 복귀/보스 제거)
        seq.OnComplete(() =>
        {
            if (restoreCameraAfter && cam)
            {
                cam.DOMove(prevCamPos, 0.45f).SetEase(Ease.InOutSine);
                cam.DORotateQuaternion(prevCamRot, 0.45f);
            }
            if (destroyBossAfter) Destroy(gameObject);

            // 플레이어 카메라 컨트롤 다시 켜기(있다면)
            // if (playerLook) playerLook.enabled = true;
        });
    }

    void OnDestroy()
    {
        if (seq != null && seq.IsActive()) seq.Kill();
        if (cam != null) cam.DOKill();
        transform.DOKill();
        if (fadeImage) fadeImage.DOKill();
    }
}
