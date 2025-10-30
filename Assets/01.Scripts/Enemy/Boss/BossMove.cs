using UnityEngine;
using UnityEngine.AI;

public class BossMove : MonoBehaviour
{
    [Tooltip("보스 상태 ")]
    private enum State { Idle, Chase, Attack, Stop }  
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

    [Header("보스 시야/카이팅 관련")]
    [Tooltip("이 아래로 들어오면 급히 후퇴")]
    [SerializeField] private float desiredMin = 10f;
    [Tooltip("이 안쪽( [min, max] )이면 좌/우로 선회")]
    [SerializeField] private float desiredMax = 18f;
    [Tooltip("너무 멀면 이 이하로만 접근")]
    [SerializeField] private float softApproachStop = 22f;
    [SerializeField] private float sightRange = 35f;
    [SerializeField] private float fovDeg = 110f;
    [SerializeField] private float stopChaseDistance = 60f;

    [Header("보스 움직임 관련")]
    [SerializeField] private float kiteBackSpeed = 6f;
    [SerializeField] private float strafeRadius = 12f;
    [SerializeField] private float strafeSwitchTime = 2.5f; // 좌/우 전환 주기
    private float _strafeSign = 1f;
    private float _nextSwitch;

    [Header("거리")]
    [SerializeField] private float preferMeleeMax = 6f;   // 이 이하면 근접 우선
    [SerializeField] private float preferRangeMin = 10f;  // 이 이상이면 원거리 우선    
    

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
        bool see = CanSee(player, sightRange, fovDeg);

        switch (_state)
        {
            case State.Idle:
                agent.isStopped = true;
                if (see) _state = State.Chase;
                break;

            case State.Chase:
                if (!see && dist > sightRange) { _state = State.Idle; break; }
                if (dist > stopChaseDistance)   { _state = State.Idle; break; }

                // 이동
                agent.isStopped = false;
                agent.SetDestination(player.position);

                // 공격 전환 판단
                if (melee != null && dist <= preferMeleeMax && melee.CanUse(player, dist))
                { 
                    _state = State.Attack;
                    agent.isStopped = true;
                    melee.Execute(player);
                    break;
                }
                if (ranged != null && dist >= preferRangeMin && ranged.CanUse(player, dist))
                {
                    _state = State.Attack;
                    agent.isStopped = true;
                    ranged.Execute(player);
                    break;
                }
                if (summoner != null && summoner.CanUse(player, dist)) 
                {
                    _state = State.Attack;
                    agent.isStopped = true;
                    summoner.Execute(player);
                    break; 
                }
                break;

            case State.Attack:
                // 공격이 돌아가는 동안 이동 금지
                agent.isStopped = true;                
                bool busy = (melee != null && melee.IsRunning) || (ranged != null && ranged.IsRunning) || (summoner != null && summoner.IsRunning);
                if (!busy) _state = State.Chase;
                break;

            case State.Stop:
                agent.isStopped = true;
                // 외부에서 일정 시간 후 Idle로 돌리거나, 피격/페이즈 전환 시 사용
                break;
        }

        FaceTarget3D(player.position, 8f);
    }

    private bool CanSee(Transform t, float range, float fov)
    {
        Vector3 to = t.position - transform.position;
        if (to.sqrMagnitude > range * range) return false;
        if (Vector3.Angle(transform.forward, to) > fov * 0.5f) return false;
        // 필요하면 Raycast로 LOS 추가
        return true;
    }

    // private void FaceTargetOnGround(Vector3 world, float turnSpeed)
    // {
    //     Vector3 to = world - transform.position; to.y = 0;
    //     if (to.sqrMagnitude < 0.001f) return;
    //     Quaternion look = Quaternion.LookRotation(to);
    //     transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * turnSpeed);
    // }

    private void FaceTarget3D(Vector3 world, float turnSpeed) {
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
