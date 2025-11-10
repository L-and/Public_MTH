using UnityEngine;

public interface IAttackTunable
{
    void SetGlobalCoolDownMul(float mul);
}

public class RangedAttack : MonoBehaviour, IEnemyAttack, IAttackTunable
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

    [Header("원거리 공격 애니메이션")]
    [SerializeField] private Animator animator;
    [SerializeField] private string shootTrigger;

    private float gloablCdMul = 1f; // 탱커형 적 분노버프용

    private float _cd; //쿨다운
    public AttackKind Kind => AttackKind.Ranged;

    public float MinRange => _minRange;

    public float MaxRange => _maxRange;

    public bool RequireLOS => _requireLOS;

    public bool IsAttacking { get; private set; }

    public bool IsOnCooldown => _cd > 0f;

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

        if (RequireLOS)
        {
            Vector3 origin = firePoint ? firePoint.position : transform.position + Vector3.up * 1.5f;
            Vector3 aim = target.position + Vector3.up * 1.5f;
            Vector3 dir = aim - origin;
            float len = dir.magnitude;
            if (len < 0.0001f) return false;

            dir /= len;

            // 막는 레이어만 대상으로 raycast. 뭔가 맞으면 가려진 것
            if (Physics.Raycast(origin, dir, len, obstacleMask))
            {
                return false;
            }
            // if (Physics.Raycast(origin, dir.normalized, out var hit, MaxRange, ~0))
            // {
            //     // 막는 레이어면 불가
            //     if (((1 << hit.collider.gameObject.layer) & obstacleMask) != 0)
            //     {
            //         return false;
            //     }
            // }
        }

        return true;
    }


    public void Attack(Transform target)
    {
        if (animator && !string.IsNullOrEmpty(shootTrigger))
            animator.SetTrigger(shootTrigger);

        if (!CanAttack(target))
        {
            return;
        }
        _cd = _coolDown * gloablCdMul;
        IsAttacking = true;

        Vector3 origin = firePoint ? firePoint.position : transform.position + Vector3.up * 1.5f;
        Vector3 dir = (target.position + Vector3.up * 1.2f) - origin;
        var go = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
        if (go.TryGetComponent<RangeProjectile>(out var proj))
        {
            Collider[] ignore = transform.root ? transform.root.GetComponentsInChildren<Collider>(true) : null;
            proj.Init(dir, transform, ignore, target);
        }

        Invoke(nameof(End), 0.2f);
    }

    private void End() => IsAttacking = false;

    public void SetGlobalCoolDownMul(float mul) => gloablCdMul = Mathf.Max(0.1f, mul);
}
