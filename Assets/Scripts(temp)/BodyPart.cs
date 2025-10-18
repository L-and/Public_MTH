using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BodyPart : MonoBehaviour
{
    public HitZones zone = HitZones.Body;        // 머리는 Head, 나머진 Body
    [SerializeField] private LayerMask hitFrom; // Player 탄/근접이 속한 레이어

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = false;                  // 반드시 Trigger
    }

    private void OnCollisionEnter(Collision collision)
    {
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
            Vector3 hitPoint = collision.contacts.Length > 0 ?
                collision.contacts[0].point : transform.position;
            target.ApplyHit(src.damage, hitPoint, zone);
        }

        // 총알 제거
        Destroy(collision.gameObject);
    }
}