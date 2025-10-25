using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    [Header("타겟")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("추적 / 시야")]
    [SerializeField] private float sightRange = 20f;
    [SerializeField] private float fovAngle = 80f;
    [SerializeField] private float stopDistance = 2.5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("배회 (NavMesh 자유 순찰)")]
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

    [Header("공격")]
    // [SerializeField] private EnemyAttack enemyAttack;   // 공격 스크립트
    [SerializeField] private MonoBehaviour[] attackBehaviours;   // IEnemyAttack 구현들 드래그
    private IEnemyAttack[] _attacks;

    [Header("공격 공통 옵션")]
    [SerializeField, Range(0f, 1f)] private float attackFacingDot = 0.6f;

    [Header("기타 링크 (필요하다면) 현시점 데미지 받는 기능 링크됨")]
    [SerializeField] private EnemyDamage enemyDamage;
    #region 필드
    private NavMeshAgent agent;
    private int speedHash;
    private float repathTimer;
    private Vector3 roamCenter;
    private float roamWaitTimer;
    private float roamWaitTarget;
    #endregion

    #region 상태변화(배회, 추적, 정지 등)
    private enum State { Roam, Chase, Stop }
    private State state = State.Roam;
    #endregion
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!enemyDamage) enemyDamage = GetComponent<EnemyDamage>();

        if(attackBehaviours != null && attackBehaviours.Length > 0)
        {
            _attacks = new IEnemyAttack[attackBehaviours.Length];
            for(int i = 0; i <attackBehaviours.Length; i++)
            {
                _attacks[i] = attackBehaviours[i] as IEnemyAttack;
            }
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

    private void OnEnable()
    {
        EnsureOnNavMesh();
    }

    private void Start()
    {
        if (agent) agent.stoppingDistance = stopDistance;

        roamCenter = (roamCenterOverride ? roamCenterOverride.position : transform.position);
        state = useRoam ? State.Roam : State.Stop;

        // NavMesh 위에 있을 때만 초기 목적지 설정
        if (state == State.Roam && agent && agent.isActiveAndEnabled && EnsureOnNavMesh())
        {
            agent.SetDestination(RandomPointOnNavmesh(roamCenter, roamRadius));
        }
    }

    private void Update()
    {
        if (enemyDamage && enemyDamage.IsDead) { SafeStopAgent(); SetSpeed(0f); return; }

        // NavMeshAgent/OnNavMesh 가드
        if (!agent || !agent.isActiveAndEnabled)
        {
            SetSpeed(0f);
            return;
        }
        if (!agent.isOnNavMesh && !EnsureOnNavMesh())
        {
            // 이번 프레임은 이동/공격 로직 스킵
            SetSpeed(0f);
            return;
        }

        bool canSee = CanSeeTarget();
        float dist = DistanceToTarget();
        var best = SelectBestAttack(canSee, dist);

        // 상태 전이
        switch (state)
        {
            case State.Roam:
                if (!useRoam)
                {
                    state = State.Stop;
                    break;
                }
                if (canSee)
                {
                    state = (dist > stopDistance) ? State.Chase : State.Stop;                    
                }
                break;

            case State.Chase:
                // 사정권 공격이 가능해졌다면 멈추고 공격으로 전환(근접/원거리 모두 해당)
                if (TryExecute(best))
                {
                    state = State.Stop;
                    break;
                }
                if (!canSee)
                {
                    state = useRoam ? State.Roam : State.Stop;
                }
                else if (dist <= stopDistance)
                {
                    state = State.Stop;
                }
                break;

            case State.Stop:
                // 제자리에서 공격을 시도, 불가능하다면 상태 복귀
                if (!TryExecute(best))
                {
                    if (canSee)
                    {
                        state = (dist > stopDistance) ? State.Chase : State.Stop;
                    }
                    else
                    {
                        state = useRoam ? State.Roam : State.Stop;
                    }
                }
                break;
        }

        // 상태 동작
        switch (state)
        {
            case State.Roam:
                RoamTick();
                SetSpeed(agent.velocity.magnitude);
                break;

            case State.Chase:
                repathTimer -= Time.deltaTime;
                if (repathTimer <= 0f && target)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    repathTimer = repathInterval;
                }
                SetSpeed(agent.velocity.magnitude);
                break;

            case State.Stop:
                SafeStopAgent();
                if (target) FaceTarget(target.position);
                SetSpeed(0f);
                break;
        }
    }

    // ─ Vision ─
    private bool CanSeeTarget()
    {
        if (!target) return false;
        Vector3 to = target.position - transform.position;
        if (to.sqrMagnitude > sightRange * sightRange) return false;

        Vector3 flat = new Vector3(to.x, 0, to.z);
        if (Vector3.Angle(transform.forward, flat) > fovAngle) return false;

        // 장애물에 걸리면 시야 차단
        Vector3 eye = transform.position + Vector3.up * 1.6f;
        if (Physics.Raycast(eye, to.normalized, out var hit, sightRange, ~0))
        {
            if ((obstacleMask.value & (1 << hit.collider.gameObject.layer)) != 0)
                return false;
        }
        return true;
    }

    // ─ Roam ─
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

    // ─ Helpers ─
    /// <summary>
    /// NavMesh 위가 아닐 때는 아무 것도 하지 않도록 한 안전한 Stop.
    /// 경고/에러 방지용.
    /// </summary>
    private void SafeStopAgent()
    {
        if (!agent || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        if (!agent.isStopped) agent.isStopped = true;
        if (agent.hasPath) agent.ResetPath();
    }

    /// <summary>
    /// 현재 위치 근처 NavMesh를 찾아 Warp로 보정. 성공 시 true.
    /// </summary>
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

    private float DistanceToTarget()
    {
        if (!target) return float.PositiveInfinity;
        return Vector3.Distance(transform.position, target.position);
    }

    private bool InAttackRange(Transform t, float range)
    {
        Vector3 a = transform.position; a.y = 0f;
        Vector3 b = t.position; b.y = 0f;
        return Vector3.Distance(a, b) <= range;
    }

    private bool IsFacingTarget(Transform t, float minDot)
    {
        Vector3 dir = (t.position - transform.position); dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return true;
        dir.Normalize();
        return Vector3.Dot(transform.forward, dir) >= minDot;
    }

    /// <summary>
    /// 현재 상황에서 가장 적절한 공격 선택 (IEnemyAttack기준)
    /// </summary>
    /// <param name="canSee"></param>
    /// <param name="dist"></param>
    /// <returns></returns>
    private IEnemyAttack SelectBestAttack(bool canSee, float dist)
    {
        if (_attacks == null || _attacks.Length == 0 || !target)
        {
            return null;
        }

        IEnemyAttack best = null;
        float bestScore = float.NegativeInfinity;

        foreach (var atk in _attacks)
        {
            if (atk == null) continue;
            if (atk.IsAttacking || atk.IsOnCooldown) continue;
            if (dist < atk.MinRange || dist > atk.MaxRange) continue;
            if (atk.RequireLOS && !canSee) continue;

            // 사거리 중앙에 가까울수록 가점
            float center = (atk.MinRange + atk.MaxRange) * 0.5f;
            float span = Mathf.Max(0.01f, atk.MaxRange - atk.MinRange);
            float score = 1f - (Mathf.Abs(dist - center) / (span * 0.5f)); // -1~1 근사
            if (atk.Kind == AttackKind.Ranged) score += 0.2f;           // 원거리 약간 선호

            if (score > bestScore) { bestScore = score; best = atk; }
        }
        return best;
    }

    ///<summary> 공격 실행 시도(정면 체크 + 정지 + Attack 호출). 성공 시 true </summary>
    private bool TryExecute(IEnemyAttack best)
    {
        if (best == null || target == null)
        {
            return false;
        }
        if (!IsFacingTarget(target, attackFacingDot)) return false;
        if (!best.CanAttack(target)) return false;

        SafeStopAgent();
        FaceTarget(target.position);
        SetSpeed(0f);
        best.Attack(target);
        return true;
    }
}
