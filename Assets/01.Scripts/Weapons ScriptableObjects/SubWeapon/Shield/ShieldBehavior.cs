using _01.Scripts.Enums;
using _01.Scripts.Utils;
using UnityEngine;

namespace _01.Scripts.Weapons_ScriptableObjects.SubWeapon.Shield
{
    [CreateAssetMenu(fileName = "Shield Behavior", menuName = "SubWeapon/Behavior/Shield Behavior")]
    public class ShieldBehavior : SubWeaponBehavior
    {
        public override void Execute(PlayerSubWeapon handler, Transform playerCamera, SubWeaponData data)
        {
   
        }

        public override void ExecuteOnTrigger(Collider other, PlayerSubWeapon handler, Transform playerCamera, SubWeaponData data)
        {
            StyleEventManager.TriggerStyleAction(EStyleType.Parry);
            
            // 쉴드 튕겨내기 애니메이션 실행
            handler.playerController.CharacterAnimController.OnShieldImpact();
            
            var projectileGo = other.transform.root.gameObject;
            // 플레이어의 투사체임을 명시하기위해 레이어 변경
            LayerChanger.SetLayerRecursively(projectileGo, Layers.Projectile);
            
            if (!projectileGo.TryGetComponent<RangeProjectile>(out var rangeProjectile))
            {
                Debug.LogWarning("[보조무기:방패] 튕겨낸 투사체에 [RangeProjectile] 컴포넌트가 없습니다.");
                return;
            }

            // 투사체가 공격할 레이어마스크 변경
            rangeProjectile.HitMask = 1 << LayerMask.NameToLayer("Enemy");
            // 플레이어의 에임이 바라보는 방향으로 투사체를 튕겨냄
            var dir = (handler.playerCamera.forward * 1000 - projectileGo.transform.position).normalized;
            rangeProjectile.ChangeDirection(dir);
            
        }
    }
}