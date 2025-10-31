using System.Collections;
using _01.Scripts.PlayerControll;
using UnityEngine;

public class BossMelee : MonoBehaviour, IMeleeAttacker
{
    [System.Serializable]
    public struct MeleePattern
    {
        public string name;
        public float useMinDistance;     // 최소 거리
        public float useMaxDistance;     // 최대 거리
        public float damage;
        public float windup;             // 공격 전 딜레이
        public float cooldown;
        public float knockbackDistance;
        public float knockbackImpulse;
        public string animTrigger;       // 애니메이션 트리거 이름
    }

    [SerializeField] private Animator animator;
    [SerializeField] private MeleePattern[] patterns;

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

        StartCoroutine(Co_ExecutePattern(patterns[idx], target, idx));
    }

    private int SelectPattern(Transform target)
    {
        float dist = Vector3.Distance(transform.position, target.position);
        float now = Time.time;
        int chosen = -1;
        float shortest = float.MaxValue;

        for (int i = 0; i < patterns.Length; i++)
        {
            var p = patterns[i];
            if (dist < p.useMinDistance || dist > p.useMaxDistance) continue;
            if (now < _nextReady[i]) continue;
            if (p.useMaxDistance < shortest)
            {
                shortest = p.useMaxDistance;
                chosen = i;
            }
        }
        return chosen;
    }

    private IEnumerator Co_ExecutePattern(MeleePattern p, Transform target, int idx)
    {
        IsRunning = true;

        if (animator && !string.IsNullOrEmpty(p.animTrigger))
            animator.SetTrigger(p.animTrigger);

        if (p.windup > 0)
            yield return new WaitForSeconds(p.windup);

        // 공격 판정
        DealDamageToPlayer(target, p.damage, target.position);
        Knockback(target, p.knockbackDistance, p.knockbackImpulse);

        _nextReady[idx] = Time.time + p.cooldown;
        IsRunning = false;
    }

    // ─────────────────────────────────────────────
    // Damage Utility
    private void DealDamageToPlayer(Transform tf, float dmg, Vector3 point)
    {
        var dz = tf.GetComponentInParent<IDamageableZone>() ?? tf.GetComponent<IDamageableZone>();
        if (dz != null)
        {
            dz.ApplyHit(dmg, point, HitZones.Body);
            return;
        }

        var pc = tf.GetComponentInParent<PlayerController>() ?? tf.GetComponent<PlayerController>();
        if (pc != null)
            pc.ApplyDamage(dmg);
    }

    private void Knockback(Transform tf, float distance, float impulse)
    {
        Vector3 dir = (tf.position - transform.position).normalized;

        if (tf.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(dir * impulse, ForceMode.Impulse);
        }
        else if (tf.TryGetComponent<CharacterController>(out var cc))
        {
            StartCoroutine(Co_PushCharacter(cc, dir, distance, 0.3f));
        }
        else
        {
            tf.position += dir * distance;
        }
    }

    private IEnumerator Co_PushCharacter(CharacterController cc, Vector3 dir, float distance, float duration)
    {
        float t = 0;
        float speed = distance / duration;
        while (t < duration)
        {
            cc.Move(dir * speed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
