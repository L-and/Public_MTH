using System.Collections.Generic;
using UnityEngine;

namespace _01.Scripts.Emission
{
    [CreateAssetMenu(fileName = "Railgun Behavior", menuName = "Emission/Behavior/Railgun Behavior")]
    public class RailgunBehavior : EmissionBehavior
    {
        [Tooltip("사거리")] 
        public float range = 100f;
        [Tooltip("적 레이어마스크")]
        public LayerMask enemyMask = LayerMask.GetMask("Enemy"); // TODO 하드코딩 수정
        
        public override void Execute(PlayerEmission handler, Transform playerCamera, EmissionAbilityData data)
        {
            Transform fireTransform = handler.firePosition; // 레일건 발사위치
            
            // RaycastAll을 사용해 레일건을 발사하여 적중된 적들의 히트박스 Collider를 가져옴
            RaycastHit[] hits = Physics.RaycastAll(fireTransform.position, playerCamera.forward, range, enemyMask);
            Debug.DrawRay(fireTransform.position, playerCamera.forward * range, Color.red, 2f);
            
            // 데미지를 적용한 적들을 저장할 해쉬셋
            var damagedTarget = new HashSet<IDamageableZone>();
            
            foreach (var hit in hits)
            {
                // 히트박스에서 BodyPart 컴포넌트를 가져옴
                if (!hit.collider.TryGetComponent<BodyPart>(out var bodyPart))
                {
                    Debug.LogWarning($"[{hit.collider.name}]충돌된 히트박스에 BodyPart가 없음");
                    continue;
                }


                IDamageableZone damageableZone = bodyPart.GetOwner();
                if (damageableZone == null)
                {
                    Debug.LogWarning($"[{hit.collider.name}]충돌된 적에게 IDamageableZone가 없음");
                    continue;
                }

                // 데미지 중복적용 방지를 위해 공격한 적인지 검색
                if (damagedTarget.Contains(damageableZone))
                {
                    Debug.Log("중복계산된 적입니다");
                    continue;
                }
                
                damageableZone.ApplyHit(data.damage, hit.point, bodyPart.zone); // 적에게 데미지 적용
                damagedTarget.Add(damageableZone); // 같은적에게 중복계산 방지를위해 해시셋에 저장
                
                Debug.Log($"[{hit.collider.gameObject.transform.root.name}]공격 성공!");
            }
        }
    }
}