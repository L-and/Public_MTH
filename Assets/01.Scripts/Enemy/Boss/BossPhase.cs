using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class BossPhase : MonoBehaviour
{
    [System.Serializable]
    public class PhasePattern
    {
        [Header("이 페이즈에서 켜고/끄는 컴포넌트")]
        public MonoBehaviour[] enableThese;   // 활성화할 스크립트들
        public MonoBehaviour[] disableThese;  // 비활성화할 스크립트들

        [Header("이 페이즈에서 켜고/끄는 오브젝트")]
        public GameObject[] enableObjects;    // SetActive(true)
        public GameObject[] disableObjects;   // SetActive(false)

        [Header("애니메이터/이펙트/사운드 등")]
        public UnityEvent onEnter;            // 페이즈 진입 시 1회 호출
        public UnityEvent onExit;             // 다음 페이즈로 넘어갈 때 1회 호출
        
    }

    [Header("보스 체력 (페이즈 순서대로)")]
    [SerializeField] public int[] phaseMaxHp = { 50, 80, 100 };

    [Header("플레이어 탄 레이어(들)")]
    [Tooltip("플레이어 총알이 속한 레이어를 인스펙터에서 체크하세요.")]
    [SerializeField] private LayerMask playerProjectile;

    [Header("패턴(페이즈 설정)")]
    [Tooltip("phaseMaxHp 길이와 동일하게 맞추는 걸 추천")]
    [SerializeField] private PhasePattern[] phasePatterns;

    [Header("이벤트 (선택)")]
    public UnityEvent<int> onPhaseStarted;       // 0=1페, 1=2페, 2=3페
    public UnityEvent<float, float> onDamage;    // (current, max) UI 갱신용
    public UnityEvent onBossDead;

    [Header("연결(선택)")]
    public BossMove _bossMove;                   // 죽을 때 끄고 싶으면 넣기

    [Header("사망 컷신 프리팹 (BossDie 스크립트 포함)")]
    [SerializeField] private GameObject bossDeathPrefab;
    [SerializeField] private Vector3 deathSpawnOffset = Vector3.zero;
    [SerializeField] private bool inheritRotationOnDeath = true;
    [SerializeField] private bool inheritScaleOnDeath = false;
        
    [Header("사망 시 원본 보스 정리")]
    [SerializeField] private GameObject bossRootToDestroy; // 비우면 this.gameObject
    [SerializeField] private bool disableBeforeDestroy = true; // 콜라이더/렌더러/AI 비활성화
    [SerializeField] private float destroyDelay = 0.05f;

    // ─ 상태 ─
    private int _phaseIndex;
    private float _currentHp;
    private float _currentMax;
    private bool _isDead;
    private PhasePattern _prevPattern;           // 이전 페이즈 저장

    // ─ UI/외부 접근 편의 ─
    public int CurrentPhaseIndex => _isDead ? phaseMaxHp.Length - 1 : _phaseIndex;
    public float CurrentHp => _currentHp;
    public float CurrentMax => _currentMax;
    public bool IsDead => _isDead;

    // 보스 죽었을 경우 발생하는 이벤트 등록
    public static event Action OnBossDefeated;

    private void Start()
    {
        BeginPhase(0);
    }

    public void ApplyDamage(float dmg, Vector3 hitPoint, Component source = null)
    {        
        if (_isDead || dmg <= 0f) return;

        _currentHp = Mathf.Max(0f, _currentHp - dmg);
        onDamage?.Invoke(_currentHp, _currentMax);
        Debug.Log("getting hit!");

        if (_currentHp <= 0f)
        {
            if (_phaseIndex + 1 < phaseMaxHp.Length)
            {
                BeginPhase(_phaseIndex + 1);
            }
            else
            {
                Die();
            }
        }
    }

    private void BeginPhase(int nextIndex)
    {
        // 이전 페이즈 onExit 호출
        if (_prevPattern != null)
            _prevPattern.onExit?.Invoke();

        _phaseIndex = Mathf.Clamp(nextIndex, 0, phaseMaxHp.Length - 1);
        _currentMax = Mathf.Max(1, phaseMaxHp[_phaseIndex]);
        _currentHp  = _currentMax;

        // 이번 페이즈 패턴 적용
        var cfg = GetPattern(_phaseIndex);
        ApplyPattern(cfg);
        _prevPattern = cfg;

        onPhaseStarted?.Invoke(_phaseIndex);
        onDamage?.Invoke(_currentHp, _currentMax);
        Debug.Log($"[BossPhase] Phase start → {(_phaseIndex + 1)} ({_currentHp}/{_currentMax})");
    }

    private PhasePattern GetPattern(int index)
    {
        if (phasePatterns == null || phasePatterns.Length == 0) return null;
        if (index < 0 || index >= phasePatterns.Length) return null;
        return phasePatterns[index];
    }

    private void ApplyPattern(PhasePattern cfg)
    {
        if (cfg == null) return;

        // 오브젝트/컴포넌트 on/off
        if (cfg.disableThese != null)
            foreach (var c in cfg.disableThese) if (c) c.enabled = false;
        if (cfg.disableObjects != null)
            foreach (var go in cfg.disableObjects) if (go) go.SetActive(false);

        if (cfg.enableThese != null)
            foreach (var c in cfg.enableThese)  if (c) c.enabled = true;
        if (cfg.enableObjects != null)
            foreach (var go in cfg.enableObjects) if (go) go.SetActive(true);

        // 사용자 정의 이벤트 (애니 트리거, 이펙트 재생 등)
        cfg.onEnter?.Invoke();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        Debug.Log("[BossPhase] Boss dead");
        onBossDead?.Invoke();
        if (_bossMove) _bossMove.enabled = false;
        OnBossDefeated?.Invoke();
        // 필요 시 Destroy(gameObject) 또는 연출 코루틴
        // 1) 사망 컷신 프리팹 소환
        if (bossDeathPrefab != null)
        {
            Vector3 spawnPos = transform.position + deathSpawnOffset;
            Quaternion spawnRot = inheritRotationOnDeath ? transform.rotation : Quaternion.identity;

            var cutscene = Instantiate(bossDeathPrefab, spawnPos, spawnRot);
            if (inheritScaleOnDeath)
                cutscene.transform.localScale = transform.localScale;
        }
        else
        {
            Debug.LogWarning("[BossPhase] bossDeathPrefab이 비었습니다. 사망 컷신을 소환하지 않습니다.");
        }
         // 2) 기존 보스 정리(렌더/충돌/AI 끄고, 약간 지연 뒤 삭제)
        var victim = bossRootToDestroy ? bossRootToDestroy : gameObject;

        if (disableBeforeDestroy && victim != null)
        {
            // NavMesh/이동 비활성
            var agent = victim.GetComponentInChildren<NavMeshAgent>(true);
            if (agent) agent.enabled = false;

            // 콜라이더 OFF
            var cols = victim.GetComponentsInChildren<Collider>(true);
            foreach (var c in cols) c.enabled = false;

            // 리지드바디 관성 제거
            var rbs = victim.GetComponentsInChildren<Rigidbody>(true);
            foreach (var rb in rbs) rb.isKinematic = true;

            // 렌더러 OFF(바로 안 보이게)
            var rends = victim.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rends) r.enabled = false;
        }

        if (victim != null)
            Destroy(victim, destroyDelay);
    }

    // ─ 충돌/트리거 양쪽 지원 ─
    private void OnCollisionEnter(Collision c)
    {
        TryDealDamageFrom(c.collider, c.GetContact(0).point);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDealDamageFrom(other, transform.position);
    }

    private void TryDealDamageFrom(Collider col, Vector3 hitPoint)
    {
        if (_isDead) return;

        int l = col.gameObject.layer;
        bool layerPass = (playerProjectile & (1 << l)) != 0;

        Debug.Log($"[BossPhase] Hit by {col.name} (layer={l}, name={LayerMask.LayerToName(l)}), " +
                $"pass={layerPass}, bossObj={name}");

        if (!layerPass) return;
        if (!GameManager.PlayerManager)
        {
            Debug.LogWarning("[BossPhase] GameManager.PlayerManager == null");
            return;
        }
        if (GameManager.PlayerManager.PlayerStat == null)
        {
            Debug.LogWarning("[BossPhase] PlayerManager.PlayerStat == null");
            return;
        }

        float damage = GameManager.PlayerManager.PlayerStat.bulletDamage;
        Debug.Log($"[BossPhase] bulletDamage={damage}");
        if (damage <= 0f)
        {
            Debug.LogWarning("[BossPhase] bulletDamage <= 0");
            return;
        }

        ApplyDamage(damage, hitPoint, col);

        // // 레이어 필터: 플레이어 총알 레이어만 통과
        // if ((playerProjectile.value & (1 << col.gameObject.layer)) == 0)
        //     return;

        // // 데미지 획득(예: GameManager에서 총알 데미지)
        // if (!GameManager.PlayerManager)
        // {
        //     Debug.LogWarning("GameManager.PlayerManager 가 존재하지 않습니다.");
        //     return;
        // }
        // float damage = GameManager.PlayerManager.PlayerStat.bulletDamage;
        // if (damage <= 0f) return;

        // ApplyDamage(damage, hitPoint, col);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 품질 체크: 길이 안 맞으면 경고만
        if (phasePatterns != null && phasePatterns.Length != phaseMaxHp.Length)
        {
            // 길이를 꼭 맞출 필요는 없지만, 맞추면 관리가 편함
            // Debug.LogWarning($"[BossPhase] phasePatterns({phasePatterns.Length}) 길이가 phaseMaxHp({phaseMaxHp.Length})와 다릅니다.");
        }
    }
#endif
}
