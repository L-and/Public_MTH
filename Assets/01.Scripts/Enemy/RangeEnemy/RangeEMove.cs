using UnityEngine;
using UnityEngine.AI;

public class RangeEMove : MonoBehaviour
{
    [Header("타겟")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("추적 / 시야")]
    [SerializeField] private float sightRange = 20f;
    [SerializeField] private float fovAngle = 80f;
    [SerializeField] private float stopDistance = 2.5f;
    [SerializeField] private LayerMask obstacleMask;

    [Header("배회 (NavMesh 기준 자유 순찰)")]
    [SerializeField] private bool userRoam = false;
    [SerializeField] private float roamRadius = 12f;
    [SerializeField] private Transform roamCenterOverride;
    [SerializeField] private float raomWaitMin = 0.8f;
    [SerializeField] private float roamWaitMax = 1.6f;

    [Header("길찾기")]
    [SerializeField] private float repathInterval = 0.15f;
    [Header("애니메이션")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParam = "moveSpeed";
    [SerializeField] private float speedDamp = 0.1f;
    
    [Header("공격")]
    [SerializeField] private EnemyAttack enemyAttack;   // 공격 스크립트
    [SerializeField] private float attackRange = 2.7f;
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
    private enum State { Idle, Roam, Chase, Shot, Stop }
    private State state = State.Idle;
    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        // if(!enemyDamage)
    }

}