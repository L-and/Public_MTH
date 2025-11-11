using UnityEngine;
using DG.Tweening;

public class BossDie : MonoBehaviour
{
    [Header("사운드")]
    [SerializeField] private AudioClip mainExplosionSfx;
    [SerializeField] private AudioClip fallScreamSfx;     // 떨어질 때 비명
    [SerializeField] private AudioClip smallExplosionSfx; // 중간 폭발
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("이펙트 프리팹")]
    [SerializeField] private ParticleSystem mainExplosionFx;  // ★ 큰 폭발
    [SerializeField] private ParticleSystem smallExplosionFx; // ★ 작은 폭발

    [Header("연출 설정")]
    [SerializeField] private float descendToY = 0f;
    [SerializeField] private float descendDuration = 3f;
    [SerializeField] private Vector3 camOffset = new Vector3(0f, 3f, -6f);
    [SerializeField] private float camApproachTime = 0.4f;

    [Header("폭발 타이밍 (초 단위)")]
    [SerializeField] private float[] explosionMoments = { 0.3f, 0.7f, 1.2f, 1.8f };

    [Header("카메라-보스 선분 폭발 옵션 (옵션 A)")]
    [SerializeField, Tooltip("0=카메라, 1=보스. 0.15~0.3 추천")]
    private float camLerpT = 0.25f;
    [SerializeField] private Vector2 camJitterXY = new Vector2(1.2f, 0.8f);
    [SerializeField, Tooltip("보스 높이와 맞추고 싶으면 보정값")]
    private float yLockToBossAdd = 0.0f;

    [Header("기타")]
    [SerializeField] private GameObject heart;
    [SerializeField] private bool destroyAfterScene = true;

    private Transform cam;
    private Transform rig;
    private Sequence seq;

    void Start()
    {
        cam = Camera.main ? Camera.main.transform : null;
        if (!cam) { Debug.LogWarning("[BossDie] MainCamera 없음"); return; }

        // 임시 카메라 리그
        rig = new GameObject("BossDeathCamRig").transform;
        rig.position = transform.position + camOffset;
        cam.SetParent(rig, true);
        cam.DOLocalMove(Vector3.zero, camApproachTime).SetEase(Ease.InOutSine);
        cam.DOLookAt(transform.position, camApproachTime);

        if (fallScreamSfx)
            AudioSource.PlayClipAtPoint(fallScreamSfx, transform.position, sfxVolume);

        // 보스 하강 + 리그 추적
        var moveTween = transform.DOMoveY(descendToY, descendDuration).SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                if (!rig) return;
                rig.position = transform.position + camOffset;
                cam.LookAt(transform.position);
            });

        seq = DOTween.Sequence();
        seq.Append(moveTween);

        // 중간 폭발들 (카메라-보스 선분 위에서 터지게 변경)
        foreach (float t in explosionMoments)
        {
            seq.Insert(t, DOVirtual.DelayedCall(0f, () =>
            {
                var pos = ExplosionPosOnCamLine(); // ★ 여기서 위치 계산
                if (smallExplosionFx)
                {
                    var fx = Instantiate(smallExplosionFx, pos,
                        Quaternion.LookRotation(Camera.main.transform.forward));
                    fx.Play();
                    Destroy(fx.gameObject, fx.main.duration);
                }
                if (smallExplosionSfx)
                    AudioSource.PlayClipAtPoint(smallExplosionSfx, pos, sfxVolume * 0.8f);
            }));
        }

        // 마지막 큰 폭발
        seq.AppendCallback(() =>
        {
            if (heart) Destroy(heart);

            var pos = ExplosionPosOnCamLine(); // ★ 큰 폭발도 카메라-보스 선분
            if (mainExplosionFx)
            {
                var vfx = Instantiate(mainExplosionFx, pos,
                    Quaternion.LookRotation(Camera.main.transform.forward)); // ★ explosion → mainExplosionFx
                vfx.transform.localScale = Vector3.one * 4f;
                vfx.Play();
                Destroy(vfx.gameObject, vfx.main.duration);
            }
            if (mainExplosionSfx)
                AudioSource.PlayClipAtPoint(mainExplosionSfx, pos, sfxVolume); // ★ 사운드 위치도 pos
        });

        seq.OnComplete(() =>
        {
            if (destroyAfterScene) Destroy(gameObject);
            if (rig) Destroy(rig.gameObject);

            if (cam)
                cam.SetParent(null, true);
        });
    }

    // === 옵션 A: 카메라와 보스 사이 선분 위에서 폭발 위치 계산 ===
    private Vector3 ExplosionPosOnCamLine()
    {
        var mainCam = Camera.main;
        if (!mainCam) return transform.position;

        // 카메라 ↔ 보스 사이를 camLerpT 비율로 보간
        Vector3 basePos = Vector3.Lerp(mainCam.transform.position, transform.position, camLerpT);

        // 화면 안에서 좌우/상하 랜덤 오프셋
        basePos += mainCam.transform.right * Random.Range(-camJitterXY.x, camJitterXY.x);
        basePos += mainCam.transform.up    * Random.Range(-camJitterXY.y, camJitterXY.y);

        // 보스 높이와 맞추고 싶으면 보정
        if (!float.IsNaN(yLockToBossAdd))
            basePos.y = transform.position.y + yLockToBossAdd;

        // near clip에 너무 가까우면 밀어내기
        float minDist = mainCam.nearClipPlane + 0.3f;
        Vector3 dir = basePos - mainCam.transform.position;
        if (dir.sqrMagnitude < (minDist * minDist))
            basePos = mainCam.transform.position + dir.normalized * minDist;

        return basePos;
    }

    private void OnDestroy()
    {
        seq?.Kill();
    }
}
