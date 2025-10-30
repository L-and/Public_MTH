using UnityEngine;
using UnityEngine.AI;

public class BossMove : MonoBehaviour
{
    [Tooltip("보스 상태 ")]
    private enum State { Idle, Combat, Stop }  
    [SerializeField] private State _state = State.Idle; // 기본 상태: Idle

    [Header("공격 대상")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private MonoBehaviour meleeComp;   // BossMelee
    [SerializeField] private MonoBehaviour rangeComp;   // BossRange
    [SerializeField] private MonoBehaviour summonComp;  // BossSummon

    private IMeleeAttacker melee;
    private IRangedAttacker ranged;
    private ISummonAttacker summoner;

    [Header("보스 시야")]
    [SerializeField] private float sightRange = 40f;
    [SerializeField] private float fovDeg = 120f;

    [Header("보스 카이팅 관련")]
    [Tooltip("이 아래로 들어오면 급히 후퇴")]
    [SerializeField] private float desiredMin = 10f;
    [Tooltip("이 안쪽( [min, max] )이면 좌/우로 선회")]
    [SerializeField] private float desiredMax = 18f;
    [Tooltip("너무 멀면 이 이하로만 접근")]
    [SerializeField] private float softApproachStop = 22f;    
    
    [SerializeField] private float stopChaseDistance = 60f;

    [Header("보스 움직임 관련")]
    [SerializeField] private float kiteBackSpeed = 6f;
    [SerializeField] private float strafeRadius = 12f;
    [SerializeField] private float strafeSwitchTime = 2.5f; // 좌/우 전환 주기
    private float _strafeSign = 1f;
    private float _nextSwitch;
    

    private void Awake()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        melee = meleeComp as IMeleeAttacker;
        ranged = rangeComp as IRangedAttacker;
        summoner = summonComp as ISummonAttacker;        
    }

    private void Update()
    {
        if (!player) { _state = State.Idle; return; }

        float dist = Vector3.Distance(transform.position, player.position);
        bool see = CanSeePlayer();

        switch (_state)
        {
            case State.Idle:
                agent.isStopped = true;
                if (see) _state = State.Combat;
                break;

            case State.Combat:
                // 1) 이동 결정: 카이팅
                Vector3 tgt = transform.position;

                if (dist < desiredMin) // Too close → back off
                {
                    Vector3 dir = (transform.position - player.position).normalized;
                    tgt = transform.position + dir * (desiredMin - dist + 2f);
                }
                else if (dist <= desiredMax) // In pocket → strafe
                {
                    if (Time.time >= _nextSwitch) { _strafeSign *= -1f; _nextSwitch = Time.time + strafeSwitchTime; }
                    Vector3 right = Vector3.Cross(Vector3.up, (player.position - transform.position).normalized);
                    tgt = player.position + right * _strafeSign * strafeRadius; // 원주 따라 선회
                }
                else // Too far → soft approach
                {
                    if (dist > softApproachStop) tgt = player.position;
                    else tgt = transform.position; // 충분히 가깝다면 멈춰서 사격
                }

                agent.isStopped = false;
                agent.SetDestination(tgt);
                FaceTarget3D(player.position, 8f);

                // 2) 능력 사용 우선순위: 원거리 > 소환 > 근접
                if (ranged != null && ranged.CanUse(player, dist)) { agent.isStopped = true; ranged.Execute(player); break; }
                if (summoner != null && summoner.CanUse(player, dist)) { agent.isStopped = true; summoner.Execute(player); break; }
                if (melee != null && dist <= desiredMin && melee.CanUse(player, dist))
                { agent.isStopped = true; melee.Execute(player); }
                break;

            case State.Stop:
                agent.isStopped = true;
                break;
        }
    }
    private bool CanSeePlayer()
    {
        Vector3 to = player.position - transform.position;
        if (to.sqrMagnitude > sightRange * sightRange) return false;
        if (Vector3.Angle(transform.forward, to) > fovDeg * 0.5f) return false;
        // 필요하면 Raycast LOS 추가
        return true;
    }

    // private void FaceTargetOnGround(Vector3 world, float turnSpeed)
    // {
    //     Vector3 to = world - transform.position; to.y = 0;
    //     if (to.sqrMagnitude < 0.001f) return;
    //     Quaternion look = Quaternion.LookRotation(to);
    //     transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * turnSpeed);
    // }

    private void FaceTarget3D(Vector3 world, float turnSpeed) 
    {
        var dir = (world - transform.position).normalized;
        if (dir.sqrMagnitude < 0.0001f) return;
        var look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * turnSpeed);
    }

    // 외부(패턴/이벤트)에서 강제 정지/재개할 수 있게 공개 메서드
    public void ForceStop(float seconds)
    {
        _state = State.Stop;
        Invoke(nameof(BackToIdle), seconds);
    }
    private void BackToIdle() => _state = State.Idle;
}
