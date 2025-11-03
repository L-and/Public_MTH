using UnityEngine;

namespace _01.Scripts.Weapons_ScriptableObjects.SubWeapon.Shield
{
    /// <summary>
    /// 보조무기의 Collider 이벤트호출을 감지하여
    /// Handler인 PlayerSubWeapon의 처리로직을 호출하는 클래스
    /// </summary>
    public class SubWeaponColliderHandler : MonoBehaviour
    {
        public PlayerSubWeapon handler;
        
        private void OnTriggerEnter(Collider other)
        {
            // 적과 보조무기(방패, 그랩)이 충돌 시 SubWeaponData.behavior의 정의사항을 실행
            handler?.SubWeaponData.behavior.ExecuteOnTrigger(other, handler, handler.playerCamera, handler.SubWeaponData);
        }
    }
}
