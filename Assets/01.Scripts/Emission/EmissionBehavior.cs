using UnityEngine;

namespace _01.Scripts.Emission
{
    public abstract class EmissionBehavior : ScriptableObject
    {
        /// <summary>
        /// 방출의 행동을 정의하는 추상 메서드
        /// </summary>
        /// <param name="handler">방출의 행동을 실행하는 (PlayerEmission 컴포넌트)</param>
        /// <param name="playerCamera">플레이어의 1인칭 카메라 트랜스폼</param>
        /// <param name="data">방출의 데이터 (EmissionAbilityData SO 에셋)</param>
        public abstract void Execute(PlayerEmission handler, Transform playerCamera, EmissionAbilityData data);
    }
}