using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class TankerMove : MonoBehaviour
{
    [Header("Target / Links")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private EnemyDamage enemyDamage;
    [SerializeField] private Animator animator;

    [Header("Attacks (drag MeleeAttack, RangedAttack)")]
    [SerializeField] private MonoBehaviour[] attackBehaviours; // IEnemyAttack
    private IEnemyAttack melee, ranged;

    [Header("Animator Params (EnemyMove 스타일)")]
    [SerializeField] private string meleeTrigger = "Attack"; // 근접 모션 트리거

    [Header("Ranges")]
    [SerializeField] private float sightRange = 18f;
    [SerializeField] private float fovAngle = 85f;
    [SerializeField] private float stopDistance = 2.4f;  // NavMeshAgent 정지 거리
    [SerializeField] private float punchRange = 3.2f;    // 근접 상한(= Melee.MaxRange 보다 작지 않게)
    [SerializeField] private float breathMin = 6f;       // 원거리 최소(이상일 때부터 쏠 수 있음)

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2.2f;
    [SerializeField] private float backstepSpeedMul = 1.0f;   // 분노 전, 가까우면 살짝 뒤로
    [SerializeField] private float repathInterval = 0.1f;

    [Header("Leash / Return")]
    [SerializeField] private float leashDistance = 25f;
    [SerializeField] private float loseSightToReturnSec = 2.0f;
    [SerializeField] private float homeStopDistance = 0.6f;

    [Header("Enrage")]
    [SerializeField] private float enrageThreshold = 0.5f; // 체력 비율 50%↓면 분노
    [SerializeField] private float enrageSpeedMul = 1.3f;
    [SerializeField] private float postAttackDelay = 0.8f;
    [SerializeField] private float enragePostDelayMul = 0.7f;

    [Header("Vision (Walls only)")]
    [SerializeField] private LayerMask obstacleMask = 0;

    // internal
    private NavMeshAgent agent;
    private Vector3 homePos; private Quaternion homeRot;
    private float loseSightTimer, delayTimer, repathTimer;
    private bool enraged;
    private float attackLockAge;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!enemyDamage) enemyDamage = GetComponent<EnemyDamage>();

        FindPlayersByTage();

        // IEnemyAttack 캐스팅
        foreach (var mb in attackBehaviours)
        {
            if (mb is IEnemyAttack atk)
            {
                if (atk.Kind == AttackKind.Melee)  melee  = atk;
                if (atk.Kind == AttackKind.Ranged) ranged = atk;
            }
        }

        agent.updateRotation = true;
        agent.stoppingDistance = stopDistance;
        if (animator) animator.applyRootMotion = false;

        homePos = transform.position;
        homeRot = transform.rotation;
        // 🔵 ADD: 공격 스크립트/애니메이터 레퍼런스 확인
        //Debug.Log($"[Tanker] refs  melee={(melee!=null?melee.ToString():"null")}, ranged={(ranged!=null?ranged.ToString():"null")}, animator={(animator?animator.name:"null")}");

    }

    private void Update()
    {
        if(target == null)
        {
            FindPlayersByTage();
        }
        if (!target || !agent || !agent.isOnNavMesh) return;
        // 🔵 ADD: 기본 상태 스냅샷 (10프레임마다)
        if (Time.frameCount % 10 == 0)
        {
            float distDbg = DistanceToTarget();
            bool canSeeDbg = CanSeeTarget();
            // Debug.Log($"[Tanker] dist={distDbg:F2}, canSee={canSeeDbg}, enraged={enraged}," +
            //           $" meleeReady={(melee != null && !melee.IsOnCooldown)}," +
            //           $" rangedReady={(ranged != null && !ranged.IsOnCooldown)}");
        }
        // Debug.Log($"[Tanker] flags  meleeAtk={(melee!=null && melee.IsAttacking)}, rangedAtk={(ranged!=null && ranged.IsAttacking)}, delay={delayTimer:F2}");
        if (ranged != null)
            // Debug.Log($"[Tanker] ranged window={InRangedWindow(DistanceToTarget())}, dist={DistanceToTarget():F2}, min={Mathf.Max(breathMin, ranged.MinRange):F1}, max={ranged.MaxRange:F1}, onCD={ranged.IsOnCooldown}, LOS={CanSeeTarget()}");
        if (melee != null)
            // Debug.Log($"[Tanker] melee gate dist={DistanceToTarget():F2} <= {Mathf.Max(punchRange, melee.MaxRange):F1}, onCD={melee.IsOnCooldown}");

        if (!agent || !agent.isOnNavMesh || !target) { SetMoveAnim(0f); return; }
        if (enemyDamage && enemyDamage.IsDead) { StopMove(); SetMoveAnim(0f); return; }

        // 분노 판정 & 이동속도 반영
        enraged = (enemyDamage && enemyDamage.HealthRatio <= enrageThreshold);
        agent.speed = walkSpeed * (enraged ? enrageSpeedMul : 1f);

        // 공격 중이면 이동 멈추고 바라보기
        bool attackLock = (melee != null && melee.IsAttacking) || (ranged != null && ranged.IsAttacking);
        if (attackLock)
        {
            attackLockAge += Time.deltaTime;

            // 애니 상태가 공격이 아닐 때 오래 잠기면 강제 해제(응급용)
            if (attackLockAge > 1.2f)
            {                
                attackLock = false; // 이동/공격 분기로 진행시켜 진단 계속
            }
            else
            {
                StopMove(); SetMoveAnim(0f); Face(target.position); return;
                            }
        }
        else
        {
            attackLockAge = 0f;
        }

        if (delayTimer > 0f) { delayTimer -= Time.deltaTime; Face(target.position); return; }

        // 시야/거리 계산
        bool canSee = CanSeeTarget();
        float dist = DistanceToTarget();

        // 리쉬: 너무 멀거나, 시야 오래 끊김
        if (!HandleLeash(canSee, dist)) return;

        if (ranged != null)
        

        // ===== 분노 전: 원거리 전용 =====
        if (!enraged)
        {
            // 🔵 ADD: 원거리 게이트 디버그
            if (Time.frameCount % 15 == 0 && ranged != null)
            {
                float d = dist;
                bool inWindow = InRangedWindow(d);
                
            }

            // 1) 원거리 창에 들어오면 멈추고 사격
            if (ranged != null && !ranged.IsOnCooldown && InRangedWindow(dist) && canSee)
            {
                StopMove(); SetMoveAnim(0f); Face(target.position);
                ranged.Attack(target); // RangedAttack 내부에서 shootTrigger 처리함 :contentReference[oaicite:3]{index=3}
                delayTimer = Mathf.Max(0.05f, postAttackDelay);
                return;
            }

            // 2) 너무 멀면 접근, 너무 가까우면 살짝 후퇴
            if (dist > ranged?.MaxRange)
            {
                MoveTo(target.position);
            }
            else if (dist < ranged?.MinRange)
            {
                // 뒤로 한 걸음
                Vector3 back = transform.position - (target.position - transform.position).normalized * 2.0f;
                MoveTo(back);
                agent.speed = walkSpeed * Mathf.Max(0.6f, backstepSpeedMul);
            }
            else
            {
                // 창에 거의 들어왔지만 쿨다운이면 멈춰서 대기
                StopMove();
            }

            return;
        }

        // ===== 분노 후: 추적 + 근접 전용 =====
        // 추적
        if (dist > stopDistance) MoveTo(target.position); else StopMove();

        // 근접 가능하면 컨트롤러가 애니 트리거 + StartSwing (EnemyMove 스타일) :contentReference[oaicite:4]{index=4} :contentReference[oaicite:5]{index=5}
        if (melee != null && !melee.IsOnCooldown && dist <= Mathf.Max(punchRange, melee.MaxRange))
        {
            StopMove(); SetMoveAnim(0f); Face(target.position);

            // 🔵 ADD: 근접 트리거 로그
            if (animator && !string.IsNullOrEmpty(meleeTrigger))
            {
                animator.SetTrigger(meleeTrigger);
                
            }

            // 🔵 ADD: 근접 공격 호출 로그
            
            melee.Attack(target);

            delayTimer = Mathf.Max(0.05f, postAttackDelay * enragePostDelayMul);
            return;
        }

        // 창에 아직 못 들어왔으면 계속 추적
    }

    // ---------- helpers ----------
    private bool InRangedWindow(float dist)
    {
        if (ranged == null) return false;
        return dist >= Mathf.Max(breathMin, ranged.MinRange) && dist <= ranged.MaxRange;
    }

    private bool HandleLeash(bool canSee, float dist)
    {
        if (dist >= leashDistance || LeashTimer(canSee))
        {
            float d = Vector3.Distance(transform.position, homePos);
            if (d > homeStopDistance) { MoveTo(homePos); Face(homePos); }
            else { StopMove(); transform.rotation = Quaternion.Slerp(transform.rotation, homeRot, Time.deltaTime * 4f); SetMoveAnim(0f); }
            return false;
        }
        return true;
    }
    private bool LeashTimer(bool canSeeNow)
    {
        if (canSeeNow) { loseSightTimer = 0f; return false; }
        loseSightTimer += Time.deltaTime;
        return loseSightTimer >= loseSightToReturnSec;
    }

    private void MoveTo(Vector3 pos)
    {
        repathTimer -= Time.deltaTime;
        if (agent.isStopped) agent.isStopped = false;
        if (repathTimer <= 0f) { agent.SetDestination(pos); repathTimer = repathInterval; }
        SetMoveAnim(agent.velocity.magnitude);
    }
    private void StopMove()
    {
        if (!agent.isStopped) agent.isStopped = true;
        if (agent.hasPath) agent.ResetPath();
    }
    private void SetMoveAnim(float worldSpeed)
    {
        if (!animator) return;
        float norm = (agent.speed > 0.0001f) ? worldSpeed / agent.speed : 0f;
        animator.SetFloat("moveSpeed", norm, 0.1f, Time.deltaTime);
    }
    private void Face(Vector3 pos, float turn = 12f)
    {
        Vector3 dir = pos - transform.position; dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * turn);
    }
    private float DistanceToTarget() => !target ? float.PositiveInfinity : Vector3.Distance(transform.position, target.position);

    private bool CanSeeTarget()
    {
        if (!target) return false;
        Vector3 to = target.position - transform.position;
        if (to.sqrMagnitude > sightRange * sightRange) return false;
        Vector3 flat = new Vector3(to.x, 0, to.z);
        if (Vector3.Angle(transform.forward, flat) > fovAngle) return false;

        Vector3 eye = transform.position + Vector3.up * 1.6f;
        if (Physics.Raycast(eye, to.normalized, out var hit, sightRange, ~0))
        {
            // 🔵 ADD: 레이캐스트가 맞춘 오브젝트/레이어 확인
            // (장애물 마스크에 포함되어 있으면 시야 차단으로 판정)
            // if (Time.frameCount % 20 == 0)
            //     Debug.Log($"[Tanker] LOS ray hit: {hit.collider.name} (layer={hit.collider.gameObject.layer})");

            if ((obstacleMask.value & (1 << hit.collider.gameObject.layer)) != 0) return false;
        }
        return true;
    }

    private void FindPlayersByTage()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if(playerObj != null)
        {
            target = playerObj.transform;
        }
    }
}
