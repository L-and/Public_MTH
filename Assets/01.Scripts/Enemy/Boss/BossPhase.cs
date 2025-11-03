using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class BossPhase : MonoBehaviour
{
    [Header("보스 체력 (페이즈 순서대로)")]
    [SerializeField] public int[] phaseMaxHp = { 50, 80, 100 };

    [Header("플레이어 탄 레이어(들)")]
    [Tooltip("플레이어 총알이 속한 레이어를 인스펙터에서 체크하세요.")]
    [SerializeField] private LayerMask playerProjectile;

    [Header("이벤트")]
    public UnityEvent<int> onPhaseStarted;       // 0=1페, 1=2페, 2=3페
    public UnityEvent<float, float> onDamage;    // (current, max) UI 갱신용
    public UnityEvent onBossDead;
    public BossMove _bossMove;

    // ─ 상태 ─
    private int _phaseIndex;
    private float _currentHp;
    private float _currentMax;
    private bool _isDead;

    // ─ UI/외부 접근 편의 ─
    public int CurrentPhaseIndex => _isDead ? phaseMaxHp.Length - 1 : _phaseIndex;
    public float CurrentHp => _currentHp;
    public float CurrentMax => _currentMax;
    public bool IsDead => _isDead;

    private void Start()
    {
        BeginPhase(0);
    }

    public void ApplyDamage(float dmg, Vector3 hitPoint, Component source = null)
    {
        if (_isDead || dmg <= 0f) return;

        _currentHp = Mathf.Max(0f, _currentHp - dmg);
        onDamage?.Invoke(_currentHp, _currentMax);
        // Debug.Log($"[BossPhase] Damage {dmg} → {_currentHp}/{_currentMax}");

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
        _phaseIndex = Mathf.Clamp(nextIndex, 0, phaseMaxHp.Length - 1);
        _currentMax = Mathf.Max(1, phaseMaxHp[_phaseIndex]);
        _currentHp  = _currentMax;

        onPhaseStarted?.Invoke(_phaseIndex);
        onDamage?.Invoke(_currentHp, _currentMax);
        Debug.Log($"[BossPhase] Phase start → {(_phaseIndex + 1)} ({_currentHp}/{_currentMax})");
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        Debug.Log("[BossPhase] Boss dead");
        onBossDead?.Invoke();
        // 필요하면 여기서 Destroy(gameObject) 등 연출 처리
        _bossMove.enabled = false;
    }

    // ─ 충돌/트리거 양쪽 지원 ─
    private void OnCollisionEnter(Collision c)
    {
        // 충돌체에서 첫 접점 사용
        TryDealDamageFrom(c.collider, c.GetContact(0).point);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 트리거일 땐 위치가 없으니 대략 중심 사용
        TryDealDamageFrom(other, transform.position);
    }

    private void TryDealDamageFrom(Collider col, Vector3 hitPoint)
    {
        if (_isDead) return;

        // 레이어 필터: 플레이어 총알 레이어만 통과
        if ((playerProjectile.value & (1 << col.gameObject.layer)) == 0)
            return;

        // 탄에서 데미지 꺼내기 (HitSource가 탄/부모 어디에 붙어 있어도 커버)
        float damage = 0f;
        if (col.TryGetComponent<HitSource>(out var hs))
            damage = Mathf.Max(0f, hs.damage);
        else if (col.GetComponentInParent<HitSource>() is HitSource hs2)
            damage = Mathf.Max(0f, hs2.damage);

        if (damage <= 0f) return;

        ApplyDamage(damage, hitPoint, col);

        // 탄 파괴는 탄 스크립트(Projectile.cs)가 맡고 있다면 여기서 굳이 제거 안 해도 됨.
        // 즉시 지우고 싶다면: Destroy(col.gameObject);
    }
}
