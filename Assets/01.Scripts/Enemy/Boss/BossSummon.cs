

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

        public GameObject minionPrefab;
        public int count;
        public float spawnRadius;         // 보스 주변 원형 반경
        public float navSampleMaxDist;    // NavMesh.SamplePosition 반경

        public string animTrigger;        // 소환 모션 트리거
        public bool facePlayerOnCast;     // 시전 중 플레이어 바라보기

        // 소환 직후 세팅(선택)
        public bool assignPlayerAsTarget; // 소환수 AI에 플레이어 바로 박아두기
        public string aiTargetFieldName;  // 예: "target" 혹은 "player"
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
            Vector3 pos = PickSpawnPointOnNav(transform.position, p.spawnRadius, p.navSampleMaxDist);
            Quaternion rot = p.facePlayerOnCast && target
                ? Quaternion.LookRotation((target.position - pos).normalized, Vector3.up)
                : Quaternion.identity;

            if (p.minionPrefab)
            {
                GameObject m = Instantiate(p.minionPrefab, pos, rot);

                if (p.assignPlayerAsTarget && target)
                {
                    // 매우 범용적인 리플렉션 세팅 (네 팀 AI 스크립트 이름/필드명에 맞게 바꿔도 됨)
                    var comp = m.GetComponent<MonoBehaviour>();
                    if (comp != null && !string.IsNullOrEmpty(p.aiTargetFieldName))
                    {
                        var field = comp.GetType().GetField(p.aiTargetFieldName);
                        if (field != null && field.FieldType == typeof(Transform))
                            field.SetValue(comp, target);
                        else
                        {
                            var prop = comp.GetType().GetProperty(p.aiTargetFieldName);
                            if (prop != null && prop.PropertyType == typeof(Transform) && prop.CanWrite)
                                prop.SetValue(comp, target);
                        }
                    }
                }
            }

            // 살짝 지연을 두면 “한꺼번에 툭”이 아니라 “툭툭” 느낌 가능
            yield return null;
        }

        _nextReady[idx] = Time.time + p.cooldown;
        IsRunning = false;
    }

    // NavMesh 위 스폰 포인트 샘플
    private static Vector3 PickSpawnPointOnNav(Vector3 center, float radius, float sampleMaxDist)
    {
        for (int tries = 0; tries < 8; tries++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 candidate = center + new Vector3(r.x, 0, r.y);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, sampleMaxDist, NavMesh.AllAreas))
                return hit.position;
        }
        // 실패 시 보스 발밑
        return center;
    }
}
