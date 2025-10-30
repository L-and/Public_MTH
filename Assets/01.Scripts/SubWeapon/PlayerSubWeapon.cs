using _01.Scripts.PlayerControll;
using _01.Scripts.SubWeapon.Shield;
using UnityEngine;

namespace _01.Scripts.SubWeapon
{
    /// <summary>
    /// 보조무기 동작에 필요한 필드와 실질적인 보조무기의 기능동작을 호출하는 클래스
    /// </summary>
    public class PlayerSubWeapon : MonoBehaviour
    {
        
        [SerializeField] SubWeaponData data; // 보조무기 속성정보들

        [Header("보조무기 장착 위치")] [SerializeField]
        private Transform equipPosition;
        
        public SubWeaponData SubWeaponData
        {
            get => data;
            set => data = value;
        }

        private PlayerController _playerController;
        public Transform playerCamera;

        /// <summary>
        /// 선택된 보조무기SO의 프리팹 게임오브젝트를 생성하는 메서드
        /// 게임실행 후 1회만 호출해야 함
        /// </summary>
        public void InstantiateSubWeapon()
        {
            // 게임오브젝트 생성, 부모설정
            var swGo = Instantiate(data.prefab, equipPosition.position, equipPosition.rotation);
            swGo.transform.parent = equipPosition;

            // Collider이벤트처리가 필요한 보조무기라면 이벤트를 받기위해 PlayerSubWeapon 을 참조하도록 할당
            if (swGo.TryGetComponent<SubWeaponColliderHandler>(out var colliderHandler))
            {
                colliderHandler.handler = this;
            }
        }
        
        public void ExecuteSubWeapon()
        {
            if (data.behavior)
            {
                data.behavior.Execute(this, _playerController.PlayerCamera, data);
            }
            else
            {
                Debug.Log($"{data.name}에 Behavior이 할당되지 않았습니다.");
            }
        }
    }
}