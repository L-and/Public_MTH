using System.Collections;
using _01.Scripts.PlayerControll;
using _01.Scripts.Weapons_ScriptableObjects.SubWeapon.Shield;
using UnityEngine;

namespace _01.Scripts.Weapons_ScriptableObjects.SubWeapon
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

        public PlayerController playerController;
        public Transform playerCamera;

        // 보조무기 오브젝트
        private GameObject _subWeaponGO;
        [SerializeField] private bool isCooldown;

        /// <summary>
        /// 선택된 보조무기SO를 사용해 초기설정을 진행하는 메서드
        /// 게임실행 후 1회만 호출해야 함
        /// </summary>
        public void InitializeSubWeapon(PlayerController playerController)
        {
            this.playerController = playerController;
            playerCamera = playerController.PlayerCamera;

            // 보조무기 모델 장착위치 설정
            if (equipPosition == null)
            {
                equipPosition = GameObject.FindGameObjectWithTag("LeftHand Position").transform;
            }
            
            // 보조무기 모델 생성 및 비활성화 (사용할때만 활성화하는 식으로 사용)
            _subWeaponGO = Instantiate(data.prefab, equipPosition.position, equipPosition.rotation);
            _subWeaponGO.transform.parent = equipPosition;
            _subWeaponGO.SetActive(false);
            
            // Collider이벤트처리가 필요한 보조무기라면 이벤트를 받기위해 PlayerSubWeapon 을 참조하도록 할당
            if (_subWeaponGO.TryGetComponent<SubWeaponColliderHandler>(out var colliderHandler))
            {
                colliderHandler.handler = this;
            }

            isCooldown = false;
        }
        
        /// <summary>
        /// 보조무기 사용
        /// </summary>
        public void ExecuteSubWeapon()
        {
            // 쿨타임 검사
            if (isCooldown) return;
            
            // 보조무기 사용 애니메이션 실행
            playerController.CharacterAnimController.SubWeaponAnimation();
            _subWeaponGO.SetActive(true);
            
            Invoke(nameof(EndUsingSubweapon), data.useDuration);
        }

        private void EndUsingSubweapon()
        {
            // 쉴드 사용종료 애니메이션 트리거
            playerController.CharacterAnimController.OnShieldUsingDone();
            _subWeaponGO.SetActive(false);
            isCooldown = true;

            StartCoroutine(CoolDownRoutine());
        }
        
        IEnumerator CoolDownRoutine()
        {
            yield return new WaitForSeconds(data.coolDown);
            isCooldown = false;
        }
    }
}