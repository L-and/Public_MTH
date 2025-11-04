using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    public enum Tactic { Default, Kite }

    [Header("타겟")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("전술")]
    [SerializeField] private Tactic tactic = Tactic.Default;

    [Header("카이팅 옵션")]
    [SerializeField] private float kiteRetreatSpeedMul = 1.0f;
    [SerializeField] private float kitePadding = 0.8f;
    [SerializeField] private float kiteStep = 3.0f;

    [Header("추적 / 시야")]
    [SerializeField] private float sightRange = 20f;
    [SerializeField] private float fovAngle = 80f;
    [SerializeField] private float stopDistance = 2.5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("배회")]
    [SerializeField] private bool useRoam = true;
    [SerializeField] private float roamRadius = 12f;
    [SerializeField] private Transform roamCenterOverride;
    [SerializeField] private float roamWaitMin = 0.8f;
    [SerializeField] private float roamWaitMax = 1.6f;

    [Header("길찾기")]
    [SerializeField] private float repathInterval = 0.15f;

    [Header("애니메이션")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParam = "moveSpeed";
    [SerializeField] private float speedDamp = 0.1f;
    [SerializeField] private string meleeTrigger = "Attack"; // ← 근접용 트리거 이름

    [Header("공격")]
    [SerializeField] private MonoBehaviour[] attackBehaviours; // IEnemyAttack들
    private IEnemyAttack[] _attacks;

    [Header("공격 공통 옵션")]
    [SerializeField, Range(0f, 1f)] private float attackFacingDot = 0.4f;

    [Header("기타 링크")]
    [SerializeField] private EnemyDamage enemyDamage;

    private NavMeshAgent agent;
    private int speedHash;
    private float repathTimer;
    private Vector3 roamCenter;
    private float roamWaitTimer;
    private float roamWaitTarget;
    private IEnemyAttack _lastBest;

    private enum State { Roam, Chase, Stop }
    private State state = State.Roam;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!enemyDamage) enemyDamage = GetComponent<EnemyDamage>();

        if (attackBehaviours != null && attackBehaviours.Length > 0)
        {
            _attacks = new IEnemyAttack[attackBehaviours.Length];
            for (int i = 0; i < attackBehaviours.Length; i++)
                _attacks[i] = attackBehaviours[i] as IEnemyAttack;
        }
        else
        {
            _attacks = System.Array.Empty<IEnemyAttack>();
        }

        if (!target)
        {
            var p = GameObject.FindGameObjectWithTag(playerTag);
            if (p) target = p.transform;
        }
        speedHash = Animator.StringToHash(speedParam);
    }

    private void OnEnable() => EnsureOnNavMesh();

    private void Start()
    {
        if (agent) agent.stoppingDistance = stopDistance;

        roamCenter = (roamCenterOverride ? roamCenterOverride.position : transform.position);
        state = useRoam ? State.Roam : State.Stop;

        if (state == State.Roam && agent && agent.isActiveAndEnabled && EnsureOnNavMesh())
            agent.SetDestination(RandomPointOnNavmesh(roamCenter, roamRadius));
    }

    private void Update()
    {
        if (enemyDamage && enemyDamage.IsDead) { SafeStopAgent(); SetSpeed(0f); return; }

        if (!agent || !agent.isActiveAndEnabled) { SetSpeed(0f); return; }
        if (!agent.isOnNavMesh && !EnsureOnNavMesh()) { SetSpeed(0f); return; }

        bool canSee = CanSeeTarget();
        float dist = DistanceToTarget();
        var best = SelectBestAttack(canSee, dist);
        _lastBest = best ?? _lastBest;

        switch (state)
        {
            case State.Roam:
                if (!useRoam) { state = State.Stop; break; }
                if (canSee) state = (dist > stopDistance) ? State.Chase : State.Stop;
                break;

            case State.Chase:
                if (TryExecute(best)) { state = State.Stop; break; }
                if (!canSee) state = useRoam ? State.Roam : State.Stop;
                else if (dist <= stopDistance) state = State.Stop;
                break;

            case State.Stop:
                if (!TryExecute(best))
                {
                    if (canSee) state = (dist > stopDistance) ? State.Chase : State.Stop;
                    else state = useRoam ? State.Roam : State.Stop;
                }
                break;
        }

        switch (state)
        {
            case State.Roam:
                RoamTick();
                SetSpeed(agent.velocity.magnitude);
                break;

            case State.Chase:
                if (tactic == Tactic.Kite && _lastBest != null && target)
                {
                    KiteMove(dist, _lastBest);
                    SetSpeed(agent.velocity.magnitude);
                    if (TryExecute(best)) state = State.Stop;
                }
                else
                {
                    repathTimer -= Time.deltaTime;
                    if (repathTimer <= 0f && target)
                    {
                        agent.isStopped = false;
                        agent.SetDestination(target.position);
                        repathTimer = repathInterval;
                    }
                    SetSpeed(agent.velocity.magnitude);
                }
                break;

            case State.Stop:
                if (TryExecute(best))
                {
                    SafeStopAgent();
                    if (target) FaceTarget(target.position);
                    SetSpeed(0f);
                }
                break;
        }
    }

    private void KiteMove(float dist, IEnemyAttack atk)
    {
        if (!agent || !target) return;

        float min = atk.MinRange + kitePadding;
        float max = atk.MaxRange - kitePadding;

        if (dist < min)
        {
            Vector3 away = (transform.position - target.position).normalized;
            Vector3 goal = transform.position + away * Mathf.Max(kiteStep, (min - dist) * 0.6f);
            if (NavMesh.SamplePosition(goal, out var hit, 2.0f, agent.areaMask))
            {
                agent.speed *= kiteRetreatSpeedMul;
                agent.isStopped = false;
                agent.SetDestination(hit.position);
            }
        }
        else if (dist > max)
        {
            Vector3 to = (target.position - transform.position).normalized;
            Vector3 goal = target.position - to * ((min + max) * 0.5f);
            if (NavMesh.SamplePosition(goal, out var hit, 2.0f, agent.areaMask))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            SafeStopAgent();
            FaceTarget(target.position);
        }
    }

    private bool CanSeeTarget()
    {
        if (!target) return false;
        Vector3 to = target.position - transform.position;
        if (to.sqrMagnitude > sightRange * sightRange) return false;

        Vector3 flat = new Vector3(to.x, 0, to.z);
        if (Vector3.Angle(transform.forward, flat) > fovAngle) return false;

        Vector3 eye = transform.position + Vector3.up * 1.6f;
        if (Physics.Raycast(eye, to.normalized, out var hit, sightRange, ~0))
            if ((obstacleMask.value & (1 << hit.collider.gameObject.layer)) != 0) return false;

        return true;
    }

    private void RoamTick()
    {
        if (!useRoam) { SafeStopAgent(); return; }
        if (!agent || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;
        if (agent.pathPending) return;

        if (agent.remainingDistance != Mathf.Infinity &&
            agent.remainingDistance <= Mathf.Max(0.2f, agent.stoppingDistance + 0.05f))
        {
            if (!agent.isStopped) agent.isStopped = true;
            roamWaitTimer += Time.deltaTime;
            if (roamWaitTimer >= roamWaitTarget)
            {
                var next = RandomPointOnNavmesh(roamCenter, roamRadius);
                agent.isStopped = false;
                agent.SetDestination(next);
                roamWaitTimer = 0f;
                roamWaitTarget = Random.Range(roamWaitMin, roamWaitMax);
            }
        }
        else
        {
            if (agent.isStopped) agent.isStopped = false;
            if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
                agent.SetDestination(RandomPointOnNavmesh(roamCenter, roamRadius));
        }
    }

    private Vector3 RandomPointOnNavmesh(Vector3 center, float radius, int tries = 10)
    {
        for (int i = 0; i < tries; i++)
        {
            Vector3 rand = center + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(rand, out var hit, 2.0f, agent.areaMask))
                return hit.position;
        }
        return transform.position;
    }

    private void SafeStopAgent()
    {
        if (!agent || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;
        if (!agent.isStopped) agent.isStopped = true;
        if (agent.hasPath) agent.ResetPath();
    }

    private bool EnsureOnNavMesh()
    {
        if (!agent || !agent.isActiveAndEnabled) return false;
        if (agent.isOnNavMesh) return true;

        if (NavMesh.SamplePosition(transform.position, out var hit, 5f, agent.areaMask))
        {
            agent.Warp(hit.position);
            return true;
        }
        return false;
    }

    private void SetSpeed(float worldSpeed)
    {
        if (!animator) return;
        float norm = (agent && agent.speed > 0.0001f) ? worldSpeed / agent.speed : 0f;
        animator.SetFloat(speedHash, norm, speedDamp, Time.deltaTime);
    }

    private void FaceTarget(Vector3 pos)
    {
        Vector3 dir = pos - transform.position; dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);
    }

    private float DistanceToTarget() =>
        !target ? float.PositiveInfinity : Vector3.Distance(transform.position, target.position);

    private bool IsFacingTarget(Transform t, float minDot)
    {
        Vector3 dir = (t.position - transform.position); dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return true;
        dir.Normalize();
        return Vector3.Dot(transform.forward, dir) >= minDot;
    }

    private IEnemyAttack SelectBestAttack(bool canSee, float dist)
    {
        if (_attacks == null || _attacks.Length == 0 || !target) return null;

        IEnemyAttack best = null;
        float bestScore = float.NegativeInfinity;

        foreach (var atk in _attacks)
        {
            if (atk == null) continue;
            if (atk.IsOnCooldown) continue;
            if (dist < atk.MinRange || dist > atk.MaxRange) continue;
            if (atk.RequireLOS && !canSee) continue;

            float center = (atk.MinRange + atk.MaxRange) * 0.5f;
            float span = Mathf.Max(0.01f, atk.MaxRange - atk.MinRange);
            float score = 1f - (Mathf.Abs(dist - center) / (span * 0.5f));
            if (atk.Kind == AttackKind.Ranged) score += 0.2f;

            if (score > bestScore) { bestScore = score; best = atk; }
        }
        return best;
    }

    private bool TryExecute(IEnemyAttack best)
    {
        if (best == null || target == null) return false;
        if (!IsFacingTarget(target, attackFacingDot)) return false;
        if (best.MinRange > DistanceToTarget() || DistanceToTarget() > best.MaxRange) return false;

        SafeStopAgent();
        FaceTarget(target.position);
        SetSpeed(0f);

        if (best.Kind == AttackKind.Melee)
        {
            // 근접: Move가 애니 트리거만 건다
            animator.SetTrigger(meleeTrigger);

            // 쿨다운/내부 상태는 근접판정 스크립트에 알려줌
            if (best is MeleeAttack melee) melee.StartSwing(); // ← 아래 MeleeAttack에 추가됨
            return true;
        }
        else
        {
            // 원거리: 기존처럼 스스로 발사
            best.Attack(target);
            return true;
        }
    }
}
