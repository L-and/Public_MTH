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
    [SerializeField] private MonoBehaviour spikeComp;   // BossSpike
    [SerializeField] private MonoBehaviour rangeComp;   // BossRange
    [SerializeField] private MonoBehaviour summonComp;  // BossSummon
    [SerializeField] private MonoBehaviour beamComp;    // BossBeam

    private IMeleeAttacker melee;
    private IRangedAttacker ranged;
    private ISummonAttacker summoner;
    private IRangedAttacker beamed;

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

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.autoRepath = false;
        melee = spikeComp as IMeleeAttacker;
        ranged = rangeComp as IRangedAttacker;
        summoner = summonComp as ISummonAttacker;
        beamed = beamComp as IRangedAttacker;
        FindPlayerByTage();
    }

    private void Update()
    {
        // if (!player) { _state = State.Idle; return; }
        if(player == null)
        {
            FindPlayerByTage();
        }

        float dist = Vector3.Distance(transform.position, player.position);
        bool see = CanSeePlayer();

        switch (_state)
        {
            case State.Idle:
                if (agent) { agent.isStopped = true; agent.ResetPath(); agent.velocity = Vector3.zero; }
                if (see) _state = State.Combat;
                break;

            case State.Combat:

                if (agent) { agent.isStopped = true; agent.ResetPath(); agent.velocity = Vector3.zero; }
                //FaceTarget3D(player.position, 8f);

                // 2) 능력 사용 우선순위대로 나열
                if (summoner != null && summoner.CanUse(player, dist)) { summoner.Execute(player); break; }
                if (melee != null && melee.CanUse(player, dist)) { melee.Execute(player); }
                if (ranged != null && ranged.CanUse(player, dist)) { ranged.Execute(player); break; }
                if (beamed != null && beamed.CanUse(player, dist)) { beamed.Execute(player); break; }
                break;

            case State.Stop:
                if (agent) { agent.isStopped = true; agent.ResetPath(); agent.velocity = Vector3.zero; }
                break;
        }
        if (agent) agent.nextPosition = transform.position;
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
    
    private void FindPlayerByTage()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // 외부(패턴/이벤트)에서 강제 정지/재개할 수 있게 공개 메서드
    public void ForceStop(float seconds)
    {
        _state = State.Stop;
        Invoke(nameof(BackToIdle), seconds);
    }
    private void BackToIdle() => _state = State.Idle;
}
