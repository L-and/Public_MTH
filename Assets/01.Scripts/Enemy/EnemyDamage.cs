using UnityEngine;
using UnityEngine.AI;

public class EnemyDamage : MonoBehaviour, IDamageableZone
{
    [Header("Rules")]
    [Tooltip("몸통이 맞아야 하는 횟수")]
    [SerializeField] private int bodyHitsToDie = 3;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTrigger = "Die";

    [Header("Cleanup (Alive → Dead 전환 시 끌 것들)")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Collider[] collidersToDisable;          // 루트/바디용(살아있을 때) 콜라이더
    [SerializeField] private MonoBehaviour[] componentsToDisable;    // AI/공격 스크립트 등
    [SerializeField] private float extraStayTime = 2.0f;             // 애니 사망 루트일 때 시체 유지시간

    [Header("Ragdoll")]
    [SerializeField] private bool useRagdoll = true;
    [SerializeField] private Animator animForRagdoll;     // 보통 animator와 동일
    [SerializeField] private Rigidbody[] ragdollBodies;   // Pelvis/Spine/Head/팔/다리 등 본 RB
    [SerializeField] private Collider[] ragdollColliders; // 본 콜라이더들(평소 OFF, 사망 시 ON)
    [SerializeField] private float deathImpulse = 50f;    // 튕기는 힘 세기

    [Header("Corpse / Collision")]
    [SerializeField] private bool changeLayerOnDeath = true;
    [SerializeField] private string corpseLayer = "Corpse";    // Projectile과 충돌하지 않는 레이어
    [SerializeField] private float corpseDisableDelay = 1.2f;  // 튕긴 뒤 래그돌 정리까지 지연

    private int bodyHitCount;
    public bool IsDead { get; private set; }

    // 마지막 피격 정보(튕김 방향/위치)
    private Vector3 lastHitPoint;
    private Vector3 lastHitDir = Vector3.forward;

    // ────────────────────────────────────────────────────────────────────────────
    #region Damage Entry
    public void ApplyHit(float rawDamage, Vector3 hitPoint, HitZones zone)
    {
        if (IsDead) return;

        // 피격 위치 저장(사망 시 사용할 수 있음)
        lastHitPoint = hitPoint;

        if (zone == HitZones.Head)
        {
            OnHeadShotDead();
            Kill();
            return;
        }

        bodyHitCount++;
        if (bodyHitCount >= bodyHitsToDie)
            OnBodyShotDead();
            Kill();
    }

    /// <summary>Projectile에서 충돌 직전에 호출. 총알 진행 방향을 넘겨줘야 자연스러운 튕김.</summary>
    public void CacheImpact(Vector3 point, Vector3 direction)
    {
        lastHitPoint = point;
        if (direction.sqrMagnitude > 0.0001f)
            lastHitDir = direction.normalized;
    }

    public void OnHeadShotDead()
    {
        StyleManager styleManager = GameObject.Find("Style Panel").GetComponent<StyleManager>();
        if (styleManager == null) return;
        styleManager.CreateNewStyle(0);
        styleManager.CreateNewStyle(5);
    }
    public void OnBodyShotDead()
    {
        StyleManager styleManager = GameObject.Find("Style Panel").GetComponent<StyleManager>();
        if (styleManager == null) return;
        styleManager.CreateNewStyle(0);
    }
    #endregion

    // ────────────────────────────────────────────────────────────────────────────
    #region Death / Ragdoll
    private void Kill()
    {
        if (IsDead) return;
        IsDead = true;

        // 이동/행동 차단
        if (agent) agent.enabled = false;
        foreach (var c in componentsToDisable) if (c) c.enabled = false;
        foreach (var col in collidersToDisable) if (col) col.enabled = false;

        // 더 이상 데미지 전달되지 않도록 BodyPart 비활성화
        foreach (var bp in GetComponentsInChildren<BodyPart>(true)) bp.enabled = false;

        // 시체 레이어로 전환(총알과 충돌 차단)
        if (changeLayerOnDeath)
        {
            int corpse = LayerMask.NameToLayer(corpseLayer);
            if (corpse >= 0) SetLayerRecursively(gameObject, corpse);
        }

        if (useRagdoll)
        {
            // 래그돌 ON
            EnableRagdoll(true);

            // 가장 가까운 본에 힘 적용
            var bone = FindClosestBone(lastHitPoint);
            if (bone == null && ragdollBodies != null && ragdollBodies.Length > 0)
                bone = ragdollBodies[0];

            if (bone != null)
            {
                Vector3 dir = (lastHitDir.sqrMagnitude > 0.01f ? lastHitDir : transform.forward).normalized;
                Vector3 forcePoint = lastHitPoint;
                Debug.Log($"💥 Applying force to {bone.name} at {forcePoint}");
                bone.AddForceAtPosition(dir * deathImpulse, forcePoint, ForceMode.Impulse);
            }

            // 잠깐 튕긴 뒤 래그돌 정리(충돌 제거/정지)
            StartCoroutine(DisableRagdollAfter(corpseDisableDelay));

            // 일정 시간 뒤 시체 삭제(원하면 풀 시스템/풀링으로 교체)
            Destroy(gameObject, 6f);
            return;
        }

        // 애니메이션 사망 루트
        if (animator)
        {
            animator.SetTrigger(deathTrigger);
            StartCoroutine(WaitDeathAndDespawn());
        }
        else Destroy(gameObject, 2f);
    }

    private void EnableRagdoll(bool on)
    {
        if (animForRagdoll) animForRagdoll.enabled = !on;
        if (ragdollBodies != null)
            foreach (var rb in ragdollBodies) if (rb) { rb.isKinematic = !on; rb.useGravity = on; }
        if (ragdollColliders != null)
            foreach (var col in ragdollColliders) if (col) col.enabled = on;
    }

    private Rigidbody FindClosestBone(Vector3 point)
    {
        if (ragdollBodies == null || ragdollBodies.Length == 0) return null;
        Rigidbody best = null; float bestDist = float.PositiveInfinity;
        foreach (var rb in ragdollBodies)
        {
            if (!rb) continue;
            float d = (rb.worldCenterOfMass - point).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = rb; }
        }
        return best;
    }

    private System.Collections.IEnumerator DisableRagdollAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 물리 정지
        if (ragdollBodies != null)
        {
            foreach (var rb in ragdollBodies)
            {
                if (!rb) continue;
#if UNITY_6000_0_OR_NEWER
                rb.linearVelocity = Vector3.zero;
#else
                rb.velocity = Vector3.zero;
#endif
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // 충돌 제거(시체가 총알/플레이어와 계속 부딪히지 않도록)
        if (ragdollColliders != null)
            foreach (var col in ragdollColliders) if (col) col.enabled = false;
    }
    #endregion

    // ────────────────────────────────────────────────────────────────────────────
    #region Animator Death (non-ragdoll)
    // Death 클립 마지막 프레임에 Animation Event로 연결 가능
    public void AE_DeathEnd()
    {
        Destroy(gameObject, extraStayTime);
    }

    private System.Collections.IEnumerator WaitDeathAndDespawn()
    {
        yield return null;
        if (animator != null)
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            float len = info.length > 0 ? info.length : 1.0f;
            yield return new WaitForSeconds(len + extraStayTime);
        }
        Destroy(gameObject);
    }
    #endregion

    // ────────────────────────────────────────────────────────────────────────────
    #region Utils
    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform t in go.transform) SetLayerRecursively(t.gameObject, layer);
    }
    #endregion
}
