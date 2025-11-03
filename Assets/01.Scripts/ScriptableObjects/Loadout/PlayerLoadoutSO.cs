using _01.Scripts.Emission;
using _01.Scripts.SubWeapon;
using UnityEngine;

namespace _01.Scripts.ScriptableObjects.Loadout
{
    [CreateAssetMenu(fileName = "PlayerLoadout", menuName = "Player/Loadout/PlayerLoadout", order = 1)]
    public class PlayerLoadoutSO : ScriptableObject
    {
        // TODO: WeaponDataSO SelectedWeapon; // 다른 브랜치에서 작업 중
        // TODO: SubWeaponDataSO SelectedSubWeapon; // 다른 브랜치에서 작업 중
        public EmissionAbilityData selectedEmission;
        public SubWeaponData selectedSubWeapon;
    }
}
