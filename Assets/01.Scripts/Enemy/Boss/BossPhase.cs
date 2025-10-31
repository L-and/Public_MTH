using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BossPhase : MonoBehaviour
{
    [Header("보스 체력 관련")]
    [SerializeField] public int[] phaseMaxHp = { 50, 80, 100 };   // 보스 체력 변수. 1페 2페 3페 순        

    [Header("플레이어 공격 처리용")]
    [SerializeField] private LayerMask playerProjectile = 15;

    [Header("페이즈 전환")]
    public UnityEvent<int> onPhaseStarted;      // 페이즈 시작체크용. 0=1페이즈 시작, 1=2페 , 2=3페
    public UnityEvent<float, float> onDamage;   // UI갱신용
    public UnityEvent onBossDead;

    private int _phaseIndex;
    private float _currentHp;   // 현시점 페이즈 체력
    private float _currentMax;  // 현시점 페이즈 최대 체력
    private bool _isDead;


    void Start()
    {
        BeginPhase(0);
    }

    public void ApplyDamage(float dmg, Vector3 hitPoint, Component source = null)
    {
        if (_isDead || dmg <= 0)
        {
            return;
        }

        _currentHp -= dmg;
        if (_currentHp < 0f)
        {
            _currentHp = 0f;
        }
        onDamage?.Invoke(_currentHp, _currentMax);

        //페이즈 종료 -> 다음 페이즈로 진입
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
        _currentHp = _currentMax;
        onPhaseStarted?.Invoke(_phaseIndex);
        onDamage?.Invoke(_currentHp, _currentMax);
        Debug.Log($"Boss Phase: {_phaseIndex}");
    }

    private void Die()
    {
        if (_isDead)
        {
            return;
        }
        _isDead = true;
        onBossDead?.Invoke();
        // 연출 후 없애고 싶다면 Destroy(gameObject)도 괜찮을지도..?
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void TryDealDamageFrom(Collider col, Vector3 hitPoint)
    {
        if (_isDead) return;

        if ((playerProjectile.value & (1 << col.gameObject.layer)) == 0)
        {
            return;
        }

        float damage = 0f;
        if (col.TryGetComponent<HitSource>(out var hs))
        {
            damage = Mathf.Max(0f, hs.damage);
        }
        else
        {
            hs = col.GetComponentInParent<HitSource>();
            if (hs != null)
            {
                damage = Mathf.Max(0f, hs.damage);
            }
        }
        if (damage <= 0f)
        {
            return;
        }
        ApplyDamage(damage, hitPoint, col);
    }


}
