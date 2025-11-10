using System;
using System.Collections;
using System.Collections.Generic;
using _01.Scripts.PlayerControll;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MeleeAttack : MonoBehaviour, IEnemyAttack
{
    [Header("사거리/쿨타임/시야")]
    [SerializeField] private float _minRange = 0f;
    [SerializeField] private float _maxRange = 2.7f;
    [SerializeField] private float _coolDown = 1.0f;
    [SerializeField] private bool _requireLOS = false;

    [Header("히트박스 (애니로 on/off)")]    
    [SerializeField] private Transform hitOrigin; // 없으면 this

    [Header("필터/데미지")]
    public LayerMask targetMask;
    public string PlayerTag = "Player";
    [SerializeField] private float damage = 10f;
    [SerializeField] private Transform ownerRoot; // 루트(적) 기준점
    [SerializeField] private Collider hitbox;       // 애니로 on/off하는 그 콜라이더

    [Header("넉백")]
    [SerializeField] private bool enableKnockback = true;   // 넉백 사용 여부
    [SerializeField] private float knockbackImpulse = 8f;   // Rigidbody용
    [SerializeField] private float knockbackDistance = 2f;  // CharacterController용
    [SerializeField] private float knockbackDuration = 0.25f;   // 밀리는 시간
    [SerializeField] private float upwardModifier = 0f;     // 살짝 띄우고 싶을 때

    // 상태  
    private bool hasHit;
    private Rigidbody _rb;

    public AttackKind Kind => AttackKind.Melee;
    public float MinRange => _minRange;
    public float MaxRange => _maxRange;
    public bool RequireLOS => _requireLOS;
    public bool IsAttacking => hitbox && hitbox.enabled;
    private float _cdUntil; // Time.time 스탬프
    public bool IsOnCooldown => Time.time < _cdUntil;

    public void SetDamage(float value) => damage = value;
    public void SetCooldown(float seconds) => _cdUntil = Time.time + seconds;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (!hitbox)
        {
            hitbox = GetComponent<Collider>();
            if (hitbox) hitbox.isTrigger = true;
        }
        if (!hitOrigin) hitOrigin = transform;
        if (!ownerRoot) ownerRoot = transform.root; // 기본값: 내 루트
        if (hitbox) hitbox.isTrigger = true;        // 항상 트리거로 강제
        //자기 자신과는 충돌 무시
        if(hitbox && ownerRoot)
        {
            var ownCols = ownerRoot.GetComponentsInChildren<Collider>(true);
            foreach(var c in ownCols)
            {
                if (c && c != hitbox) Physics.IgnoreCollision(hitbox, c, true);
            }
        }
    }   

    public bool CanAttack(Transform target)
    {
        if (IsOnCooldown || !target) return false;
        float dist = Vector3.Distance(transform.position, target.position);
        return !(dist < MinRange || dist > MaxRange);
    }

    // Move가 근접을 선택했을 때 호출해 쿨타임/상태만 세팅
    public void StartSwing()
    {
        hasHit = false;      // 이번 스윙 아직 타격 없음
        _cdUntil = Time.time + _coolDown; // 시간 기반 쿨다운 시작
        // 애니메이션 트리거는 EnemyMove가 건다.
    }

    // 원거리와 인터페이스 호환을 위해 구현만 유지 (실제론 Move가 트리거를 건다)
    public void Attack(Transform target)
    {
        StartSwing();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[MeleeTrigger] ENTER name={other.name}, layer={other.gameObject.layer}, tag={other.tag}, IsAttacking={IsAttacking}, hasHit={hasHit}, mask={targetMask.value}");
        TryHit(other);
    }

    private static readonly Dictionary<PlayerController, Coroutine> _reEnableJobs
        = new Dictionary<PlayerController, Coroutine>();

    private void TempDisableController(PlayerController pc, float seconds)
    {
        if (!pc) return;

        if (_reEnableJobs.TryGetValue(pc, out var oldJob) && oldJob != null)
        {
            StopCoroutine(oldJob);
        }

        bool wasEnabled = pc.enabled;
        if (wasEnabled) pc.enabled = false; // 이미 꺼져있으면 그대로 둠

        float restoreAfter = Mathf.Max(0.01f, seconds) + 0.02f;
        var job = StartCoroutine(Co_ReEnableLater(pc, wasEnabled, restoreAfter));
        _reEnableJobs[pc] = job;
    }

    private IEnumerator Co_ReEnableLater(PlayerController pc, bool targetState, float delay)
    {
        float t = 0f;
        while (t < delay)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (pc) pc.enabled = targetState;
        _reEnableJobs.Remove(pc);
    }

    private void OnTriggerStay(Collider other)  => TryHit(other);

    private void TryHit(Collider other)
    {
        if (!IsAttacking)
        {
            Debug.Log("!IsAttacking doing wrong");
            return;
        }
        if (hasHit)
        {
            Debug.Log("hasHit doing wrong");
            return;
        }

        if (targetMask.value != 0 && (targetMask.value & (1 << other.gameObject.layer)) == 0)
        {
            Debug.Log("targetMask doing wrong");
            return;
        }
        // if (!other.CompareTag(PlayerTag)) return;

        var pc = other.GetComponentInParent<PlayerController>();
        if (pc == null)
        {
            Debug.Log($"Can't find PlayerController. now it's {pc}");
            return;
        }

        var damageable = other.GetComponentInParent<IDamageableZone>();
        if (damageable == null || damageable.IsDead) return;

        Vector3 hitPoint = other.ClosestPoint(hitOrigin ? hitOrigin.position : transform.position);
        pc.ApplyHit(damage, hitPoint, HitZones.Body);

        // 넉백 관련 부분
        if (enableKnockback)
        {            
            // 방향: 공격자(ownerRoot) → 피격자
            Vector3 origin = ownerRoot ? ownerRoot.position : transform.position;
            Vector3 rawDir = (pc.transform.position - origin);
            Vector3 dir = Vector3.ProjectOnPlane(rawDir, Vector3.up).normalized;

            if (upwardModifier != 0f)
                dir += Vector3.up * Mathf.Clamp(upwardModifier, -0.5f, 0.5f);
            
            // 컨트롤러를 '일시적으로' 꺼두고, 반드시 되돌리기 예약
            // CC 밀기 시간(knockbackDuration)에 맞추어 복구
            TempDisableController(pc, knockbackDuration);

            if (pc.TryGetComponent<CharacterController>(out var cc))
            {
                // CC를 우선 사용: FPS 캐릭터는 대개 CC 기반
                StartCoroutine(Co_PushCharacter(cc, dir.normalized, knockbackDistance, knockbackDuration));
            }
            else if (pc.TryGetComponent<Rigidbody>(out var rb) && !rb.isKinematic)
            {
                // RB가 있을 때만 임펄스 사용
                rb.AddForce(dir.normalized * knockbackImpulse, ForceMode.Impulse);
            }
            else
            {
                // 최후의 수단: 위치 가산
                pc.transform.position += dir.normalized * knockbackDistance;
            }

        }
        
        hasHit = true; // 스윙당 1회 (여러 대상 각각 1회면 HashSet으로 변경)
    }
    
    // CharacterController용 넉백
    private System.Collections.IEnumerator Co_PushCharacter(CharacterController cc, Vector3 dir, float distance, float duration)
    {
        float t = 0f;
        float speed = distance / Mathf.Max(0.001f, duration);

        while (t < duration)
        {
            cc.Move(dir * speed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        cc.Move(Vector3.down * 0.02f);
    }

    // 애니 이벤트 연결용 (선택)
    public void OnHitboxOpen()  { if (hitbox) hitbox.enabled = true;  }
    public void OnHitboxClose() { if (hitbox) hitbox.enabled = false; }
}
