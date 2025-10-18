using System;
using UnityEngine;

namespace _01.Scripts.PlayerControll.Status
{
    /// <summary>
    /// 플레이어의 모든 스탯데이터와 관련 로직을 처리하는 스크립트입니다.
    /// </summary>
    public class PlayerStatus : MonoBehaviour
    {
        // PlayerController 참조
        private PlayerController _pc;

        [Header("최대속도")] 
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float dashMaxSpeed = 15f;
        [SerializeField] private float slidingMaxSpeed = 25f;
        
        public float CurrentMaxSpeed
                {
                    get
                    {
                        if (_pc.MovementFSM.CurrentState ==
                            _pc.MovementFSM.DashState)
                            return dashMaxSpeed;
                        else if (_pc.MovementFSM.CurrentState ==
                                 _pc.MovementFSM.JumpState ||
                                 _pc.MovementFSM.CurrentState ==
                                 _pc.MovementFSM.SlidingState)
                            return slidingMaxSpeed;
                        else
                            return maxSpeed;
                    }
                }
        
        [Header("가속도(이동조작 반응성)")]
        [SerializeField] private float baseAcc = 10f;
        [SerializeField] private float airAcc = 5f;
        [SerializeField] private float slidingAcc = 5f;

        public float CurrentAcceleration
        {
            get
            {
                if (_pc.MovementFSM.CurrentState == _pc.MovementFSM.JumpState)
                    return airAcc;
                if (_pc.MovementFSM.CurrentState == _pc.MovementFSM.SlidingState)
                    return slidingAcc;

                return baseAcc;
            }
        }
        

        [Header("대쉬상태 관련변수")] 
        [SerializeField] private float dashDurationTime = 0.5f;

        public float DashDurationtime
        {
            get => dashDurationTime;
            private set => dashDurationTime = value;
        }

        // 무적상태인지 판단하는 프로퍼티 TODO 무적의 지속시간을 어떤식으로 판별할지 정의 후 수정해야 함
        public bool IsInvincible
        {
            get
            {
                return (_pc.MovementFSM.CurrentState
                        == _pc.MovementFSM.DashState);
            }
        }

        
        public float BaseAcc => baseAcc;
        public float AirAcc => airAcc;
        
        
        [Header("마우스 감도")]
        [SerializeField] public float mouseSensitivity = 0.5f;
        
        
        [Header("점프력 관련 필드")]
        [SerializeField] private float jumpForce = 10f;

        public float CurrentJumpForce => jumpForce;

        # region Unity Methods

        private void Awake()
        {
            _pc = GetComponent<PlayerController>();
        }

        # endregion
    }
}