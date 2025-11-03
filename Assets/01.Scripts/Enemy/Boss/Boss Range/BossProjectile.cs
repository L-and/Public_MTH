using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossProjectile : MonoBehaviour
{
    [Header("투사체 속도 관련")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private bool useGravity = false;
    [SerializeField] private bool faceVelocity = true;

    [Header("유도 성질")]
    [SerializeField] private bool homing = false;
    [SerializeField] private float turnRateDegPerSec = 360f;

    [Header("투사체 데미지")]
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private LayerMask hitMask = ~0;   // 맞출 대상 레이어

    [Header("플레이어 IDamageableZone")]
    [SerializeField] private IDamageableZone playerDamageableZone;

    private Rigidbody rb;
    private Vector3 initialDir = Vector3.forward;
    private Transform owner; 
    private Transform homingTarget;
    private bool initialized;

    public void Init(Vector3 dir, Transform owner, Collider[] ignoreThese = null,
                 Transform homingTarget = null, float? overrideSpeed = null)
    {
        this.owner = owner;
        this.homingTarget = homing ? homingTarget : null;

        if (overrideSpeed.HasValue) speed = overrideSpeed.Value; // ★ 여기서 덮어쓰기

        initialDir = dir.sqrMagnitude > 0.0001f ? dir.normalized : transform.forward;
        initialized = true;

        if (ignoreThese != null && TryGetComponent<Collider>(out var myCol))
            foreach (var c in ignoreThese) if (c && c.enabled) Physics.IgnoreCollision(myCol, c, true);

        ApplyVelocity(initialDir);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = useGravity;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        if (!initialized)
            ApplyVelocity(transform.forward);

        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (homing && homingTarget)
        {
            Vector3 desired = (homingTarget.position + Vector3.up * 1.2f) - transform.position;
            if (desired.sqrMagnitude > 0.0001f)
            {
                desired.Normalize();
                Vector3 current = rb.linearVelocity.sqrMagnitude > 0.0001f
                    ? rb.linearVelocity.normalized
                    : transform.forward;

                float maxRad = Mathf.Deg2Rad * turnRateDegPerSec * Time.fixedDeltaTime;
                Vector3 newDir = Vector3.RotateTowards(current, desired, maxRad, 0f);
                ApplyVelocity(newDir);
            }
        }

        if (faceVelocity && rb.linearVelocity.sqrMagnitude > 0.01f)
            transform.forward = rb.linearVelocity.normalized;
    }

    private void ApplyVelocity(Vector3 dir)
    {
        rb.linearVelocity = dir * speed;  // Unity 6000.x 전용
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"[BossProjectile] collided with {other.collider.name}, tag={other.collider.tag}");


        if (other.collider.CompareTag("Player"))
        {
            playerDamageableZone = other.collider.GetComponentInParent<IDamageableZone>();
            if (playerDamageableZone == null)
            {
                Debug.Log("[BossProjectile] Player hit, but no IDamageableZone found.");
                return;
            }
            Debug.Log("[BossProjectile] Player got hit");
            playerDamageableZone.ApplyHit(10, other.GetContact(0).point, HitZones.Body);
        }
        Debug.Log($"[BossProjectile] HandleHit called by {other.collider.name}");
        HandleHit(other.collider, other.GetContact(0).point);
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     HandleHit(other, transform.position);
    // }
    
    private void HandleHit(Collider hitCol, Vector3 hitPoint)
    {
        // 소유자 무시
        if (owner && hitCol.transform.IsChildOf(owner))
        {
            Debug.Log($"[BossProjectile] Ignored hit on owner: {hitCol.name}");
            return;
        }

        // 맞출 대상만 반응
        if ((hitMask.value & (1 << hitCol.gameObject.layer)) == 0)
        {
            Debug.Log($"[BossProjectile] Ignored layer: {LayerMask.LayerToName(hitCol.gameObject.layer)}");
            return;
        }

        // TODO 김민서: 이부분은 튕겨난 투사체가 적에게 적중했을 때 과열게이지를 충전하는식으로 변경하면 될듯함
        //if (hitCol.TryGetComponent<IDamageableZone>(out var dmg))
        //    dmg.ApplyHit(damage, hitPoint, HitZones.Body);
        
        Debug.Log($"[BossProjectile] Destroying self after hitting {hitCol.name}");
        Destroy(gameObject);
    }
}
