

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossSummon : MonoBehaviour, ISummonAttacker
{
    [System.Serializable]
    public struct SummonPattern
    {
        public string name;
        public float useMinDistance;      // 이 이상일 때 사용
        public float useMaxDistance;      // 이 이하면 사용
        public float windup;              // 소환 전 딜레이(애니/이펙트)
        public float cooldown;

        [Header("기존 근접형 Enemy 프리팹 그대로 사용")]
        public GameObject enemyPrefab;
        public int count;
        public float spawnRadius;         // 보스 주변 원형 반경
        public float navSampleMaxDist;    // NavMesh.SamplePosition 반경

        [Header("소환 후 타깃 주입 옵션")]
        public bool setPlayerAsTarget;
        public string targetFieldOrProperty; // 예: "target" / "player" 등

        public string animTrigger;
        
    }

    [SerializeField] private Animator animator;
    [SerializeField] private SummonPattern[] patterns;

    public bool IsRunning { get; private set; }
    private float[] _nextReady;

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
            return i; // 간단 정책: 조건 맞는 첫 패턴
        }
        return -1;
    }

    private IEnumerator Co_Execute(SummonPattern p, Transform target, int idx)
    {
        IsRunning = true;

        if (animator && !string.IsNullOrEmpty(p.animTrigger))
            animator.SetTrigger(p.animTrigger);

        if (p.windup > 0f)
            yield return new WaitForSeconds(p.windup);

        // 소환 실행
        for (int i = 0; i < p.count; i++)
        {
            Vector3 pos = PickSpawnOnNav(transform.position, p.spawnRadius, p.navSampleMaxDist);
            Quaternion rot = target ? Quaternion.LookRotation((target.position - pos).normalized, Vector3.up) : Quaternion.identity;

            if (p.enemyPrefab)
            {
                GameObject m = Instantiate(p.enemyPrefab, pos, rot);

                if (p.setPlayerAsTarget && target)
                    TryAssignTarget(m, p.targetFieldOrProperty, target);
            }

            yield return null; // 살짝 텀 주기(툭툭 소환)
        }

        _nextReady[idx] = Time.time + p.cooldown;
        IsRunning = false;
    }

    // NavMesh 위 스폰 포인트 샘플
    private static Vector3 PickSpawnOnNav(Vector3 center, float radius, float sampleMaxDist)
    {
        for (int tries = 0; tries < 8; tries++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 candidate = center + new Vector3(r.x, 0, r.y);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, sampleMaxDist, NavMesh.AllAreas))
                return hit.position;
        }
        return center;
    }

     // 가장 범용적인 타깃 주입(필드나 프로퍼티에 Transform 할당 시도)
    private static void TryAssignTarget(GameObject go, string fieldOrProp, Transform target)
    {
        if (string.IsNullOrEmpty(fieldOrProp)) return;

        // 프리팹의 첫 번째 MonoBehaviour를 대상으로 시도
        var mb = go.GetComponent<MonoBehaviour>();
        if (!mb) return;

        var t = mb.GetType();
        var f = t.GetField(fieldOrProp);
        if (f != null && f.FieldType == typeof(Transform)) { f.SetValue(mb, target); return; }

        var p = t.GetProperty(fieldOrProp);
        if (p != null && p.PropertyType == typeof(Transform) && p.CanWrite) { p.SetValue(mb, target); return; }

        // 실패해도 무시(프리팹 AI가 자체적으로 타깃을 찾게 둘 수도 있음)
    }
}
