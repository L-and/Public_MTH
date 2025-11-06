using System;
using UnityEngine;

namespace _01.Scripts.PlayerControll.Animation
{
    /// <summary>
    /// 플레이어 애니메이터의 애니메이션 실행, 트리거 조작을 담당하는 클래스
    /// </summary>
    public class CharacterAnimationController : MonoBehaviour
    {
        private PlayerController _playerController;
        private Animator _characterAnim;

        /// <summary>
        /// 플레이어의 이동속도
        /// </summary>
        public Vector3 movementVelocity;
        
        /// <summary>
        /// Hashed "Movement".
        /// </summary>
        private static readonly int HashMovement = Animator.StringToHash("Movement");

        private static readonly int EmissionFireAnimHash = Animator.StringToHash("Emission Fire");
        private static readonly int SubWeaponUseAnimHash = Animator.StringToHash("SubWeapon Use");

        private float _dampTimeLocomotion = 0.15f;
        private int _layerActions;
        private int _layerEmission;
        private int _layerOverlay;

        private void Awake()
        {
            _characterAnim = GetComponent<Animator>();
            _playerController = transform.root.gameObject.GetComponent<PlayerController>();
            
            _layerActions = _characterAnim.GetLayerIndex("Layer Actions");
            _layerEmission = _characterAnim.GetLayerIndex("Layer Emission");
            _layerOverlay = _characterAnim.GetLayerIndex("Layer Overlay");
        }

        private void Update()
        {
            UpdateAnimator();
        }

        /// <summary>
        /// 애니메이터의 파라미터를 프레임마다 업데이트
        /// </summary>
        private void UpdateAnimator()
        {
            
            _characterAnim.SetFloat(HashMovement, movementVelocity.magnitude, _dampTimeLocomotion, Time.deltaTime);
        }

        public void FireAnimation(bool hasAmmo)
        {
            var stateName = hasAmmo ? "Fire" : "Fire Empty";
            _characterAnim.Play(stateName, _layerOverlay, 0.0f);

        }
        
        public void ReloadAnimation(bool isMagazineEmpty)
        {
            var stateName = isMagazineEmpty ? "Reload Empty" : "Reload";
            _characterAnim.Play(stateName, _layerActions, 0.0f);
        }

        public void EmissionFireAnimation()
        {
            _characterAnim.SetTrigger(EmissionFireAnimHash);
        }

        public void SubWeaponAnimation()
        {
            _characterAnim.SetTrigger(SubWeaponUseAnimHash);
        }

        /// <summary>
        /// 쉴드 튕겨내기 애니메이션 트리거
        /// </summary>
        public void OnShieldImpact()
        {
            _characterAnim.SetTrigger("Shield Impact");
        }

        /// <summary>
        /// 쉴드 사용종료 애니메이션 트리거
        /// </summary>
        public void OnShieldUsingDone()
        {
            _characterAnim.SetTrigger("Shield Using Done");
        }
    }
}
