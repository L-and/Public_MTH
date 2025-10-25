using UnityEngine;

public class RangedAttack : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private float _minRange = 4f; // 원거리 공격 최소 사거리
    [SerializeField] private float _maxRange = 15;
    [SerializeField] private float _coolDown = 1.2f;
    [SerializeField] private bool _requireLOS = true;

    [Header("투사체")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private LayerMask obstacleMask = ~0;

    private float _cd; //쿨다운
    public AttackKind Kind => AttackKind.Melee;

    public float MinRange => _minRange;

    public float MaxRange => _maxRange;

    public bool RequireLOS => _requireLOS;

    public bool IsAttacking {get; private set;}

    public bool IsOnCooldown => _cd > 0f;

    private void Update()
    {
        if( _cd > 0f)
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

        if (RequireLOS)
        {
            Vector3 origin = firePoint ? firePoint.position : transform.position + Vector3.up * 1.5f;
            Vector3 dir = (target.position + Vector3.up * 1.5f) - origin;
            if (Physics.Raycast(origin, dir.normalized, out var hit, MaxRange, ~0))
            {
                // 막는 레이어면 불가
                if (((1 << hit.collider.gameObject.layer) & obstacleMask) != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }


    public void Attack(Transform target)
    {
        if (!CanAttack(target))
        {
            return;
        }
        _cd = _coolDown;

        Vector3 origin = firePoint ? firePoint.position : transform.position + Vector3.up * 1.5f;
        Vector3 dir = (target.position + Vector3.up * 1.2f) - origin;
        var go = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
        if (go.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = dir.normalized * projectileSpeed;
        }

        Invoke(nameof(End), 0.2f);
    }

    private void End() => IsAttacking = false;

}
