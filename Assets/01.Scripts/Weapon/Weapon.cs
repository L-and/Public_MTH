using System;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;

namespace _01.Scripts.Weapon
{
    public class Weapon : WeaponBehaviour
    {

        #region Serialize Fileds
        
        [Header("Firing")]

        [Tooltip("Is this weapon automatic? If yes, then holding down the firing button will continuously fire.")]
        [SerializeField] 
        private bool automatic;
        
        [Tooltip("탄의 발사속도")]
        [SerializeField]
        private float projectileImpulse = 400.0f;
        
        [Tooltip("Amount of shots this weapon can shoot in a minute. It determines how fast the weapon shoots.")]
        [SerializeField] 
        private int roundsPerMinutes = 200;
        
        [Tooltip("Mask of things recognized when firing.")]
        [SerializeField]
        private LayerMask mask;
        
        [Tooltip("Maximum distance at which this weapon can fire accurately. Shots beyond this distance will not use linetracing for accuracy.")]
        [SerializeField]
        private float maximumDistance = 500.0f;
        
        [Header("탄피배출 위치")]
        [Tooltip("Transform that represents the weapon's ejection port, meaning the part of the weapon that casings shoot from.")]
        [SerializeField]
        private Transform socketEjection;
        
        [Header("Resources")]

        [Tooltip("탄피 프리팹")]
        [SerializeField]
        private GameObject prefabCasing;
        
        [Tooltip("탄 프리팹 (무기를 발사할 때 생성됨)")]
        [SerializeField]
        private GameObject prefabProjectile;
        
        [Header("Audio Clips Reloads")]

        [Tooltip("재장전 오디오클립")]
        [SerializeField]
        private AudioClip audioClipReload;
        
        [Tooltip("탄창이 비었을 때, 재장전 오디오클립")]
        [SerializeField]
        private AudioClip audioClipReloadEmpty;
        
        [Header("Audio Clips Other")]

        [Tooltip("탄창이 비었을 때, 발사 오디오클립")]
        [SerializeField]
        private AudioClip audioClipFireEmpty;
        
        #endregion
        
        #region FIELDS

        /// <summary>
        /// 현재 장탄수
        /// </summary>
        private int _ammunitionCurrent;
        
        /// <summary>
        /// Weapon Animator.
        /// </summary>
        private Animator _animator;
        /// <summary>
        /// 무기의 부가정보 스크립트
        /// </summary>
        private WeaponAttachmentManagerBehaviour _attachmentManager;

        /// <summary>
        /// 반동 스크립트
        /// </summary>
        private Recoil _recoilScript;
        
        /// <summary>
        /// 오디오 소스
        /// </summary>
        private AudioSource _audioSource;

        #region 총구,탄창 스크립트 (Attachment Behaviours)
        
        /// <summary>
        /// 탄창정보 레퍼런스 스크립트
        /// </summary>
        private MagazineBehaviour _magazineBehaviour;
        /// <summary>
        /// 총구정보 레퍼런스 스크립트
        /// </summary>
        private MuzzleBehaviour _muzzleBehaviour;
            
        #endregion
        
        #endregion
        
        #region Unity Methods
        
        protected override void Awake()
        {
            // 컴포넌트들 캐싱
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _attachmentManager = GetComponent<WeaponAttachmentManagerBehaviour>();
            _recoilScript = transform.root.GetComponentInChildren<Recoil>();
        }
        protected override void Start()
        {
            #region Cache Attachment References
            
            //Get Magazine.
            _magazineBehaviour = _attachmentManager.GetEquippedMagazine();
            //Get Muzzle.
            _muzzleBehaviour = _attachmentManager.GetEquippedMuzzle();

            #endregion

            //Max Out Ammo.
            _ammunitionCurrent = _magazineBehaviour.GetAmmunitionTotal();
        }
        
        #endregion
        
        #region GETTERS
        
        public override Animator GetAnimator() => _animator;
        
        public override AudioClip GetAudioClipReload() => audioClipReload;
        public override AudioClip GetAudioClipReloadEmpty() => audioClipReloadEmpty;

        public override AudioClip GetAudioClipFireEmpty() => audioClipFireEmpty;
        public override AudioClip GetAudioClipFire() => _muzzleBehaviour.GetAudioClipFire();

        public override int GetAmmunitionCurrent() => _ammunitionCurrent;

        public override int GetAmmunitionTotal() => _magazineBehaviour.GetAmmunitionTotal();

        public override bool IsAutomatic()  => automatic;
        public override float GetRateOfFire() => roundsPerMinutes;

        public override bool IsFull() => _ammunitionCurrent == _magazineBehaviour.GetAmmunitionTotal();
        public override bool HasAmmunition() => _ammunitionCurrent > 0;
        
        // RuntimeAnimatorController을 사용하게되면 수정 필요
        public override RuntimeAnimatorController GetAnimatorController() => null;
        public override WeaponAttachmentManagerBehaviour GetAttachmentManager() => _attachmentManager;

        #endregion
        
        #region Methods

        public override void Fire(float spreadMultiplier = 1)
        {
            //We need a muzzle in order to fire this weapon!
            if (_muzzleBehaviour == null)
                return;

            Transform muzzleSocket = _muzzleBehaviour.GetSocket();

            // 탄이 없다면 공격발 사운드 재생
            if (!HasAmmunition())
            {
                _audioSource.PlayOneShot(GetAudioClipFireEmpty());
                return;
            }
            
            // 발사 애니메이션 재생
            const string animName = "Fire";
            _animator.Play(animName, 0, 0f);
            
            // 반동 적용
            _recoilScript.ApplyRecoil();
            
            // 총구화염 재생
            _muzzleBehaviour.Effect();
            
            // 장탄수 감소
            _ammunitionCurrent = Mathf.Clamp(_ammunitionCurrent - 1, 0, _magazineBehaviour.GetAmmunitionTotal());
            
            // 탄피 배출
            EjectCasing();
            
            // TODO MainCamera대신 플레이어 카메라로 변경 필요
            var playerCamera = Camera.main.transform;
            
            //Determine the rotation that we want to shoot our projectile in.
            Quaternion rotation = Quaternion.LookRotation(playerCamera.position + playerCamera.forward * 1000.0f - muzzleSocket.position);

            if (Physics.Raycast(new Ray(playerCamera.position, playerCamera.forward),
                    out RaycastHit hit, maximumDistance, mask))
            {
                rotation = Quaternion.LookRotation(hit.point - muzzleSocket.position);
            }
            
            GameObject projectile = Instantiate(prefabProjectile, muzzleSocket.position, rotation);
            
            // 탄두 발사 물리적용
            projectile.GetComponent<Rigidbody>().linearVelocity = projectile.transform.forward * projectileImpulse;
            
            // 발사 오디오클립 재생
            _audioSource.PlayOneShot(GetAudioClipFire());
        }

        public override void Reload()
        {
            string animName = HasAmmunition() ? "Reload" : "Reload Empty";
            _animator.Play(animName, 0, 0f);
        }
        
        // amount가 -1이면 탄약을 전부 충전
        public override void FillAmmunition(int amount)
        {
            _ammunitionCurrent = amount != -1 ? Mathf.Clamp(_ammunitionCurrent + amount, 
                0, GetAmmunitionTotal()) : _magazineBehaviour.GetAmmunitionTotal();
        }


        /// <summary>
        /// 탄피 배출
        /// </summary>
        public override void EjectCasing()
        {
            if(prefabCasing != null && socketEjection != null)
                Instantiate(prefabCasing, socketEjection.position, socketEjection.rotation);
        }
        
        #endregion
    }
}
