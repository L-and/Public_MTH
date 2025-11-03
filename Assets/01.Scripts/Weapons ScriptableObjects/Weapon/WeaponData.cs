using UnityEngine;

namespace _01.Scripts.Weapons_ScriptableObjects.Weapon
{
    [CreateAssetMenu(fileName = "New WeaponData", menuName = "Weapon/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("기본정보")] 
        public string weaponName; // 총기명
        [TextArea] public string description;

        [Tooltip("공격력")] public float damage;
        [Tooltip("탄창용량")] public int magazineSize;
        [Tooltip("RPM")] public int rpm;
        
        [Header("반동세기")]
        [Tooltip("수직반동")] public float verticalRecoil;
        [Tooltip("좌우반동")] public float horizontalRecoil;
        
        [Header("프리팹")] 
        public GameObject prefab;
    } 
}