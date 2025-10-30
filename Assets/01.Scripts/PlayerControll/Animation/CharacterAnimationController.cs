using System;
using UnityEngine;

namespace _01.Scripts.PlayerControll.Animation
{
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

        private void Awake()
        {
            _characterAnim = GetComponent<Animator>();
            _playerController = transform.root.gameObject.GetComponent<PlayerController>();
            
            _layerActions = _characterAnim.GetLayerIndex("Layer Actions");
            _layerEmission = _characterAnim.GetLayerIndex("Layer Emission");
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

        public void ReloadAnimation(bool isMagazineEmpty)
        {
            var stateName = isMagazineEmpty ? "Reload Empty" : "Reload";
            _characterAnim.Play(stateName, _layerActions, 0.0f);
        }

        public void EmissionFireAnimation()
        {
            // var stateName = "Fire";
            // _characterAnim.Play(stateName, _layerEmission, 0.0f);
            
            _characterAnim.SetTrigger(EmissionFireAnimHash);
        }

        public void SubWeaponAnimation()
        {
            _characterAnim.SetTrigger(SubWeaponUseAnimHash);
        }
        
        /// <summary>
        /// 애니메이션 클립에서 방출무기를 발사하는 이벤트메서드 [단발] (레일건, 로켓런쳐)
        /// </summary>
        public void OnEmissionFire()
        {
            _playerController.FireEmission();
        }

        public void OnShieldImpact()
        {
            _characterAnim.SetTrigger("Shield Impact");
        }
    }
}
