using System;
using UnityEditor.Animations;
using UnityEngine;

namespace _01.Scripts.Weapons_ScriptableObjects.Weapon
{
    [CreateAssetMenu(fileName = "New WeaponData", menuName = "Weapon/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        // 속성값이 변경되면 변경사항을 반영하는 메서드를 연결할 액션
        public event Action OnStatsChanged;
        
        [Header("기본정보")] 
        public string weaponName; // 총기명
        [TextArea] public string description;

        [Tooltip("공격력"), SerializeField] 
        private float damage;

        [Header("캐릭터 애니메이터")] 
        public AnimatorOverrideController characterAnimator;
        
        public float Damage
        {
            get => damage;
            set
            {
                damage = value;
                OnStatsChanged?.Invoke();
            }
        }
        
        [Tooltip("탄창용량"), SerializeField] 
        private int magazineSize;
        public int MagazineSize
        {
            get => magazineSize;
            set
            {
                magazineSize = value;
                OnStatsChanged?.Invoke();
            }
        }
        
        [Tooltip("RPM"), SerializeField] 
        private int rpm;
        public int Rpm
        {
            get => rpm;
            set
            {
                rpm = value;
                OnStatsChanged?.Invoke();
            }
        }
        
        [Header("반동세기")]
        [Tooltip("수직반동"), SerializeField] 
        private float verticalRecoil;
        public float VerticalRecoil
        {
            get => verticalRecoil;
            set
            {
                verticalRecoil = value;
                OnStatsChanged?.Invoke();
            }
        }
        
        [Tooltip("좌우반동"), SerializeField] 
        private float horizontalRecoil;
        public float HorizontalRecoil
        {
            get => horizontalRecoil;
            set
            {
                horizontalRecoil = value;
                OnStatsChanged?.Invoke();
            }
        }
        
        [Header("프리팹")] 
        public GameObject prefab;
    } 
}