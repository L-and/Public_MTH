using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("공격 모션 길이 설정. 대략 0.7s")]
    public float attackTotalTime = 0.7f;
    [Range(0, 1)] public float activeStartNormalized = 0.25f;
    [Range(0, 1)] public float activeEndNormalized = 0.5f;

    [Header("Attack Shape")]
    public Transform attackOrigin;          // 보통 오른손 본
    public float sweepRadius = 0.25f;
    public float sweepPadding = 0.05f;

    [Header("Damage")]
    public float damage = 20f;
    public LayerMask targetMask;

    [Header("Cooldown")]
    public float cooldown = 0.9f;

    [Header("Animator")]
    public Animator animator;
    public string attackTrigger = "Attack";
    [Tooltip("공격 중 이동/회전 멈추기용 스크립트 (예: EnemyMove)")]
    public MonoBehaviour moverToPause;

    // ── 연속 공격 옵션 ─────────────────────────────────────────────
    [Header("Auto Chain (연속 공격)")]
    [Tooltip("사정거리 안에 있는 동안 자동으로 다음 공격을 잇는다.")]
    public bool autoRepeatWhileInRange = true;
    [Tooltip("다음 공격을 잇기 전 대기(초). 애니 간 미세한 텀)")]
    public float chainDelay = 0.05f;
    [Tooltip("체인 중에는 쿨다운을 무시하고 바로 잇기")]
    public bool ignoreCooldownWhileChaining = true;
    [Tooltip("체인 시 사정거리(EnemyMove.stopDistance보다 살짝 크게)")]
    public float chainRange = 2.8f;
    [Tooltip("체인 시 정면성(코사인). 0.6이면 약 53.1도")]
    [Range(0f,1f)] public float chainFacingDot = 0.5f;

    private bool isAttacking;
    private bool active;
    private float lastAttackTime = -999f;
    private Vector3 lastOriginPos;
    private readonly HashSet<int> hitOnce = new();

    // 연속공격 판단용 현재 타깃 (EnemyMove에서 넘겨줌)
    private Transform currentTarget;

    public bool IsAttacking => isAttacking;

    private void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // ── 외부 호출 ─────────────────────────────────────────────────
    public bool CanAttack(Transform target, float range)
    {
        if (!ignoreCooldownWhileChaining && Time.time < lastAttackTime + cooldown) return false;
        if (isAttacking) return false;
        if (target == null) return false;

        Vector3 a = transform.position; a.y = 0;
        Vector3 b = target.position; b.y = 0;
        return Vector3.Distance(a, b) <= range;
    }

    public void Attack() => Attack(currentTarget);

    public void Attack(Transform target)
    {
        // 체인 중이면 쿨다운 무시 옵션 처리
        if (!ignoreCooldownWhileChaining && Time.time < lastAttackTime + cooldown) return;
        if (isAttacking) return;

        currentTarget = target; // 체인용 저장

        isAttacking = true;
        lastAttackTime = Time.time;
        hitOnce.Clear();

        if (moverToPause != null)
            moverToPause.enabled = false;

        if (animator != null && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);
    }

    // ── 업데이트 ─────────────────────────────────────────────────
    private void Update()
    {
        if (!isAttacking || animator == null) return;

        var state = animator.GetCurrentAnimatorStateInfo(0);
        // 스테이트 이름/태그 커스텀 안 쓸 경우, 클립명 "Attack" 고정 사용
        bool inAttackState = state.IsName("Attack") || state.tagHash == Animator.StringToHash("Attack");
        if (!inAttackState) return;

        float t = state.normalizedTime % 1f;
        bool shouldActive = (t >= activeStartNormalized && t <= activeEndNormalized);

        if (shouldActive && !active) OnActiveStart();
        else if (!shouldActive && active) OnActiveEnd();

        if (active && attackOrigin != null)
        {
            Vector3 curr = attackOrigin.position;
            Vector3 dir = curr - lastOriginPos;
            float dist = dir.magnitude + sweepPadding;
            if (dist > 0f)
            {
                dir = (dist > 0.0001f) ? (dir / dist) : Vector3.forward;
                var hits = Physics.SphereCastAll(lastOriginPos, sweepRadius, dir, dist, targetMask, QueryTriggerInteraction.Collide);
                for (int i = 0; i < hits.Length; i++)
                    TryDamage(hits[i].collider, hits[i].point, -dir);
            }
            lastOriginPos = curr;
        }

        if (t >= 0.98f)
            EndAttack();
    }

    private void OnActiveStart()
    {
        active = true;
        if (attackOrigin != null) lastOriginPos = attackOrigin.position;
    }

    private void OnActiveEnd()
    {
        active = false;
    }

    private void EndAttack()
    {
        active = false;
        isAttacking = false;

        // 이동 재개는 "체인을 하지 않을 때"만
        bool willChain = autoRepeatWhileInRange && ShouldChainNext();
        if (moverToPause != null)
            moverToPause.enabled = !willChain;

        if (willChain)
        {
            // 쿨다운 무시하고 잇고 싶으면 타임스탬프 되돌리기
            if (ignoreCooldownWhileChaining)
                lastAttackTime = Time.time - cooldown;

            StartCoroutine(ChainNextAttack());
        }
    }

    private IEnumerator ChainNextAttack()
    {
        yield return new WaitForSeconds(chainDelay);
        if (ShouldChainNext())
            Attack(currentTarget); // 바로 다음 공격
    }

    private bool ShouldChainNext()
    {
        if (currentTarget == null) return false;

        // 거리
        Vector3 me = transform.position; me.y = 0;
        Vector3 tg = currentTarget.position; tg.y = 0;
        if (Vector3.Distance(me, tg) > chainRange) return false;

        // 정면성
        Vector3 dir = (tg - me).normalized;
        if (Vector3.Dot(transform.forward, dir) < chainFacingDot) return false;

        return true; // (시야/가림은 EnemyMove가 상태로 관리 중이니 여기선 단순 판정)
    }

    private void TryDamage(Collider col, Vector3 hitPoint, Vector3 dir)
    {
        if (col == null) return;
        int id = col.GetInstanceID();
        if (hitOnce.Contains(id)) return;
        hitOnce.Add(id);

        if (col.CompareTag("Player"))
        {
            col.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            var ps = col.GetComponentInParent<_01.Scripts.PlayerControll.Status.PlayerStat>();
            if (ps != null)
            {
                ps.AddOverHeat(5f);
                Debug.Log("Player got hit (melee). Add 5 Heat");
            }
            return;
        }

        // 적 끼리 공격하게 한다면 활성화. 지금은 플레이어만 공격받음
        // if(col.TryGetComponent<IDamageableZone>(out var dmg))
        // {
        //     dmg.ApplyHit(damage, hitPoint, HitZones.Body);
        // }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.3f);
        Gizmos.DrawWireSphere(attackOrigin.position, sweepRadius);
    }
}
