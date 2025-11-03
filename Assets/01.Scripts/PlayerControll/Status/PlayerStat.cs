using System;
using UnityEngine;

namespace _01.Scripts.PlayerControll.Status
{
    /// <summary>
    /// 플레이어의 스탯데이터를 저장하고 값을 조작하는 스크립트입니다.
    /// </summary>
    [Serializable]
    public class PlayerStat
    {
        // 복사 생성자
        public PlayerStat(PlayerStat other)
        {
            this.hp = new Stat(other.hp);
            this.stamina = new Stat(other.stamina);
            this.overheat = new Stat(other.overheat);
            this.bulletDamage = other.bulletDamage;
            this.maxSpeed = other.maxSpeed;
            this.dashMaxSpeed = other.dashMaxSpeed;
            this.slidingMaxSpeed = other.slidingMaxSpeed;
            this.jumpMaxSpeed = other.jumpMaxSpeed;
            this.baseAcc = other.baseAcc;
            this.airAcc = other.airAcc;
            this.slidingAcc = other.slidingAcc;
            this.dashPower = other.dashPower;
            this.slidingPower = other.slidingPower;
            this.dashDurationTime = other.dashDurationTime;
            this.mouseSensitivity = other.mouseSensitivity;
            this.jumpForce = other.jumpForce;
        }
        
        [Header("체력/스테미너/과열")] 
        [SerializeField] public Stat hp;
        [SerializeField] public Stat stamina;
        [SerializeField] public Stat overheat;

        [Header("공격력")] 
        [SerializeField] public float bulletDamage = 10f;
        

        [Header("최대속도")] 
        [SerializeField] public float maxSpeed = 10f;
        [SerializeField] public float dashMaxSpeed = 15f;
        [SerializeField] public float slidingMaxSpeed = 13f;
        [SerializeField] public float jumpMaxSpeed = 15f;
        
        [Header("가속도(이동조작 반응성)")]
        [SerializeField] private float baseAcc = 100f;
        [SerializeField] private float airAcc = 5f;
        [SerializeField] private float slidingAcc = 5f;

        [Header("대쉬/슬라이딩 파워")] 
        [SerializeField] public float dashPower = 30f;
        [SerializeField] public float slidingPower = 100f;
  
        public float BaseAcc => baseAcc;
        public float AirAcc => airAcc;
        public float SlidingAcc => slidingAcc;
        

        [Header("대쉬상태 관련변수")] 
        [SerializeField] private float dashDurationTime = 0.5f;

        public float DashDurationtime
        {
            get => dashDurationTime;
            private set => dashDurationTime = value;
        }
        
        [Header("마우스 감도")]
        [SerializeField] public float mouseSensitivity = 0.5f;
        
        
        [Header("점프력 관련 필드")]
        [SerializeField] private float jumpForce = 10f;

        public float CurrentJumpForce => jumpForce;

        # region Methods

        /// <summary>
        /// 필드 초기화 메서드
        /// </summary>
        private void Initialize()
        {
            // 상태값 초기화
            hp.Initialize();
            stamina.Initialize(1);
            overheat.Initialize();
            overheat.Value = 0;
        }
        
        
        public void AddOverHeat(float amount)
        {
            overheat.Value += amount;
            overheat.Value = Mathf.Clamp(overheat.Value, 0, overheat.maxValue);
        }
        
        # endregion
        
    }
}