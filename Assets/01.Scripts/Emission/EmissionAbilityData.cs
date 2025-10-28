using UnityEngine;

namespace _01.Scripts.Emission
{
    [CreateAssetMenu(fileName = "New EmissionAbilityData", menuName = "Emission/Emission Ability Data")]
    public class EmissionAbilityData : ScriptableObject
    {
        [Header("기본정보")] 
        public string abilityName; // 방출 이름
        [TextArea] public string description; // 설명

        public float damage;
        public float overheatCost;
        
        [Header("동작방식 정의")]
        [Tooltip("능력이 동작하는 방식을 정의하는 EmissionBehavior 에셋을 연결")]
        public EmissionBehavior behavior;
    }
}