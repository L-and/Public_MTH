using UnityEngine;

public class MeleeAttack : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private float _minRange = 0f;
    [SerializeField] private float _maxRange = 2.7f;
    [SerializeField] private float _coolDown = 1.0f;
    [SerializeField] private bool _requireLOS = false;

    [Header("애니메이션")]
    public Animator animator;
    public string attackTrigger = "Attack";

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

    private void End() => IsAttacking = false;

}