using System;
using UnityEngine;

namespace _01.Scripts.PlayerControll.Animation
{
    public class CharacterAnimationController : MonoBehaviour
    {
        private Animator _characterAnim;

        /// <summary>
        /// 플레이어의 이동속도
        /// </summary>
        public Vector3 movementVelocity;
        
        /// <summary>
        /// Hashed "Movement".
        /// </summary>
        private static readonly int HashMovement = Animator.StringToHash("Movement");

        private float _dampTimeLocomotion = 0.15f;
        private int _layerActions;

        private void Awake()
        {
            _characterAnim = GetComponent<Animator>();
            
            _layerActions = _characterAnim.GetLayerIndex("Layer Actions");
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
    }
}
