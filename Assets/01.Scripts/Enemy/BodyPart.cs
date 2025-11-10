using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BodyPart : MonoBehaviour
{
    [Header("데미지 부위 설정")]
    public HitZones zone = HitZones.Body;        // 약점은 Weak, 나머진 Body
    [SerializeField] private LayerMask hitFrom; // Player 탄/근접이 속한 레이어
    [SerializeField] private MonoBehaviour owner;
    private IDamageableZone _damageable; // 루트(IDamageableZone) 참고를 외부에서 주입받음

    // 루트가 이걸 호출해서 주입
    public void Initialize(IDamageableZone damageable)
    {
        _damageable = damageable;
        owner = damageable as MonoBehaviour; // 디버그용 확인 
    }

    private void Awake()
    {
        // inspector에 직접 넣어둔 경우용
        if (_damageable == null && owner is IDamageableZone dz) _damageable = dz;
    }

    // 레일건 혹은 레이캐스트에서 쓸 얇은 API
    public IDamageableZone GetOwner() => _damageable;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = false;                  // 반드시 Trigger
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{name} < {collision.collider.name}({collision.gameObject.layer})");
        // 공격물이 지정 레이어가 아니면 무시 (선택)
        if ((hitFrom.value & (1 << collision.gameObject.layer)) == 0)
            return;

        // 공격 정보 가져오기
        if (!collision.gameObject.TryGetComponent<HitSource>(out var src))
            return;

        // 부모에게 데미지 전달
        var target = GetComponentInParent<IDamageableZone>();
        if (target != null)
        {
            Vector3 hitPoint = transform.position; // fallback

            if (collision.contacts != null && collision.contacts.Length > 0)
            {
                hitPoint = collision.contacts[0].point;
            }
            else
            {
                Debug.LogWarning($"⚠️ No contact point for {name}, fallback to transform.position");
            }
            //target.ApplyHit(src.damage, hitPoint, zone);
            target.ApplyHit(src.GetDamage(), hitPoint, zone);
            // Vector3 hitPoint = collision.contacts.Length > 0 ?
            //     collision.contacts[0].point : transform.position;
            // target.ApplyHit(src.damage, hitPoint, zone);
        }

        // 총알 제거
        Destroy(collision.gameObject);
    }
}