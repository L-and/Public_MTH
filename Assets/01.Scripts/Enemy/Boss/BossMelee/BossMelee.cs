using System.Collections;
using _01.Scripts.PlayerControll;
using UnityEngine;

public class BossMelee : MonoBehaviour, IMeleeAttacker
{
    [System.Serializable]
    public struct MeleePattern
    {
        public string name;
        public float useMinDistance;
        public float useMaxDistance;
        public float damage;
        public float windup;             // 공격 전 딜레이 (예: 손 모으는 구간)
        public float cooldown;
        public float knockbackDistance;  // (필요시) 후처리로 남겨둠
        public float knockbackImpulse;   // (필요시)
        public string animTrigger;

        [Header("Hitboxes opened by animation events")]
        public MeleeAttack[] hitboxes;   // 이 패턴에서 열리는 히트박스들(지면 가시 등)
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

        // 1) 애니 트리거
        if (animator && !string.IsNullOrEmpty(p.animTrigger))
            animator.SetTrigger(p.animTrigger);

        // 2) 윈드업(모션 예열)
        if (p.windup > 0f)
            yield return new WaitForSeconds(p.windup);

        // 3) 이번 스윙에서 사용할 히트박스들 준비
        if (p.hitboxes != null)
        {
            foreach (var hb in p.hitboxes)
            {
                if (!hb) continue;
                hb.SetDamage(p.damage);  // 패턴 데미지 주입
                hb.StartSwing();         // 쿨다운 스탬프 및 1회 히트 초기화
                // 히트박스의 실제 on/off는 애니메이션 이벤트에서 OnHitboxOpen/Close가 수행
            }
        }

        // 4) (선택) 넉백 등 후처리가 '피격 시' 일어나야 한다면,
        //    MeleeAttack의 OnTrigger에서 IDamageableZone/PlayerController를 찾을 때 같이 처리하는 게 더 정확함.
        //    (한 번만 데미지/넉백: MeleeAttack이 hasHit로 1회 제한 이미 있음) :contentReference[oaicite:2]{index=2}

        // 5) 패턴 쿨타임
        _nextReady[idx] = Time.time + p.cooldown;

        IsRunning = false;
    }
}
