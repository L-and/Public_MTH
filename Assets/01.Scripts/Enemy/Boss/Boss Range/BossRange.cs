using System.Collections;
using UnityEngine;

public class BossRange : MonoBehaviour, IRangedAttacker
{
    [System.Serializable]
    public struct RangedPattern
    {
        public string name;
        public float useMinDistance;
        public float useMaxDistance;
        public float windup;
        public float cooldown;

        public Transform firePoint;
        public GameObject projectilePrefab;
        public float damage;
        public float speed;

        public string animTrigger;
    }

    [SerializeField] private Animator animator;
    [SerializeField] private RangedPattern[] patterns;

    private float[] _nextReady;
    public bool IsRunning { get; private set; }

    private void Awake()
    {
        _nextReady = new float[patterns.Length];
    }

    public bool CanUse(Transform target, float distance)
    {
        float now = Time.time;
        for (int i = 0; i < patterns.Length; i++)
        {
            var p = patterns[i];
            if (distance >= p.useMinDistance && distance <= p.useMaxDistance && now >= _nextReady[i])
                return true;
        }
        return false;
    }

    public void Execute(Transform target)
    {
        if (IsRunning) return;
        int idx = SelectPattern(target);
        if (idx == -1) return;
        StartCoroutine(Co_Execute(patterns[idx], target, idx));
    }

    private int SelectPattern(Transform target)
    {
        float dist = Vector3.Distance(transform.position, target.position);
        float now = Time.time;
        for (int i = 0; i < patterns.Length; i++)
        {
            var p = patterns[i];
            if (dist < p.useMinDistance || dist > p.useMaxDistance) continue;
            if (now < _nextReady[i]) continue;
            return i;
        }
        return -1;
    }

    private IEnumerator Co_Execute(RangedPattern p, Transform target, int idx)
    {
        IsRunning = true;

        if (animator && !string.IsNullOrEmpty(p.animTrigger))
            animator.SetTrigger(p.animTrigger);

        if (p.windup > 0) yield return new WaitForSeconds(p.windup);

        // ── 직선탄 1회 발사(기본형) ──
        if (p.firePoint && p.projectilePrefab)
        {
            Vector3 dir = (target.position + Vector3.up * 1.0f - p.firePoint.position).normalized;
            var go = Instantiate(p.projectilePrefab, p.firePoint.position, Quaternion.LookRotation(dir));
            if (go.TryGetComponent<Rigidbody>(out var rb))
                rb.linearVelocity = dir * Mathf.Max(1f, p.speed);

            // 네 프로젝트 투사체가 damage 필드를 갖는다면 주입
            var proj = go.GetComponent<RangeProjectile>();
            if (proj != null)
            {
                // SetDamage 메서드가 있다면 사용, 없다면 public field 직접 할당
                var mi = typeof(RangeProjectile).GetMethod("SetDamage");
                if (mi != null) mi.Invoke(proj, new object[] { p.damage });
                else
                {
                    var fi = typeof(RangeProjectile).GetField("damage");
                    if (fi != null) fi.SetValue(proj, p.damage);
                }
            }
        }

        _nextReady[idx] = Time.time + p.cooldown;
        IsRunning = false;
    }
}
