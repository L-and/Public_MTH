using _01.Scripts.PlayerControll.Status;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MeleeAttack : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private float _minRange = 0f;
    [SerializeField] private float _maxRange = 2.7f;
    [SerializeField] private float _coolDown = 1.0f;
    [SerializeField] private bool _requireLOS = false;

    [Header("애니메이션")]
    public Animator animator;
    public string attackTrigger = "Attack";

    [Header("공격 판정")]
    [Tooltip("현재 플레이어만 적용 중")]
    public LayerMask targetMask;
    public string PlayerTag = "Player";
    public float addHeatOnHit = 10f;

    private float _cd; //쿨다운
    public AttackKind Kind => AttackKind.Melee;

    public float MinRange => _minRange;

    public float MaxRange => _maxRange;

    public bool RequireLOS => _requireLOS;

    public bool IsAttacking { get; private set; }

    public bool IsOnCooldown => _cd > 0f;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    private void Update()
    {
        if (_cd > 0f)
        {
            _cd -= Time.deltaTime;
        }
    }

    public bool CanAttack(Transform target)
    {
        if (IsAttacking || IsOnCooldown || !target)
        {
            return false;
        }
        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < MinRange || dist > MaxRange)
        {
            return false;
        }
        return true;
    }

    public void Attack(Transform target)
    {
        if (!CanAttack(target))
        {
            return;
        }
        IsAttacking = true;
        _cd = _coolDown;
        // TODO: 애니메이션 트리거/히트 판정(코루틴 등)
        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
        {
            animator.SetTrigger(attackTrigger);
        }

        Invoke(nameof(End), 0.4f);
    }
    
    private bool hasHit = false;

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        if (!IsAttacking)
        {
            return;
        }
        if (hasHit)
        {
            return;
        }

        if (targetMask.value != 0 && (targetMask.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        if (!other.CompareTag(PlayerTag))
        {
            return;
        }

        var ps = other.GetComponentInParent<PlayerStatus>();
        if (ps == null)
        {
            return;
        }

        Debug.Log("Player got hit by Melee Attack");
        Debug.Log($"Player add {addHeatOnHit} Heat");
        ps.AddOverHeat(addHeatOnHit);

        hasHit = true;
    }    

    private void End()
    {
        IsAttacking = false;
        hasHit = false;
    } 
        
}