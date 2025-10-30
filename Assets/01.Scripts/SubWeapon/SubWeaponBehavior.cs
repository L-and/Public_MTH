using UnityEngine;

namespace _01.Scripts.SubWeapon
{
    /// <summary>
    /// 보조무기의 동작기능을 정의하기위한 SO 추상클래스
    /// </summary>
    public abstract class SubWeaponBehavior : ScriptableObject
    {
        /// <summary>
        /// 보조무기의 동작기능을 구현하는 추상 메서드
        /// </summary>
        /// <param name="handler">보조무기의 동작기능을 실행하는 컴포넌트 (플레이어가 직접실행 || 애니메이션 이벤트로 실행되는 방식)</param>
        /// <param name="playerCamera">플레이어의 1인칭 카메라 트랜스폼</param>
        /// <param name="data">보조무기의 데이터</param>
        public abstract void Execute(PlayerSubWeapon handler, Transform playerCamera, SubWeaponData data);

        /// <summary>
        /// 보조무기의 게임오브젝트에 충돌감지시(Collision) 처리될 기능을 작성하는 가상메서드
        /// </summary>
        /// <param name="other">충돌대상</param>
        /// <param name="handler"></param>
        /// <param name="playerCamera"></param>
        /// <param name="data"></param>
        public virtual void ExecuteOnCollision(Collision other, PlayerSubWeapon handler, Transform playerCamera, SubWeaponData data) { }

        /// <summary>
        /// 보조무기의 게임오브젝트에 충돌감지시(Trigger) 처리될 기능을 작성하는 가상메서드
        /// </summary>
        /// <param name="other"></param>
        /// <param name="handler"></param>
        /// <param name="playerCamera"></param>
        /// <param name="data"></param>
        public virtual void ExecuteOnTrigger(Collider other, PlayerSubWeapon handler, Transform playerCamera, SubWeaponData data) { }
        
    }
}