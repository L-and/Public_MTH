using _01.Scripts.Weapons_ScriptableObjects.Emission;
using _01.Scripts.Weapons_ScriptableObjects.SubWeapon;
using UnityEngine;

namespace _01.Scripts.Weapons_ScriptableObjects.Loadout
{
    /// <summary>
    /// 무기,보조무기,방출의 Data SO들을 저장하는 SO
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerLoadout", menuName = "Player/Loadout/PlayerLoadout", order = 1)]
    public class PlayerLoadoutsSO : ScriptableObject
    {
        // TODO 배열이 아니라 id:SO 형식의 딕셔너리로 변경해야 함
        // public WeaponData[] weapons;
        public SubWeaponData[] subWeapons;
        public EmissionAbilityData[] emissions;
    }

    public class PlayerLoadout
    {
        // public WeaponData Weapon;
        public SubWeaponData SubWeapon;
        public EmissionAbilityData Emission;
    }
}
