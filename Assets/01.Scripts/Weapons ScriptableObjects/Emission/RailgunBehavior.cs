using System;
using System.Collections.Generic;
using _01.Scripts.Enums;
using LaserBeams;
using UnityEngine;

namespace _01.Scripts.Weapons_ScriptableObjects.Emission
{
    [CreateAssetMenu(fileName = "Railgun Behavior", menuName = "Emission/Behavior/Railgun Behavior")]
    public class RailgunBehavior : EmissionBehavior
    {
        [Tooltip("사거리")] 
        public float range = 100f;
        [Tooltip("적 레이어마스크")]
        public LayerMask enemyMask;

        [Header("레일건 총알 (효과만 보여지고 데미지를 주는 기능은 없음)")]
        public GameObject railgunProjectile;
        
        private GameObject _muzzleEffectInstance;
        public float projectileImpulse = 30;
        
        public override void Execute(PlayerEmission handler, Transform playerCamera, EmissionAbilityData data)
        {
            Debug.Log("레일건 발사");
            
            // ######### 총알 이팩트 생성
            
            Transform muzzleTransform = handler.firePosition; // 레일건 발사위치 Transform
            
            // 총알이 발사될 회전값 계산
            Quaternion fireRotation = Quaternion.LookRotation(playerCamera.position + playerCamera.forward * 1000.0f - muzzleTransform.position);
            if (Physics.Raycast(new Ray(playerCamera.position, playerCamera.forward),
                    out RaycastHit aimHit, range, enemyMask))
            {
                fireRotation = Quaternion.LookRotation(aimHit.point - muzzleTransform.position);
            }
            
            
            var projectile = Instantiate(railgunProjectile,  muzzleTransform.position, fireRotation);
            
            // 발사 물리 적용
            projectile.transform.position = muzzleTransform.position;
            projectile.transform.rotation = fireRotation;
            projectile.GetComponent<Rigidbody>().linearVelocity = projectile.transform.forward * projectileImpulse;
            
            // ######### 총구 이팩트 생성
            
            var explosionScript = muzzleTransform.GetComponentInChildren<ExplosionScript>();
            _muzzleEffectInstance = explosionScript.gameObject;
            
            explosionScript.StartEffect();
            
            // ########### 데미지적용 계산
            
            // RaycastAll을 사용해 레일건을 발사하여 적중된 적들의 히트박스 Collider를 가져옴
            RaycastHit[] hits = Physics.RaycastAll(muzzleTransform.position, playerCamera.forward, range, enemyMask);
            
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
                
                // 스타일액션 이벤트 호출 (레일건처치)
                StyleEventManager.TriggerStyleAction(EStyleType.RailgunKill);
                
                Debug.Log($"[{hit.collider.gameObject.transform.root.name}]공격 성공!");
            }
            
            // 스타일액션 이벤트 호출 (동시처치)
            for (var i = 0; i < damagedTarget.Count; i++)
            {
                StyleEventManager.TriggerStyleAction(EStyleType.MultiKill);
            }
            
        }
    }
}