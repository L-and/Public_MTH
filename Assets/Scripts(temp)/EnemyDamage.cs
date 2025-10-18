using UnityEngine;
using UnityEngine.AI;

public class EnemyDamage : MonoBehaviour, IDamageableZone
{
    [Header("Rules")]
    [Tooltip("몸통이 맞아야 하는 횟수")]
    [SerializeField] private int bodyHitsToDie = 3;
    
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTrigger = "Die"; // Animator 트리거 이름

     [Header("Cleanup")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider[] collidersToDisable;   // 루트/바디 콜라이더
    [SerializeField] private MonoBehaviour[] componentsToDisable; // AI 스크립트 등
    [SerializeField] private float extraStayTime = 2.0f; // 애니 끝나고 시체 유지

    [Tooltip("맞을 때 재생할 VFX/SFX (선택)")]
    [SerializeField] private GameObject hitVfx;

    private int bodyHitCount;
    public bool IsDead { get; private set; }

    public void ApplyHit(float rawDamage, Vector3 hitPoint, HitZones zone)
    {
        if (IsDead) return;

        if (zone == HitZones.Head)
        {
            Kill();
            return;
        }

        // 몸통
        bodyHitCount++;
        if (bodyHitCount >= bodyHitsToDie) Kill();
    }

    private void Kill()
    {
        if (IsDead) return;
        IsDead = true;

        if (agent) agent.enabled = false;
        foreach (var c in componentsToDisable) if (c) c.enabled = false;
        foreach (var col in collidersToDisable) if (col) col.enabled = false;

        if (animator)
        {
            animator.ResetTrigger("Die");
            animator.SetTrigger("Die");
            StartCoroutine(WaitDeathAndDespawn());
        }
        else Destroy(gameObject, 2f);
    }

    // Death 애니메이션 끝에서 호출할 애니메이션 이벤트
    // (클립 마지막 프레임에 AE_DeathEnd 추가)
    public void AE_DeathEnd()
    {
        Destroy(gameObject, extraStayTime);
    }
    
    private System.Collections.IEnumerator WaitDeathAndDespawn()
    {
        // 한 프레임 기다려 스테이트 반영
        yield return null;

        var info = animator.GetCurrentAnimatorStateInfo(0); // 레이어 0 기준
        float len = info.length > 0 ? info.length : 1.0f;
        yield return new WaitForSeconds(len + extraStayTime);
        Destroy(gameObject);
    }
}
