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

    // 상태
    private float _cd;
    private bool hasHit;
    private Rigidbody _rb;

    public AttackKind Kind => AttackKind.Melee;
    public float MinRange => _minRange;
    public float MaxRange => _maxRange;
    public bool RequireLOS => _requireLOS;
    public bool IsOnCooldown => _cd > 0f;

    public bool IsAttacking => hitbox && hitbox.enabled;

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

    private void Update()
    {
        if (_cd > 0f) _cd -= Time.deltaTime;
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
        _cd = _coolDown;     // 쿨타임 시작
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
        if( pc == null)
        {
            Debug.Log($"Can't find PlayerController. now it's {pc}");
            return;
        }

        var damageable = other.GetComponentInParent<IDamageableZone>();
        if (damageable == null || damageable.IsDead) return;

        Vector3 hitPoint = other.ClosestPoint(hitOrigin ? hitOrigin.position : transform.position);
        pc.ApplyHit(damage, hitPoint, HitZones.Body);
        hasHit = true; // 스윙당 1회 (여러 대상 각각 1회면 HashSet으로 변경)
    }

    // 애니 이벤트 연결용 (선택)
    public void OnHitboxOpen()  { if (hitbox) hitbox.enabled = true;  }
    public void OnHitboxClose() { if (hitbox) hitbox.enabled = false; }
}
