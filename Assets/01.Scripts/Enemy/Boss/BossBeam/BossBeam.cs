using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBeam : MonoBehaviour, IRangedAttacker
{
    [System.Serializable]
    public struct BeamPattern
    {
        [Header("Basics")]
        public string name;
        public Transform firePoint;      // 빔 시작 위치/방향
        public GameObject beamPrefab;    // 선택: 이펙트 프리팹(없어도 OK)
        public float windup;             // 시전 준비시간
        public float duration;           // 유지시간
        public float cooldown;           // 쿨다운

        [Header("Use Range")]
        public float useMinDistance;     // 사용 최소거리
        public float useMaxDistance;     // 사용 최대거리

        [Header("Hit Box (BoxCast)")]
        public float length;             // 빔 길이
        public float thickness;          // 빔 두께(정사각截面 한 변)
        public LayerMask hitMask;        // 피격 레이어

        [Header("Damage / Tick")]
        public float damagePerTick;      // 틱당 데미지
        public float tickInterval;       // 몇 초마다 1틱
        public bool continuousTick;      // true면 duration 내내 tickInterval마다 가동

        [Header("SFX (per hit)")]
        public AudioClip[] hitClips;     // 히트 순간 재생
        [Range(0f, 1f)] public float hitVolume;
    }

    [Header("Patterns (여러 개 가능)")]
    public BeamPattern[] patterns;

    [Header("General")]
    public bool faceTargetDuringBeam = true; // 유지 중에도 타깃을 향함
    public string damageSendMessage = "ApplyDamage"; // 대상 오브젝트에 SendMessage로 호출할 메서드명(없으면 무시)
    public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

    public bool IsRunning { get; private set; }

    // 내부 상태
    private float[] _nextReady;                        // 패턴별 다음 사용 가능 시점
    private Collider[] _ownerCols;                     // 자기(보스) 콜라이더(자체 피격 방지)
    private AudioSource _audio;                        // 히트 SFX
    private readonly Collider[] _overlapBuf = new Collider[48];
    private readonly Dictionary<Collider, float> _lastHitTime = new Dictionary<Collider, float>(64);

    void Awake()
    {
        if (patterns == null) patterns = new BeamPattern[0];
        _nextReady = new float[patterns.Length];

        _ownerCols = GetComponentsInParent<Collider>(includeInactive: true);

        _audio = GetComponent<AudioSource>();
        if (!_audio) _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
    }

    public bool CanUse(Transform target, float distance)
    {
        for (int i = 0; i < patterns.Length; i++)
        {
            var p = patterns[i];
            if (!p.firePoint) continue;
            if (Time.time < _nextReady[i]) continue;
            if (distance < p.useMinDistance || distance > p.useMaxDistance) continue;
            if (p.duration <= 0f || p.tickInterval <= 0f || p.length <= 0f || p.thickness <= 0f) continue;
            return true;
        }
        return false;
    }

    public void Execute(Transform target)
    {
        if (IsRunning) return;
        StartCoroutine(FireBeamRoutine(target));
    }

    private IEnumerator FireBeamRoutine(Transform target)
    {
        IsRunning = true;

        int sel = SelectUsablePattern(target);
        if (sel < 0) { IsRunning = false; yield break; }
        var p = patterns[sel];

        // 준비 시간
        if (p.windup > 0f) yield return new WaitForSeconds(p.windup);

        // 이펙트 생성(선택)
        GameObject beamFx = null;
        if (p.beamPrefab && p.firePoint)
        {
            beamFx = Instantiate(p.beamPrefab, p.firePoint.position, p.firePoint.rotation, p.firePoint);
            beamFx.SetActive(true);
        }

        float tEnd = Time.time + p.duration;
        float tickTimer = 0f;
        _lastHitTime.Clear();

        while (Time.time < tEnd)
        {
            // 조준 유지(선택)
            if (target && faceTargetDuringBeam)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
            }

            // 틱 진행
            tickTimer += Time.deltaTime;
            if (tickTimer >= p.tickInterval)
            {
                tickTimer = 0f;
                DoBeamTick(p);
                if (!p.continuousTick)
                {
                    // 단발형 틱일 경우 한 번만 적용
                    break;
                }
            }

            // 이펙트 위치/방향 추적
            if (beamFx && p.firePoint)
            {
                beamFx.transform.position = p.firePoint.position;
                beamFx.transform.rotation = p.firePoint.rotation;
            }

            yield return null;
        }

        if (beamFx) Destroy(beamFx);
        _nextReady[sel] = Time.time + p.cooldown;
        IsRunning = false;
    }

    private int SelectUsablePattern(Transform target)
    {
        float dist = target ? Vector3.Distance(transform.position, target.position) : Mathf.Infinity;
        for (int i = 0; i < patterns.Length; i++)
        {
            var p = patterns[i];
            if (!p.firePoint) continue;
            if (Time.time < _nextReady[i]) continue;
            if (dist < p.useMinDistance || dist > p.useMaxDistance) continue;
            if (p.duration <= 0f || p.tickInterval <= 0f || p.length <= 0f || p.thickness <= 0f) continue;
            return i;
        }
        return -1;
    }

    private void DoBeamTick(BeamPattern p)
    {
        if (!p.firePoint) return;

        // Box 중심: firePoint 기준 forward로 length * 0.5
        Vector3 center = p.firePoint.position + p.firePoint.forward * (p.length * 0.5f);
        Quaternion rot = p.firePoint.rotation;
        Vector3 halfExtents = new Vector3(p.thickness * 0.5f, p.thickness * 0.5f, p.length * 0.5f);

        int count = Physics.OverlapBoxNonAlloc(center, halfExtents, _overlapBuf, rot, p.hitMask, triggerInteraction);
        if (count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            var col = _overlapBuf[i];
            if (!col) continue;
            if (IsOwnerCollider(col)) continue;

            // 동일 프레임/틱 중복 방지(충분)
            if (_lastHitTime.TryGetValue(col, out float last) && Time.time - last < 0.001f)
                continue;
            _lastHitTime[col] = Time.time;

            // 데미지 전달
            ApplyDamage(col, p.damagePerTick);

            // 히트 SFX
            if (_audio && p.hitClips != null && p.hitClips.Length > 0)
            {
                var clip = p.hitClips[Random.Range(0, p.hitClips.Length)];
                if (clip) _audio.PlayOneShot(clip, p.hitVolume);
            }
        }
    }

    private bool IsOwnerCollider(Collider c)
    {
        if (_ownerCols == null) return false;
        for (int i = 0; i < _ownerCols.Length; i++)
            if (_ownerCols[i] == c) return true;
        return false;
    }

    private void ApplyDamage(Collider targetCol, float amount)
    {
        // 1) SendMessage(프로젝트마다 다를 수 있어 가장 안전)
        if (!string.IsNullOrEmpty(damageSendMessage))
            targetCol.gameObject.SendMessage(damageSendMessage, amount, SendMessageOptions.DontRequireReceiver);

        // 2) IDamageableZone 등이 있다면 여기서 직접 캐스팅하여 호출(네 프로젝트 규격에 맞게 바꿔도 됨)
        // var dz = targetCol.GetComponent<IDamageableZone>();
        // if (dz != null) dz.OnDamage(amount, ...);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (patterns == null) return;
        Gizmos.color = new Color(1f, 0.25f, 0.1f, 0.2f);
        foreach (var p in patterns)
        {
            if (!p.firePoint || p.length <= 0f || p.thickness <= 0f) continue;
            var center = p.firePoint.position + p.firePoint.forward * (p.length * 0.5f);
            var half = new Vector3(p.thickness * 0.5f, p.thickness * 0.5f, p.length * 0.5f);
            Matrix4x4 m = Matrix4x4.TRS(center, p.firePoint.rotation, Vector3.one);
            using (new UnityEditor.Handles.DrawingScope(m))
            {
                UnityEditor.Handles.DrawWireCube(Vector3.zero, half * 2f);
            }
        }
    }
#endif
}
