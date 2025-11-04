using InfimaGames.LowPolyShooterPack;
using UnityEngine;

namespace _01.Scripts.Weapon
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        #region Unity Methods
        /// <summary>
        /// Awake.
        /// </summary>
        protected virtual void Awake(){}

        /// <summary>
        /// Start.
        /// </summary>
        protected virtual void Start(){}

        /// <summary>
        /// Update.
        /// </summary>
        protected virtual void Update(){}

        /// <summary>
        /// Late Update.
        /// </summary>
        protected virtual void LateUpdate(){}
        #endregion
        
        #region GETTERS

        /// <summary>
        /// Returns the reload audio clip.
        /// </summary>
        public abstract AudioClip GetAudioClipReload();
        /// <summary>
        /// Returns the reload empty audio clip.
        /// </summary>
        public abstract AudioClip GetAudioClipReloadEmpty();

        /// <summary>
        /// Returns the fire empty audio clip.
        /// </summary>
        public abstract AudioClip GetAudioClipFireEmpty();

        /// <summary>
        /// Returns the fire audio clip.
        /// </summary>
        public abstract AudioClip GetAudioClipFire();
        
        /// <summary>
        /// Returns Current Ammunition. 
        /// </summary>
        public abstract int GetAmmunitionCurrent();
        /// <summary>
        /// Returns Total Ammunition.
        /// </summary>
        public abstract int GetAmmunitionTotal();

        /// <summary>
        /// Returns the Weapon's Animator component.
        /// </summary>
        public abstract Animator GetAnimator();
        
        /// <summary>
        /// Returns true if this weapon shoots in automatic.
        /// </summary>
        public abstract bool IsAutomatic();
        /// <summary>
        /// Returns true if the weapon has any ammunition left.
        /// </summary>
        public abstract bool HasAmmunition();

        /// <summary>
        /// Returns true if the weapon is full of ammunition.
        /// </summary>
        public abstract bool IsFull();
        /// <summary>
        /// Returns the weapon's rate of fire.
        /// </summary>
        public abstract float GetRateOfFire();

        /// <summary>
        /// Returns the RuntimeAnimationController the Character needs to use when this Weapon is equipped!
        /// </summary>
        public abstract RuntimeAnimatorController GetAnimatorController();
        /// <summary>
        /// Returns the weapon's attachment manager component.
        /// </summary>
        public abstract WeaponAttachmentManagerBehaviour GetAttachmentManager();
        
        #endregion
        
        #region METHODS

        /// <summary>
        /// 무기를 발사한다
        /// Fires the weapon.
        /// </summary>
        /// <param name="spreadMultiplier">Value to multiply the weapon's spread by. Very helpful to account for aimed spread multipliers.</param>
        public abstract void Fire(float spreadMultiplier = 1.0f);
        /// <summary>
        /// 무기를 재장전 한다
        /// </summary>
        public abstract void Reload();

        /// <summary>
        /// 장착한 무기의 탄약을 amount값만큼 채우거나, -1로 설정하면 전부 채운다
        /// </summary>
        public abstract void FillAmmunition(int amount);

        /// <summary>
        /// 무기에서 탄피를 배출한다
        /// Ejects a casing from the weapon. This is commonly called from animation events, but can be called from anywhere.
        /// </summary>
        public abstract void EjectCasing();

        #endregion
        
        #region Custom 메서드

        public abstract void Initialize();
        
        public abstract void SetRateOfFire(int rpm);

        public abstract void SetMagazineSize(int amount);

        #endregion
    }
}
